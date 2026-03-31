using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class BoatHullManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraOrbiter cameraOrbiter;
    
    [Header("Sections")]
    public List<HullSection> hullSections = new List<HullSection>();

    [Header("Sinking")]
    [Range(0f, 1f)]
    public float sinkThreshold = 0.5f;
    
    public float sinkSpeed = 0.5f;
    
    public float sinkTiltSpeed = 5f;
    
    public float fullySubmergedY = -10f;

    [Header("Flooding (slows the boat)")]
    [Range(0f, 0.5f)]
    public float speedPenaltyPerCriticalSection = 0.1f;

    [Header("Events")]
    public UnityEvent          OnBoatSinking;
    public UnityEvent<float>   OnOverallHealthChanged;  // 0–1
    public UnityEvent          OnBoatSunk;
    
    public bool  IsSinking { get; private set; }
    public bool  IsSunk    { get; private set; }
    
    public float OverallHealth
    {
        get
        {
            if (hullSections.Count == 0) return 1f;
            float sum = 0f;
            foreach (var s in hullSections) sum += s.HealthFraction;
            return sum / hullSections.Count;
        }
    }
    
    public float SpeedMultiplier
    {
        get
        {
            int criticalCount = 0;
            foreach (var s in hullSections)
                if (s.State == HullSection.DamageState.Critical ||
                    s.State == HullSection.DamageState.Destroyed)
                    criticalCount++;

            float penalty = criticalCount * speedPenaltyPerCriticalSection;
            return Mathf.Clamp01(1f - penalty);
        }
    }

    private bool _sinkEventFired;
    private bool _sunkEventFired;
    private float _sinkTiltAngle;

    void Start()
    {
        // Auto-discover sections if none were manually assigned
        if (hullSections.Count == 0)
            hullSections.AddRange(GetComponentsInChildren<HullSection>());

        if (hullSections.Count == 0)
            Debug.LogWarning("[BoatHullManager] No HullSections found! Add HullSection components to child objects.");

        // Subscribe to each section's events
        foreach (var section in hullSections)
        {
            section.OnSectionDamaged.AddListener(HandleSectionDamaged);
            section.OnSectionDestroyed.AddListener(HandleSectionDestroyed);
        }
    }

    void Update()
    {
        if (IsSinking && !IsSunk)
            UpdateSinking();
    }
    
    public float DamageSection(string sectionName, float damage)
    {
        HullSection section = FindSection(sectionName);
        if (section == null)
        {
            Debug.LogWarning($"[BoatHullManager] Section '{sectionName}' not found.");
            return 0f;
        }
        return section.ApplyDamage(damage);
    }
    
    public float DamageAtPoint(Vector3 worldPoint, float damage)
    {
        HullSection nearest = GetNearestSection(worldPoint);
        if (nearest == null) return 0f;

        Debug.Log($"[BoatHullManager] Hit resolved to section: {nearest.sectionName}");
        return nearest.ApplyDamage(damage);
    }
    
    public void RepairSection(string sectionName, float amount)
    {
        FindSection(sectionName)?.Repair(amount);
    }
    
    public void RepairAll(float amount)
    {
        foreach (var s in hullSections) s.Repair(amount);
    }
    
    public HullSection GetMostDamagedSection()
    {
        HullSection worst = null;
        float lowestFraction = float.MaxValue;
        foreach (var s in hullSections)
        {
            if (s.HealthFraction < lowestFraction)
            {
                lowestFraction = s.HealthFraction;
                worst = s;
            }
        }
        return worst;
    }
    
    private void HandleSectionDamaged(HullSection section)
    {
        OnOverallHealthChanged?.Invoke(OverallHealth);
        CheckSinkCondition();
    }

    private void HandleSectionDestroyed(HullSection section)
    {
        Debug.Log($"[BoatHullManager] Section DESTROYED: {section.sectionName}");
        OnOverallHealthChanged?.Invoke(OverallHealth);
        CheckSinkCondition();
    }

    private void CheckSinkCondition()
    {
        if (IsSinking) return;

        int destroyedCount = 0;
        foreach (var s in hullSections)
            if (s.State == HullSection.DamageState.Destroyed)
                destroyedCount++;

        float destroyedFraction = (float)destroyedCount / hullSections.Count;

        if (destroyedFraction >= sinkThreshold)
            StartSinking();
    }

    private void StartSinking()
    {
        IsSinking = true;
        if (!_sinkEventFired)
        {
            if (cameraOrbiter.IsInSideView)
            {
                cameraOrbiter.ToggleSideView();
            }
            _sinkEventFired = true;
            Debug.Log("[BoatHullManager] Boat is SINKING!");
            OnBoatSinking?.Invoke();
        }
    }

    private void UpdateSinking()
    {
        // Tilt towards the most damaged side
        HullSection worst = GetMostDamagedSection();
        Vector3 tiltAxis = Vector3.forward; // default: tilt forward/back

        if (worst != null)
        {
            // Tilt towards the damaged section's local position
            Vector3 localPos = transform.InverseTransformPoint(worst.transform.position);
            tiltAxis = new Vector3(localPos.z, 0f, -localPos.x).normalized;
        }

        _sinkTiltAngle += sinkTiltSpeed * Time.deltaTime;
        transform.Rotate(tiltAxis, sinkTiltSpeed * Time.deltaTime, Space.Self);

        // Sink downward
        transform.position += Vector3.down * sinkSpeed * Time.deltaTime;

        if (transform.position.y <= fullySubmergedY && !_sunkEventFired)
        {
            _sunkEventFired = true;
            IsSunk = true;
            Debug.Log("[BoatHullManager] Boat has SUNK.");
            OnBoatSunk?.Invoke();
        }
    }

    private HullSection FindSection(string name)
    {
        foreach (var s in hullSections)
            if (s.sectionName == name) return s;
        return null;
    }

    private HullSection GetNearestSection(Vector3 worldPoint)
    {
        HullSection nearest = null;
        float minDist = float.MaxValue;
        foreach (var s in hullSections)
        {
            float d = Vector3.Distance(worldPoint, s.transform.position);
            if (d < minDist) { minDist = d; nearest = s; }
        }
        return nearest;
    }

    void OnDrawGizmosSelected()
    {
        foreach (var s in hullSections)
        {
            if (s == null) continue;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, s.transform.position);
        }
    }
}