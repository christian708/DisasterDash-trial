using UnityEngine;

/// <summary>
/// Attach to the wind gust prefab. Moves the object from right to left
/// and slows the player's speed while they're inside it.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Wind : MonoBehaviour
{
    private float travelSpeed;
    private float normalPlayerSpeed;
    private float slowedPlayerSpeed;
    private float despawnX;

    public void Init(float travelSpeed, float normalPlayerSpeed, float slowedPlayerSpeed, float despawnX)
    {
        this.travelSpeed = travelSpeed;
        this.normalPlayerSpeed = normalPlayerSpeed;
        this.slowedPlayerSpeed = slowedPlayerSpeed;
        this.despawnX = despawnX;
    }

    private void Update()
    {
        // Move right -> left
        transform.position += Vector3.left * travelSpeed * Time.deltaTime;

        if (transform.position.x <= despawnX)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            player.SetSpeed(slowedPlayerSpeed);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerController player))
        {
            player.SetSpeed(normalPlayerSpeed);
        }
    }
}