using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Back : MonoBehaviour
{
    [SerializeField] private string targetScene = "characSelect";

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(GoBack);
    }

    public void GoBack()
    {
        SceneManager.LoadScene(targetScene);
    }
}