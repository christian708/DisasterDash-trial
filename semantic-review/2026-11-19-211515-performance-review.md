# Mobile performance hot spots across the hazard and player systems

This codebase is a Unity 2D mobile game with seven levels, each running multiple overlapping hazard scripts. The majority of the per-frame cost is concentrated in a small number of scripts, but those scripts are either always active or active during the game's most chaotic moments. The main categories of waste are: reflection called every physics tick, `GetComponent` calls inside stay-trigger callbacks that run continuously while the player stands in a zone, `FindAnyObjectByType` called at scene start and stored only sometimes, `Debug.Log` left in production collision handlers, and `Camera.main` accessed every frame in a loop. None of these are individually catastrophic on desktop, but they stack up on mobile where GC pressure and per-frame allocations directly cause frame drops.

**Watch for:** (confirmed) Reflection + heap allocations in `SmokeHazard.HasMask()` called every physics tick. (confirmed) `GetComponentInParent` called on every `OnTriggerStay2D` tick in `SmokeHazard`. (confirmed) `Debug.Log` with string interpolation inside `OnTriggerEnter2D` in `RockHazard` and `Branch`. (confirmed) `Camera.main` dereferenced in a per-frame loop in `EquakeManager.IsBelowCamera`. (confirmed) `FindAnyObjectByType` at runtime in `FallenTree.ChopRoutine` and `CementDebrisSpawner.Start`.

**Verdict**: NEEDS_CHANGES

---

## High-level view

`SmokeHazard` is the most expensive script in the codebase per frame while the player stands in smoke. `HasMask()` uses `System.Reflection` to discover and invoke the inventory's equip method at runtime, allocating `MethodInfo` and `object[]` on every call. `OnTriggerStay2D` calls this, plus two `GetComponentInParent` lookups, on every fixed-update tick. During the volcano level where smoke is a persistent zone hazard, this runs 50+ times per second.

`EquakeManager.IsBelowCamera()` calls `Camera.main` on every iteration of the debris-tracking inner loop in `DebrisSlot`. `Camera.main` does a tagged object lookup internally and is not cached; during active earthquake phase with multiple debris slots running simultaneously, this fires several times per frame.

`RockHazard` and `Branch` both have `Debug.Log` calls with string interpolation (`$"..."`) inside `OnTriggerEnter2D`. Each interpolation allocates a new string on the managed heap. These are in collision handlers that fire at the moment of impact—not every frame, but on mobile devices these allocations can contribute to GC spikes at the worst possible time.

`FallenTree.ChopRoutine` calls `FindAnyObjectByType<PlayerController>()` inside a coroutine that starts mid-gameplay. `CementDebrisSpawner.Start` does the same on scene load. `FindAnyObjectByType` traverses every live object in the scene; wiring the reference in the Inspector or caching it in `Awake` is free.

`PlayerController.HandleAnimation` calls `AudioManager.Instance?.PlayRunSound()` or `StopRunSound()` unconditionally every `Update` frame. `PlayRunSound` has an `isPlaying` guard so the AudioSource itself only restarts once, but the method call, null check, and property access still happen 60 times per second regardless. The same pattern appears in `CameraShake`, which calls `AudioManager.Instance?.StopLoopingSFX()` every frame that `wasShakinLastFrame` is true before zeroing the flag—that part is a single-frame window and is fine.

`HeldItemIndicatorUI.Update` polls `InventoryManager.WearablesVersion` and `PlayerHeldItem.HeldItemId` every frame via version checks. The version-check pattern is good and prevents redundant work. However, `Debug.Log` is called inside `UpdateWearables` on every version change, which will fire whenever a wearable is equipped.

---

<details>
<summary>Issues (8)</summary>

1. **Reflection in SmokeHazard.HasMask** — `HasMask()` uses `System.Reflection` to find and invoke `InventoryManager.IsEquipped` at runtime, allocating on every physics tick while the player is in smoke. Replace with a direct `InventoryManager.Instance.IsEquipped(maskItemId)` call.

2. **GetComponentInParent every stay-tick in SmokeHazard** — `OnTriggerStay2D` calls `other.GetComponentInParent<PlayerController>()` and `other.GetComponentInParent<PlayerHealth>()` on every fixed-update tick. Cache both on `OnTriggerEnter2D` and clear them on `OnTriggerExit2D`.

