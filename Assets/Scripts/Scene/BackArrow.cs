using UnityEngine;
using UnityEngine.SceneManagement;

public class BackArrow : MonoBehaviour
{
    [SerializeField] private string targetScene = "Level0_Fire_Interior";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneContoller.instance.LoadScene(targetScene);
        }
    }
}
