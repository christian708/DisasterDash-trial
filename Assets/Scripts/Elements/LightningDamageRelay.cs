using UnityEngine;

/// <summary>
/// Put this on the DamageCollider child (alongside its Collider2D) when the
/// collider isn't on the same object as LightningHazard itself. Forwards
/// trigger events up to the parent's LightningHazard component.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LightningDamageRelay : MonoBehaviour
{
    private LightningHazard hazard;

    private void Awake()
    {
        hazard = GetComponentInParent<LightningHazard>();
    }

    private void OnTriggerEnter2D(Collider2D other) => hazard?.HandleTriggerEnter(other);
    private void OnTriggerStay2D(Collider2D other) => hazard?.HandleTriggerStay(other);
    private void OnTriggerExit2D(Collider2D other) => hazard?.HandleTriggerExit(other);
}