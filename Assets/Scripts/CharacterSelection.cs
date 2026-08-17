using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    public GameObject boyHighlight;
    public GameObject girlHighlight;

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
}