using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Wind : MonoBehaviour
{
    [Header("Raincoat")]
    [Tooltip("Must match the raincoat's itemId in item_database.json exactly")]
    [SerializeField] private string raincoatItemId = "raincoat";

    [Header("No raincoat")]
    [Tooltip("Push-back speed applied to the player (units/sec)")]
    [SerializeField] private float pushBackSpeed = 3f;

    private float travelSpeed;
    private float normalPlayerSpeed;
    private float slowedPlayerSpeed;
    private float despawnX;

    private PlayerController player;

    public void Init(float travelSpeed, float normalPlayerSpeed, float slowedPlayerSpeed, float despawnX)
    {
        this.travelSpeed = travelSpeed;
        this.normalPlayerSpeed = normalPlayerSpeed;
        this.slowedPlayerSpeed = slowedPlayerSpeed;
        this.despawnX = despawnX;
    }

    private void Update()
    {
        transform.position += Vector3.left * travelSpeed * Time.deltaTime;

        if (transform.position.x <= despawnX)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        AudioManager.Instance?.PlaySFX("thunder");
        other.TryGetComponent(out player);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (player == null || !other.CompareTag("Player")) return;

        if (IsRaincoatEquipped())
        {
            // Raincoat: ONLY slow down, no push
            player.SetSpeedMultiplier(slowedPlayerSpeed / normalPlayerSpeed);
            player.SetWindPush(0f);
        }
        else
        {
            // No raincoat: normal speed + pushed back
            player.SetSpeedMultiplier(1f);
            player.SetWindPush(-pushBackSpeed);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) ReleasePlayer();
    }

    // Wind despawns while the player is still inside -> Exit never fires, so reset here
    private void OnDestroy()
    {
        ReleasePlayer();
    }

    private void ReleasePlayer()
    {
        if (player != null)
        {
            player.SetSpeedMultiplier(1f);
            player.SetWindPush(0f);
        }
        player = null;
    }

    // Checks every worn item, so it still works with a second wearable on.
    private bool IsRaincoatEquipped()
    {
        return InventoryManager.Instance != null
            && InventoryManager.Instance.IsEquipped(raincoatItemId);
    }
}