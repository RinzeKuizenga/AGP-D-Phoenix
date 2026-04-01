using UnityEngine;

[RequireComponent(typeof(BoatHullManager))]
public class DebrisToHullDamage : MonoBehaviour
{
    [Header("Damage Values")]
    public float plankDamage = 8f;
    public float wreckDamage = 17f;

    [Header("Tags")]
    public string debrisTag = "Debris";
    public string wreckTag  = "Wreck";

    [Header("Cooldown")]
    [Tooltip("Seconds of invincibility after a hit to prevent multi-hit on same collision")]
    public float hitCooldown = 0.4f;

    private BoatHullManager _hullManager;
    private float _lastHitTime = -999f;


    void Awake()
    {
        _hullManager = GetComponent<BoatHullManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Time.time - _lastHitTime < hitCooldown) return;

        string tag = collision.gameObject.tag;
        if (tag != debrisTag && tag != wreckTag) return;

        _lastHitTime = Time.time;
        
        Vector3 hitPoint = collision.contacts[0].point;
        float damage = (tag == wreckTag) ? wreckDamage : plankDamage;

        float dealt = _hullManager.DamageAtPoint(hitPoint, damage);
        Debug.Log($"[DebrisToHullDamage] {collision.gameObject.name} hit at {hitPoint} — {dealt:F1} damage dealt");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (Time.time - _lastHitTime < hitCooldown) return;

        string tag = other.tag;
        if (tag != debrisTag && tag != wreckTag) return;

        _lastHitTime = Time.time;

        float damage = (tag == wreckTag) ? wreckDamage : plankDamage;
        _hullManager.DamageAtPoint(other.ClosestPoint(transform.position), damage);
    }
}