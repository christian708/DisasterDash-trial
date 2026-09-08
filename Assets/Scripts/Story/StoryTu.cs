using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StoryTu : MonoBehaviour, IPointerClickHandler
{
    [Header("Story Slides")]
    [SerializeField] private Image storyImage;

    [SerializeField] private Sprite boySlide1;
    [SerializeField] private Sprite girlSlide1;


    [SerializeField] private Sprite sharedSlide2;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "LevelMenu";

    private Sprite[] activeSlides;
    private int currentIndex = 0;

    void Start()
    {
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Boy");

        if (selectedCharacter == "Girl")
        {
            activeSlides = new Sprite[]
            {
                girlSlide1,
                sharedSlide2
            };
        }
        else
        {
            activeSlides = new Sprite[]
            {
                boySlide1,
                sharedSlide2
            };
        }

        currentIndex = 0;
        ShowCurrentSlide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        currentIndex++;

        if (currentIndex >= activeSlides.Length)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            ShowCurrentSlide();
        }
    }

    private void ShowCurrentSlide()
    {
        if (storyImage != null && activeSlides != null && activeSlides.Length > 0)
        {
            storyImage.sprite = activeSlides[currentIndex];
        }
    }
}