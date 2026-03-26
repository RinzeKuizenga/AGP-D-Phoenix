using UnityEngine;

public class CameraShakeEvent : MonoBehaviour
{
    [Header("Override shake values per section (0 = use Shake defaults)")]
    public float lightMagnitude = 0f;
    public float lightDuration  = 0f;
    public float heavyMagnitude = 0f;
    public float heavyDuration = 0f;

    [SerializeField] private AudioClip smallSmash;
    [SerializeField] private AudioClip bigSmash;

    // ── Wire to OnSectionDamaged ──────────────────────────────────────────────
    public void ShakeLight(HullSection _)
    {
        Debug.Log("BABABABABABABAB");
        if (Shake.Instance == null) return;
 
        // Use override values if set, otherwise fall back to the singleton's defaults
        float mag = lightMagnitude > 0f ? lightMagnitude : Shake.Instance.magnitude * 0.5f;
        float dur = lightDuration  > 0f ? lightDuration  : Shake.Instance.duration  * 0.5f;

        //Debug.Log("BABABABABABABAB");
        AudioManager.Instance.PlaySFX(smallSmash, 0.2f);
        Shake.Instance.TriggerShake(dur, mag);
    }
 
    // ── Wire to OnSectionCritical / OnSectionDestroyed ────────────────────────
    public void ShakeHeavy(HullSection _)
    {
        if (Shake.Instance == null) return;
 
        float mag = heavyMagnitude > 0f ? heavyMagnitude : Shake.Instance.magnitude;
        float dur = heavyDuration  > 0f ? heavyDuration  : Shake.Instance.duration;

        AudioManager.Instance.PlaySFX(bigSmash, 0.2f);
        Shake.Instance.TriggerShake(dur, mag);
    }
}
