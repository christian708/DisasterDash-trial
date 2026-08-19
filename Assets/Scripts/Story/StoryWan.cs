using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class StoryWan : MonoBehaviour, IPointerClickHandler
{
    [Header("Story Slides")]
    [SerializeField] private Image storyImage;
    [SerializeField] private Sprite[] boySlides;   // S1, S2, S3, S4, S5, S6, S7, S8
    [SerializeField] private Sprite[] girlSlides;  // S1, S2G, S3, S4, S5, S6G, S7, S8

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "LevelMenu";

    private Sprite[] activeSlides;
    private int currentIndex = 0;

    void Start()
    {
        string selectedCharacter = PlayerPrefs.GetString("SelectedCharacter", "Boy");
        activeSlides = (selectedCharacter == "Girl") ? girlSlides : boySlides;

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
        if (storyImage != null && activeSlides.Length > 0)
        {
            storyImage.sprite = activeSlides[currentIndex];
        }
    }
}