3. **Camera.main in per-frame debris loop** — `EquakeManager.IsBelowCamera` calls `Camera.main` on every invocation; with multiple debris slots running this fires several times per frame. Cache `Camera.main` in `Awake` or `OnEnable`.

4. **Debug.Log with string interpolation in collision handlers** — `RockHazard.OnTriggerEnter2D` and `Branch.OnTriggerEnter2D` both allocate interpolated strings at the moment of player contact. Wrap in `#if UNITY_EDITOR` or remove before shipping.

5. **FindAnyObjectByType at runtime in FallenTree** — `ChopRoutine` calls `FindAnyObjectByType<PlayerController>()` mid-gameplay. This traverses every live object in the scene. Cache the reference in `Awake` or accept it as a parameter from `TreeChopZone`, which already has access to the player indirectly.

6. **FindAnyObjectByType in CementDebrisSpawner.Start** — `Start` calls `FindAnyObjectByType<PlayerController>()`. Assign the reference in the Inspector or find it in `Awake` via tag, same as `DebrisProximityTrigger` already does correctly.

7. **AudioManager called every frame in PlayerController** — `HandleAnimation` calls `PlayRunSound()` / `StopRunSound()` unconditionally every `Update`. Guard with a local `wasMoving` bool so the call only happens on the transition frame (moving→stopped or stopped→moving).

8. **Debug.Log in HeldItemIndicatorUI on every wearables change** — `UpdateWearables` logs a slot assignment on every version increment. This fires whenever the player equips a wearable. Wrap in `#if UNITY_EDITOR`.

</details>

---

<details>
<summary>Details</summary>

## Reflection overhead in SmokeHazard

`SmokeHazard.HasMask()` is the most expensive method in the codebase per-frame while it's active. Rather than calling `InventoryManager.Instance.IsEquipped(maskItemId)` directly, it uses `System.Reflection` to discover which of seven candidate method names the class exposes, then invokes through `MethodInfo.Invoke`. The call site is `OnTriggerStay2D`, which fires on every physics tick (50 times/second by default) while the player stands in smoke.

The reflection path allocates:
- `new[] { typeof(string) }` — a `Type[]` on each `GetMethod` probe attempt
- `new object[] { maskItemId }` — a boxed `object[]` on each `Invoke`
- Intermediate strings in `.ToLower()` during the field/property scan fallback

`InventoryManager` already has a public `IsEquipped(string)` method. The entire `HasMask()` method and the `EquipMethodNames` static array can be replaced with:

```csharp
private bool HasMask()
{
    return InventoryManager.Instance != null
        && InventoryManager.Instance.IsEquipped(maskItemId);
}
```

The same `OnTriggerStay2D` calls `other.GetComponentInParent<PlayerController>()` and conditionally `other.GetComponentInParent<PlayerHealth>()` on every tick. These aren't free — `GetComponentInParent` walks up the transform hierarchy. Cache both on `OnTriggerEnter2D`:

```csharp
private PlayerController cachedPC;
private PlayerHealth cachedHealth;

private void OnTriggerEnter2D(Collider2D other)
{
    if (!IsPlayer(other)) return;
    cachedPC = other.GetComponentInParent<PlayerController>();
    cachedHealth = other.GetComponentInParent<PlayerHealth>();
    damageTimer = 0f;
}

private void OnTriggerExit2D(Collider2D other)
{
    if (!IsPlayer(other)) return;
    cachedPC = null;
    cachedHealth = null;
}
```

There is a secondary issue in `IsPlayer`: it calls `other.GetComponentInParent<PlayerController>()` as a fallback when `CompareTag` fails. If this branch fires, the lookup runs twice per tick (once in `IsPlayer`, once in the stay handler). `IsPlayer` should rely solely on `CompareTag`.

## Camera.main in the debris tracking loop

`EquakeManager.DebrisSlot` is a coroutine running once per active debris piece. Its inner `while` loop calls `IsBelowCamera(debris.transform.position)` every frame. `IsBelowCamera` calls `Camera.main`, which Unity implements as `FindObjectOfType<Camera>()` with a tag filter — a scene traversal every call.

During the earthquake's falling debris phase, with `slotsPerPrefab` controlling how many simultaneous coroutines run, this means 2–4 `Camera.main` lookups per frame for the duration of `rocksDuration` (15 seconds by default). Add `private Camera cachedCamera;` and assign it in `Awake`.

## Heap allocations in collision handlers

