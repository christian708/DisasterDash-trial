using UnityEngine;

public class CementDebrisSpawner : MonoBehaviour
{
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private BoxCollider2D crouchZone;        // drag CrouchZone here
    [SerializeField] private BoxCollider2D tableTopCollider;  // drag TableTopCollider here
    [SerializeField] private float spawnHeight = 4f;
    [SerializeField] private float xOffset = 0f;              // shift left/right from table center

    private PlayerController player;
    private bool hasDropped = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
    }

    private bool PlayerInCrouchZone()
    {
        if (player == null || crouchZone == null) return false;
        if (!player.IsCrouching) return false;

        float x = player.transform.position.x;
        Bounds b = crouchZone.bounds;
        return x >= b.min.x && x <= b.max.x;
    }

    private void Update()
    {
        if (hasDropped) return;
        if (debrisPrefab == null || tableTopCollider == null) return;
        if (!PlayerInCrouchZone()) return;

        hasDropped = true;

        Bounds top = tableTopCollider.bounds;
        Vector3 pos = new Vector3(top.center.x + xOffset, top.max.y + spawnHeight, 0f);

        GameObject piece = Instantiate(debrisPrefab, pos, Quaternion.identity);
        piece.GetComponent<CementDebris>().Drop(top.max.y);
    }
}