using UnityEngine;

/// <summary>
/// Attach to any element GameObject (fire, flood surge, rock, branch, etc.)
/// to trigger sounds from AudioManager's SFX library.
///
/// Set the IDs in the Inspector to match entries in AudioManager's SFX Library.
/// Call PlaySound(id) from other scripts on the same element, or use
/// PlayOnStart / PlayOnEnable for ambient looping sounds.
/// </summary>
public class ElementSoundPlayer : MonoBehaviour
{
    [Header("Sound IDs")]
    [Tooltip("ID of the ambient/start sound (e.g. 'fire'). Played when this element activates. Leave blank to skip.")]
    [SerializeField] private string onStartSoundId = "";

    [Tooltip("ID of the end/extinguish sound (e.g. 'fire_extinguish'). Call PlayEndSound() from other scripts when the element finishes.")]
    [SerializeField] private string onEndSoundId = "";

    [Header("Playback")]
    [Tooltip("Play the start sound automatically when this GameObject becomes active.")]
    [SerializeField] private bool playOnEnable = true;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        if (playOnEnable && !string.IsNullOrEmpty(onStartSoundId))
            PlaySound(onStartSoundId);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Play any sound from the library by its ID.</summary>
    public void PlaySound(string id)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(id);
        else
            Debug.LogWarning("[ElementSoundPlayer] AudioManager not found in scene.");
    }

    /// <summary>Shortcut — plays the onStartSoundId.</summary>
    public void PlayStartSound()
    {
        if (!string.IsNullOrEmpty(onStartSoundId))
            PlaySound(onStartSoundId);
    }

    /// <summary>Shortcut — plays the onEndSoundId. Call this when the element is extinguished / destroyed.</summary>
    public void PlayEndSound()
    {
        if (!string.IsNullOrEmpty(onEndSoundId))
            PlaySound(onEndSoundId);
    }
}