`RockHazard.OnTriggerEnter2D` and `Branch.OnTriggerEnter2D` both execute a `Debug.Log($"... helmet equipped = {...}")` at the moment of player contact. String interpolation always allocates, so every hit generates two or three short-lived heap objects. On mobile, GC collections are more expensive and less predictable than on desktop, and collisions are exactly the moment a GC pause would be most visible. Both logs should be wrapped in `#if UNITY_EDITOR` blocks, or removed.

## FindAnyObjectByType at gameplay time

`FallenTree.ChopRoutine` is a coroutine that starts the moment the player uses the machete on a tree. Its first line is `FindAnyObjectByType<PlayerController>()`, which walks every loaded object mid-gameplay. `TreeChopZone.OnUse` already calls `tree.Chop(...)` and has access to the player context; `FallenTree` should take a `[SerializeField] private PlayerController player;` or resolve via tag once in `Awake`.

`CementDebrisSpawner.Start` does the same. `FindAnyObjectByType` is slower than `GameObject.FindGameObjectWithTag("Player")`; both are unnecessary when the reference can be dragged into the Inspector, as `DebrisProximityTrigger` demonstrates.

## Per-frame AudioManager calls in PlayerController

`PlayerController.HandleAnimation` runs every `Update` frame. The last four lines unconditionally call into `AudioManager`:

```csharp
if (isMoving)
    AudioManager.Instance?.PlayRunSound();
else
    AudioManager.Instance?.StopRunSound();
```

`PlayRunSound` guards against double-starting with `if (footstepSource.isPlaying) return;`, so the AudioSource doesn't stutter. But the method call, null-conditional dereference, and property read still happen 60 times per second. A `private bool wasMoving` field on `PlayerController` limits the call to the two transition frames:

```csharp
if (isMoving != wasMoving)
{
    wasMoving = isMoving;
    if (isMoving) AudioManager.Instance?.PlayRunSound();
    else AudioManager.Instance?.StopRunSound();
}
```

## LandslideBlock fade loop allocations

`LandslideBlock.RemoveRoutine` calls `GetComponentsInChildren<Collider2D>()` and `GetComponentsInChildren<SpriteRenderer>()` at the start of the coroutine, allocating two arrays. Not a recurring hot path, but caching both as fields in `Awake` removes the allocations for free.

## DebrisProximityTrigger distance check

`DebrisProximityTrigger.Update` calls `Vector2.Distance` every frame until triggered. `Vector2.Distance` computes a square root; since the comparison is against a constant `triggerRadius`, replacing it with `sqrMagnitude` and `triggerRadius * triggerRadius` avoids the sqrt entirely. After triggering, the script sets `triggered = true` and returns immediately, so the fix only matters for the period before trigger—but every stationary debris piece in the level runs this until the player approaches.

```csharp
// Before
float dist = Vector2.Distance(transform.position, playerTransform.position);
if (dist <= triggerRadius)

// After
float sqrDist = ((Vector2)transform.position - (Vector2)playerTransform.position).sqrMagnitude;
if (sqrDist <= triggerRadius * triggerRadius)
```

</details>

---

<details>
<summary>Files changed</summary>

| File | Issue |
|---|---|
| `Elements/SmokeHazard.cs` | Reflection in `HasMask()`, `GetComponentInParent` on every stay tick |
| `Screen Shake/EquakeManager.cs` | `Camera.main` in per-frame debris tracking loop |
| `Screen Shake/RockHazard.cs` | `Debug.Log` with string interpolation in `OnTriggerEnter2D` |
| `Elements/Branch.cs` | `Debug.Log` with string interpolation in `OnTriggerEnter2D` |
| `Elements/FallenTree.cs` | `FindAnyObjectByType` mid-gameplay in coroutine |
| `Elements/CementDebrisSpawner.cs` | `FindAnyObjectByType` in `Start` |
| `Player/PlayerController.cs` | `AudioManager` called every frame in `HandleAnimation` |
| `Elements/DebrisProximityTrigger.cs` | `Vector2.Distance` (sqrt) every frame; use `sqrMagnitude` |
| `Elements/LandslideBlock.cs` | `GetComponentsInChildren` allocations at coroutine start (low priority) |
| `Item/HeldItemIndicatorUI.cs` | `Debug.Log` on every wearables version change |

Full source: `c:\Users\ANKIE\UnityProjects\Assets\Scripts`

</details>
