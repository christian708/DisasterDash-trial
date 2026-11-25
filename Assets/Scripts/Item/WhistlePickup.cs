using UnityEngine;

public class WhistlePickup : MonoBehaviour, IInteractable
{
    [SerializeField] StoryPopup storyPopup;      // the Image child with StoryPopup.cs
    [SerializeField] GameObject storyPopupPanel; // the root StoryPopupPanel Canvas
    [SerializeField] GameObject ls0;             // LS_0 — hidden after story finishes

    bool used;

    public void OnPickUp()
    {
        if (used) return;
        used = true;

        // Hide whistle
        foreach (var r in GetComponentsInChildren<SpriteRenderer>()) r.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;

        // Activate the parent panel first so the child becomes visible
        if (storyPopupPanel != null) storyPopupPanel.SetActive(true);

        storyPopup.onFinished.AddListener(OnStoryFinished);
        storyPopup.Show();
    }

    void OnStoryFinished()
    {
        storyPopup.onFinished.RemoveListener(OnStoryFinished);

        if (ls0 != null) ls0.SetActive(false);

        gameObject.SetActive(false);
    }

    public void OnUse() { }
}