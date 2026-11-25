using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    // every interactable the player is currently touching
    private readonly Dictionary<Collider2D, IInteractable> nearby = new Dictionary<Collider2D, IInteractable>();
    private readonly List<Collider2D> tempKeys = new List<Collider2D>();

    private bool wasUsePressed = false;
    private bool wasPickUpPressed = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        Register(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // re-acquire anything that was dropped (e.g. after another object deactivated)
        if (!nearby.ContainsKey(other)) Register(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        nearby.Remove(other);
    }

    private void Register(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable == null) return;

        if (!nearby.ContainsKey(other))
            Debug.Log("Nearby interactable: " + other.gameObject.name);

        nearby[other] = interactable;
    }

    private void Prune()
    {
        tempKeys.Clear();
        foreach (var kv in nearby)
        {
            var mb = kv.Value as MonoBehaviour;
            if (kv.Key == null || mb == null || !mb.isActiveAndEnabled)
                tempKeys.Add(kv.Key);
        }
        foreach (var k in tempKeys) nearby.Remove(k);
    }

    private IInteractable Nearest()
    {
        IInteractable best = null;
        float bestDist = float.MaxValue;
        foreach (var kv in nearby)
        {
            float d = ((Vector2)kv.Key.bounds.center - (Vector2)transform.position).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = kv.Value; }
        }
        return best;
    }

    void Update()
    {
        Prune();

        if (nearby.Count == 0)
        {
            wasUsePressed = false;
            wasPickUpPressed = false;
            return;
        }

        bool useDown = MobileButton.UsePressed || (Keyboard.current != null && Keyboard.current.eKey.isPressed);
        bool pickupDown = MobileButton.PickUpPressed || (Keyboard.current != null && Keyboard.current.fKey.isPressed);

        if (useDown && !wasUsePressed)
        {
            // Use: offer it to everything nearby; each target ignores it unless the held item matches
            var list = new List<IInteractable>(nearby.Values);
            foreach (var i in list)
            {
                var mb = i as MonoBehaviour;
                if (mb != null && mb.isActiveAndEnabled) i.OnUse();
            }
        }

        if (pickupDown && !wasPickUpPressed)
        {
            // Pick up: only the nearest one
            Nearest()?.OnPickUp();
        }

        wasUsePressed = useDown;
        wasPickUpPressed = pickupDown;
    }
}