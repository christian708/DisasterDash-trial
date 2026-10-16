using UnityEngine;

public class WhistleTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private StoryPopup storyPopup;
    private bool used = false;

    public void OnPickUp()
    {
        if (used) return;
        used = true;
        storyPopup.Show();
        gameObject.SetActive(false);
    }

    public void OnUse() { }
}