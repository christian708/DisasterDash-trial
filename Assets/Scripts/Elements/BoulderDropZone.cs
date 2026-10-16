using UnityEngine;

public class BoulderDropZone : MonoBehaviour
{
    [SerializeField] private BoulderHazard boulder;
    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        boulder.Drop();
    }
}