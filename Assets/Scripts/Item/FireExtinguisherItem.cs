using UnityEngine;

public class FireExtinguisherItem : MonoBehaviour, IInteractable
{
    [SerializeField] private FireHazard nearbyFire; // drag Fire1 here
    [SerializeField] private Transform playerHoldPoint; // optional, for the "holding" visual
    [SerializeField] private GameObject heldItemVisual; // optional, small icon shown while held

    private bool isPickedUp = false;

    public void OnPickUp()
    {
        if (isPickedUp)
        {
            PutDown();
        }
        else
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        isPickedUp = true;
        Debug.Log("Picked up fire extinguisher");

        gameObject.SetActive(false); // remove from world

        if (heldItemVisual != null)
            heldItemVisual.SetActive(true); // show held icon on player
    }

    private void PutDown()
    {
        isPickedUp = false;
        Debug.Log("Put down fire extinguisher");

        // Reactivate in the world at the player's current position
        if (playerHoldPoint != null)
        {
            transform.position = playerHoldPoint.position;
        }
        gameObject.SetActive(true);

        if (heldItemVisual != null)
            heldItemVisual.SetActive(false);
    }

    public void OnUse()
    {
        if (!isPickedUp)
        {
            Debug.Log("Pick up the extinguisher first (Press B)");
            return;
        }

        if (nearbyFire != null)
        {
            Debug.Log("Fire extinguished!");
            nearbyFire.gameObject.SetActive(false);
        }
    }
}