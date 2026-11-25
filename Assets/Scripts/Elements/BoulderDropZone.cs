using UnityEngine;

public class BoulderDropZone : MonoBehaviour
{
    [SerializeField] private BoulderHazard boulder;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (boulder == null) return;

        // Still falling, resting or crumbling: wait until it's gone.
        if (boulder.IsDropping) return;

        boulder.ResetBoulder(); // back to its start position, visible again
        boulder.Drop();
    }
}