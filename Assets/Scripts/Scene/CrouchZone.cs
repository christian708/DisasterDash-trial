using UnityEngine;

public class CrouchZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController p = other.GetComponentInParent<PlayerController>();
        if (p != null) p.SetCrouch(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController p = other.GetComponentInParent<PlayerController>();
        if (p != null) p.SetCrouch(false);
    }
}