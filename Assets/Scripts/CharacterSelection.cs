using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public GameObject boyHighlight;
    public GameObject girlHighlight;

    [Header("Where to go after confirming")]
    [SerializeField] private string nextSceneName = "Level0_Fire_Interior";

    private void Start()
    {
        // Boy is selected by default
        SelectBoy();
    }

    public void SelectBoy()
    {
        PlayerPrefs.SetString("SelectedCharacter", "Boy");

        if (boyHighlight != null)
            boyHighlight.SetActive(true);

        if (girlHighlight != null)
            girlHighlight.SetActive(false);

        Debug.Log("Boy selected!");
    }

    public void SelectGirl()
    {
        PlayerPrefs.SetString("SelectedCharacter", "Girl");

        if (boyHighlight != null)
            boyHighlight.SetActive(false);

        if (girlHighlight != null)
            girlHighlight.SetActive(true);

        Debug.Log("Girl selected!");
    }

    // Hook this up to a "Confirm" / "Start" / "Play" button's OnClick()
    public void ConfirmSelection()
    {
        string chosenCharacter = PlayerPrefs.GetString("SelectedCharacter", "Boy");

        // Keep GameSession's data in sync so the character choice travels along
        // with whatever save data was already staged (New Game / Continue).
        SaveData data = GameSession.LoadedData ?? new SaveData();
        data.selectedCharacter = chosenCharacter;
        GameSession.LoadedData = data;

        SceneManager.LoadScene(nextSceneName);
    }
}