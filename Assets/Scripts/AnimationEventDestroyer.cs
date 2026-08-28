using UnityEngine;

/// <summary>
/// Attach to your resolve-effect GameObject (alongside SpriteRenderer/Animator).
/// Hook DestroySelf() up as an Animation Event on the last frame of the clip
/// so the effect cleans itself up right when the animation finishes.
/// </summary>
public class AnimationEventDestroyer : MonoBehaviour
{
    // Called from an Animation Event - see setup steps.
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}