using UnityEngine;
using UnityEngine.SceneManagement;

public class ArrowFinishPoint : MonoBehaviour
{
    [SerializeField] private string targetScene = "Level1_Fire";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneContoller.instance.LoadScene(targetScene);
        }
    }
}