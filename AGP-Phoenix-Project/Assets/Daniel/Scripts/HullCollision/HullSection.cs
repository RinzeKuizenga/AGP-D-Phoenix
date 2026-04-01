using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attach one of these to each named section of the boat hull
/// (e.g. BowSection, StarboardSection, SternSection, etc.)
/// 
/// Each section has its own HP, damage states, and visual responses.
/// </summary>
public class HullSection : MonoBehaviour
{
    // ── Identity ──────────────────────────────────────────────────────────────

    [Header("Identity")]
    public string sectionName = "Hull Section";

    [Tooltip("Multiplier applied to incoming damage (e.g. 1.5 = weak spot, 0.5 = armoured)")]
    public float damageMultiplier = 1f;

    // ── Health ────────────────────────────────────────────────────────────────

    [Header("Health")]
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;

    // ── Damage State Thresholds (% of maxHealth) ──────────────────────────────

    [Header("Damage Thresholds  (0–1 fraction of max HP)")]
    [Tooltip("Below this health fraction the section is 'Damaged'")]
    public float damagedThreshold  = 0.6f;
    [Tooltip("Below this health fraction the section is 'Critical'")]
    public float criticalThreshold = 0.3f;

    // ── Visuals ───────────────────────────────────────────────────────────────

    [Header("Visuals")]
    
    public Renderer[] sectionRenderers;
    public Material damagedMaterial;
    public Material criticalMaterial;
    
    public GameObject damageDecal;
    
    public GameObject criticalDecal;

    [Header("Effects")]
    public ParticleSystem leakParticles;
    
    public ParticleSystem floodParticles;
    
    public ParticleSystem impactParticles;

    [Header("Events")]
    public UnityEvent<HullSection>       OnSectionDamaged;   // any hit
    public UnityEvent<HullSection>       OnSectionCritical;  // crossed critical threshold
    public UnityEvent<HullSection>       OnSectionDestroyed; // reached 0 HP

    public enum DamageState { Intact, Damaged, Critical, Destroyed }
    public DamageState State { get; private set; } = DamageState.Intact;

    private Material[] _originalMaterials;
    private bool _criticalEventFired = false;
    private bool _destroyedEventFired = false;
    

    void Awake()
    {
        currentHealth = maxHealth;

        // Cache original materials so we can restore them if needed
        if (sectionRenderers != null && sectionRenderers.Length > 0)
        {
            _originalMaterials = sectionRenderers[0].materials;
        }

        // Make sure decals start hidden
        if (damageDecal)   damageDecal.SetActive(false);
        if (criticalDecal) criticalDecal.SetActive(false);
    }

    /// <summary>Apply damage to this section. Returns actual damage dealt.</summary>
    public float ApplyDamage(float rawDamage)
    {
        if (State == DamageState.Destroyed) return 0f;

        float actual = rawDamage * damageMultiplier;
        currentHealth = Mathf.Max(0f, currentHealth - actual);

        PlayImpactEffect();
        UpdateState();
        OnSectionDamaged?.Invoke(this);

        return actual;
    }

    /// <summary>Repair this section by a given amount.</summary>
    public void Repair(float amount)
    {
        if (State == DamageState.Destroyed) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        _criticalEventFired  = false; // allow re-firing if damaged again
        _destroyedEventFired = false;
        UpdateState();
    }

    /// <summary>Health as a 0–1 fraction.</summary>
    public float HealthFraction => currentHealth / maxHealth;

    private void UpdateState()
    {
        DamageState newState;

        if (currentHealth <= 0f)
            newState = DamageState.Destroyed;
        else if (HealthFraction <= criticalThreshold)
            newState = DamageState.Critical;
        else if (HealthFraction <= damagedThreshold)
            newState = DamageState.Damaged;
        else
            newState = DamageState.Intact;

        if (newState == State) return; // nothing changed

        State = newState;
        ApplyVisuals();

        switch (State)
        {
            case DamageState.Critical:
                if (!_criticalEventFired)
                {
                    _criticalEventFired = true;
                    OnSectionCritical?.Invoke(this);
                }
                break;

            case DamageState.Destroyed:
                if (!_destroyedEventFired)
                {
                    _destroyedEventFired = true;
                    OnSectionDestroyed?.Invoke(this);
                }
                break;
        }
    }

    private void ApplyVisuals()
    {
        switch (State)
        {
            case DamageState.Intact:
                SetMaterial(_originalMaterials?[0]);
                SetDecal(false, false);
                StopParticles(leakParticles);
                StopParticles(floodParticles);
                break;

            case DamageState.Damaged:
                SetMaterial(damagedMaterial);
                SetDecal(true, false);
                PlayParticles(leakParticles);
                StopParticles(floodParticles);
                break;

            case DamageState.Critical:
                SetMaterial(criticalMaterial);
                SetDecal(true, true);
                StopParticles(leakParticles);
                PlayParticles(floodParticles);
                break;

            case DamageState.Destroyed:
                SetMaterial(criticalMaterial);
                SetDecal(true, true);
                StopParticles(leakParticles);
                PlayParticles(floodParticles);
                break;
        }
    }

    private void SetMaterial(Material mat)
    {
        if (mat == null || sectionRenderers == null) return;
        foreach (var r in sectionRenderers)
            if (r) r.material = mat;
    }

    private void SetDecal(bool showDamage, bool showCritical)
    {
        if (damageDecal)   damageDecal.SetActive(showDamage);
        if (criticalDecal) criticalDecal.SetActive(showCritical);
    }

    private void PlayParticles(ParticleSystem ps)
    {
        if (ps && !ps.isPlaying) ps.Play();
    }

    private void StopParticles(ParticleSystem ps)
    {
        if (ps && ps.isPlaying) ps.Stop();
    }

    private void PlayImpactEffect()
    {
        if (impactParticles) impactParticles.Play();
    }
    
    void OnDrawGizmosSelected()
    {
        // Show section health as a coloured sphere in the editor
        Color c = State switch
        {
            DamageState.Intact    => Color.green,
            DamageState.Damaged   => Color.yellow,
            DamageState.Critical  => Color.red,
            DamageState.Destroyed => Color.black,
            _                     => Color.white
        };
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}