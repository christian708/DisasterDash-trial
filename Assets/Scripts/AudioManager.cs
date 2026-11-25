using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton that owns two AudioSources:
///   - one for looping background music
///   - one for one-shot element sound effects
///
/// Persists across scenes via DontDestroyOnLoad and automatically switches
/// BGM based on which scene loads — no per-scene setup needed.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Background Music")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip defaultBGM;
    [Range(0f, 1f)]
    [SerializeField] private float bgmVolume = 0.5f;

    [Header("Story Music")]
    [Tooltip("BGM that plays during Story1–Story8 scenes.")]
    [SerializeField] private AudioClip storyBGMClip;

    [Header("Element Sound Effects")]
    [SerializeField] private AudioSource sfxSource;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)]
    [Tooltip("Separate volume for element sounds (fire, flood etc). Defaults higher so they cut through BGM.")]
    [SerializeField] private float elementVolume = 3f;

    [Header("Looping Element Sound")]
    [Tooltip("Dedicated AudioSource for looping hazard sounds (e.g. earthquake). Separate from SFX so it doesn't interrupt one-shots.")]
    [SerializeField] private AudioSource loopSource;
    [Range(0f, 1f)]
    [SerializeField] private float loopVolume = 1f;

    [Header("UI Sounds")]
    [Tooltip("Clip played when any UI button is clicked or the story screen is tapped.")]
    [SerializeField] private AudioClip uiClickClip;
    [Range(0f, 1f)]
    [SerializeField] private float uiVolume = 1f;

    [Header("Player Footsteps")]
    [Tooltip("Looping clip played while the player is running.")]
    [SerializeField] private AudioClip runningClip;
    [SerializeField] private AudioSource footstepSource;
    [Range(0f, 1f)]
    [SerializeField] private float footstepVolume = 0.8f;

    [Header("SFX Library")]
    [Tooltip("Add an entry for each element sound. The ID must match what ElementSoundPlayer sends (e.g. 'fire', 'fire_extinguish').")]
    [SerializeField] private SFXEntry[] sfxLibrary;

    // Scene names that use the default (level) BGM
    private static readonly string[] levelScenes =
    {
        "Level0_Fire_Interior",
        "Level1_Fire",
        "Level2_Earthquake_Interior",
        "Level3_Earthquake",
        "Level4_Typhoon_In",
        "Level5_Typhoon",
        "Level6_Flood_In",
        "Level7_Flood",
        "Level8_LandslideIn",
        "Level9_Land",
        "Level10_VolIn",
        "Level11_Vol",
        "Level12_Mixed",
        "LevelMenu",
        "MainMenu"
    };

    // Scene names that use the story BGM
    private static readonly string[] storyScenes =
    {
        "Story1", "Story2", "Story3", "Story4",
        "Story5", "Story6", "Story7", "Story8",
        "Fire", "Fire1", "Fl", "Equake", "Equake1", "LS", "Tp", "VOL"
    };

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgmSource != null) bgmSource.volume = bgmVolume;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Restore saved volumes
        float music = PlayerPrefs.GetFloat("Vol_Music", 80f) / 100f;
        float sfx   = PlayerPrefs.GetFloat("Vol_SFX",   80f) / 100f;
        bgmVolume      = music;
        sfxVolume      = sfx;
        elementVolume  = sfx;
        uiVolume       = sfx;
        footstepVolume = sfx;
        if (bgmSource != null) bgmSource.volume = bgmVolume;

        // Play the correct BGM for whichever scene we started in
        HandleBGMForScene(SceneManager.GetActiveScene().name);
    }

    // ── Scene BGM Switching ───────────────────────────────────────────────────

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleBGMForScene(scene.name);
    }

    private void HandleBGMForScene(string sceneName)
    {
        if (IsInList(sceneName, storyScenes))
        {
            PlayStoryBGM();
        }
        else if (IsInList(sceneName, levelScenes))
        {
            PlayDefaultBGM();
        }
        // Any other scene (MainMenu, Charac_Select, etc.) keeps whatever is playing
    }

    private static bool IsInList(string sceneName, string[] list)
    {
        foreach (string s in list)
        {
            if (string.Equals(s, sceneName, System.StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    // ── Background Music ──────────────────────────────────────────────────────

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null) return;

        if (clip == null)
        {
            bgmSource.Stop();
            return;
        }

        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void PlayDefaultBGM()
    {
        if (defaultBGM != null)
            PlayBGM(defaultBGM);
    }

    public void PlayStoryBGM()
    {
        if (storyBGMClip != null)
            PlayBGM(storyBGMClip);
    }

    public void StopBGM() => bgmSource?.Stop();

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmSource != null) bgmSource.volume = bgmVolume;
    }

    // ── UI Sounds ─────────────────────────────────────────────────────────────

    /// <summary>Called by UIButtonSound on any button click.</summary>
    public void PlayUIClick()
    {
        if (sfxSource == null || uiClickClip == null) return;
        sfxSource.PlayOneShot(uiClickClip, uiVolume);
    }

    /// <summary>Called by story screens on tap — reuses the same UI click clip.</summary>
    public void PlayTapSound() => PlayUIClick();

    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
    }

    // ── Looping Element Sound ─────────────────────────────────────────────────

    /// <summary>
    /// Plays a clip on the dedicated loop source (e.g. earthquake rumble during shake).
    /// Stops any previously looping sound first.
    /// </summary>
    public void PlayLoopingSFX(string id)
    {
        if (loopSource == null || sfxLibrary == null) return;

        foreach (SFXEntry entry in sfxLibrary)
        {
            if (string.Equals(entry.id, id, System.StringComparison.OrdinalIgnoreCase))
            {
                if (entry.clip == null) return;
                loopSource.clip = entry.clip;
                loopSource.loop = true;
                loopSource.volume = loopVolume;
                loopSource.Play();
                return;
            }
        }

        Debug.LogWarning($"[AudioManager] Looping SFX id '{id}' not found in library.");
    }

    /// <summary>Stops the looping element sound.</summary>
    public void StopLoopingSFX()
    {
        if (loopSource != null && loopSource.isPlaying)
            loopSource.Stop();
    }

    // ── Element Sound Effects ─────────────────────────────────────────────────

    public void PlaySFX(string id)
    {
        if (sfxSource == null || sfxLibrary == null) return;

        foreach (SFXEntry entry in sfxLibrary)
        {
            if (string.Equals(entry.id, id, System.StringComparison.OrdinalIgnoreCase))
            {
                if (entry.clip != null)
                    sfxSource.PlayOneShot(entry.clip, entry.isElementSound ? elementVolume : sfxVolume);
                return;
            }
        }

        Debug.LogWarning($"[AudioManager] SFX id '{id}' not found in library.");
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    public void SetElementVolume(float volume)
    {
        elementVolume = Mathf.Clamp01(volume);
    }

    // ── Footsteps ─────────────────────────────────────────────────────────────

    /// <summary>Start looping the running sound. Called by PlayerController when movement begins.</summary>
    public void PlayRunSound()
    {
        if (footstepSource == null || runningClip == null) return;
        if (footstepSource.isPlaying) return;

        footstepSource.clip = runningClip;
        footstepSource.loop = true;
        footstepSource.volume = footstepVolume;
        footstepSource.Play();
    }

    /// <summary>Stop the running sound. Called by PlayerController when movement stops.</summary>
    public void StopRunSound()
    {
        if (footstepSource != null && footstepSource.isPlaying)
            footstepSource.Stop();
    }

    public void SetFootstepVolume(float volume)
    {
        footstepVolume = Mathf.Clamp01(volume);
        if (footstepSource != null && footstepSource.isPlaying)
            footstepSource.volume = footstepVolume;
    }
}

/// <summary>One entry in the SFX library — an ID string paired with a clip.</summary>
[System.Serializable]
public class SFXEntry
{
    [Tooltip("Unique string ID used to request this sound (e.g. 'fire', 'fire_extinguish', 'earthquake').")]
    public string id;
    public AudioClip clip;
    [Tooltip("Tick this for element sounds (fire, flood, etc.) so they use the Element Volume slider instead of SFX Volume.")]
    public bool isElementSound = false;
}
