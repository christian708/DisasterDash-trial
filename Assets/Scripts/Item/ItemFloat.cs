using UnityEngine;

/// <summary>
/// Attach to any pickable item to make it float gently up and down.
/// Add this component to items in every scene for a subtle idle animation.
/// </summary>
public class ItemFloat : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.08f;  // how far up/down it moves
    [SerializeField] private float frequency = 1.2f;   // how fast it bobs
    [SerializeField] private float randomOffset = 0f;  // set automatically in Awake

    private Vector3 startPos;

    private void Awake()
    {
        startPos = transform.position;
        randomOffset = Random.Range(0f, Mathf.PI * 2f); // each item bobs out of sync
    }

    private void Update()
    {
        Vector3 pos = startPos;
        pos.y += Mathf.Sin((Time.time * frequency) + randomOffset) * amplitude;
        transform.position = pos;
    }
}
