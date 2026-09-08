using UnityEngine;

/// <summary>
/// Trauma-based camera shake. Call AddTrauma(amount) to trigger/intensify shaking.
/// Trauma decays over time; shake amplitude scales with trauma^2 for a punchier feel
/// at high trauma and subtle shake at low trauma.
/// Attach this to the Camera itself (or a parent rig if you have one).
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [Header("Shake Settings")]
    [SerializeField] private float maxOffset = 0.3f;      // max positional offset in world units
    [SerializeField] private float maxRoll = 5f;           // max rotation in degrees (2D games can leave this small/0)
    [SerializeField] private float traumaDecaySpeed = 1f;  // trauma lost per second
    [SerializeField] private float noiseFrequency = 25f;   // how fast the Perlin noise evolves

    private float trauma = 0f;
    private Vector3 originalLocalPos;
    private float seedX, seedY, seedRot;

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        originalLocalPos = transform.localPosition;

        // Random seeds so multiple shakes don't look identical
        seedX = Random.Range(0f, 100f);
        seedY = Random.Range(0f, 100f);
        seedRot = Random.Range(0f, 100f);
    }

    private void Update()
    {
        if (trauma > 0f)
        {
            trauma = Mathf.Clamp01(trauma - traumaDecaySpeed * Time.deltaTime);

            float shakeAmount = trauma * trauma; // squared for punchier falloff

            float offsetX = maxOffset * shakeAmount * (Mathf.PerlinNoise(seedX, Time.time * noiseFrequency) * 2f - 1f);
            float offsetY = maxOffset * shakeAmount * (Mathf.PerlinNoise(seedY, Time.time * noiseFrequency) * 2f - 1f);
            float roll = maxRoll * shakeAmount * (Mathf.PerlinNoise(seedRot, Time.time * noiseFrequency) * 2f - 1f);

            transform.localPosition = originalLocalPos + new Vector3(offsetX, offsetY, 0f);
            transform.localRotation = Quaternion.Euler(0f, 0f, roll);
        }
        else
        {
            transform.localPosition = originalLocalPos;
            transform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>Adds trauma (0-1). Call this once when the earthquake phase starts,
    /// or repeatedly for a sustained/escalating shake.</summary>
    public void AddTrauma(float amount)
    {
        trauma = Mathf.Clamp01(trauma + amount);
    }

    /// <summary>Instantly stops shaking, e.g. when the earthquake phase ends early.</summary>
    public void StopShake()
    {
        trauma = 0f;
        transform.localPosition = originalLocalPos;
        transform.localRotation = Quaternion.identity;
    }

    public bool IsShaking => trauma > 0f;
}