using UnityEngine;

/// <summary>
/// Attach to any GameObject in the LevelMenu (or MainMenu) scene.
/// Tells AudioManager to switch back to the default BGM when this scene loads,
/// overriding any story music that was playing before.
/// </summary>
public class SceneMenuBGM : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDefaultBGM();
    }
}
