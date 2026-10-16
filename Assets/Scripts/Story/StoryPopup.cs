using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StoryPopup : MonoBehaviour
{
    [SerializeField] private Image slideImage;
    [SerializeField] private Sprite boySlide;
    [SerializeField] private Sprite girlSlide;
    [SerializeField] private Sprite sharedSlide;

    public UnityEvent onFinished;

    private Sprite[] slides;
    private int index;

    public void Show()
    {
        slides = new Sprite[] { IsBoy() ? boySlide : girlSlide, sharedSlide };
        index = 0;
        slideImage.sprite = slides[0];
        gameObject.SetActive(true);
    }

    public void Next()
    {
        index++;
        if (index >= slides.Length)
        {
            gameObject.SetActive(false);
            onFinished.Invoke();
            return;
        }
        slideImage.sprite = slides[index];
    }

    private bool IsBoy()
    {
        // CHANGE THIS to however your CharacterSelection stores the choice
        return PlayerPrefs.GetString("SelectedCharacter", "Boy") == "Boy";
    }
}