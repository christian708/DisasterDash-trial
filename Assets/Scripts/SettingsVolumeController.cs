using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to any GameObject in the Settings scene.
/// Wire the two Slider references in the Inspector.
/// Values are saved to PlayerPrefs so they persist across sessions.
/// </summary>
public class SettingsVolumeController : MonoBehaviour
{
    [Header("Sliders (0 – 100)")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MusicKey = "Vol_Music";
    private const string SFXKey   = "Vol_SFX";

    private void Start()
    {
        // Load saved values, default 80
        float music = PlayerPrefs.GetFloat(MusicKey, 80f);
        float sfx   = PlayerPrefs.GetFloat(SFXKey,   80f);

        // Set slider values without triggering onValueChanged yet
        musicSlider.SetValueWithoutNotify(music);
        sfxSlider.SetValueWithoutNotify(sfx);

        // Apply immediately so AudioManager reflects saved state on enter
        ApplyMusic(music);
        ApplySFX(sfx);

        // Wire listeners
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
    }

    private void OnDestroy()
    {
        musicSlider?.onValueChanged.RemoveListener(OnMusicChanged);
        sfxSlider?.onValueChanged.RemoveListener(OnSFXChanged);
    }

    // ── Callbacks ─────────────────────────────────────────────────────────────

    private void OnMusicChanged(float value)
    {
        ApplyMusic(value);
        PlayerPrefs.SetFloat(MusicKey, value);
        PlayerPrefs.Save();
    }

    private void OnSFXChanged(float value)
    {
        ApplySFX(value);
        PlayerPrefs.SetFloat(SFXKey, value);
        PlayerPrefs.Save();
    }

    // ── Apply to AudioManager ─────────────────────────────────────────────────

    private void ApplyMusic(float sliderValue)
    {
        // Slider is 0–100, AudioManager volumes are 0–1
        float v = sliderValue / 100f;
        AudioManager.Instance?.SetBGMVolume(v);
    }

    private void ApplySFX(float sliderValue)
    {
        float v = sliderValue / 100f;
        AudioManager.Instance?.SetSFXVolume(v);
        AudioManager.Instance?.SetElementVolume(v);
        AudioManager.Instance?.SetUIVolume(v);
        AudioManager.Instance?.SetFootstepVolume(v);
    }
}
