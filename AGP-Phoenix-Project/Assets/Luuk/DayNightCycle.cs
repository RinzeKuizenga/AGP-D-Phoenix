using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using System.Collections;

public class DayNightCycle : MonoBehaviour
{
    [Header("Volume Reference")]
    public Volume volume;

    [Header("Day Cycle Skyboxes")]
    public Cubemap dawn;
    public Cubemap midday;
    public Cubemap sunset;
    public Cubemap night;

    [Header("Rainy Weather")]
    public Cubemap rainySkybox;
    public GameObject rainObject;
    [Range(0f, 1f)]
    [Tooltip("0 = never rains, 1 = always rains")]
    public float rainChance = 0.3f;
    [Range(0f, 1f)]
    [Tooltip("Separate chance for rain specifically at night")]
    public float nightRainChance = 0.5f;
    public float rainMinDuration = 60f;
    public float rainMaxDuration = 150f;

    [Header("Day Cycle Settings")]
    public float timePerPhase = 60f;
    [Tooltip("How long the full fade out + fade in takes in seconds")]
    public float fadeDuration = 0.8f;
    public bool autoStart = true;
    public bool loopCycle = true;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private HDRISky hdriSky;
    private Exposure exposure;
    private float originalExposure;
    private Cubemap[] dayCycle;
    private string[] phaseNames = { "Dawn", "Midday", "Sunset", "Night" };
    private int currentPhase = 0;
    private bool isRaining = false;
    private Coroutine cycleCoroutine;
    private Coroutine rainCoroutine;
    private Coroutine fadeCoroutine;

    void Start()
    {
        if (!InitializeVolume()) return;

        dayCycle = new Cubemap[] { dawn, midday, sunset, night };

        if (rainObject != null)
            rainObject.SetActive(false);

        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        // Wait for HDRP to fully initialize before capturing exposure
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        originalExposure = exposure.fixedExposure.value;
        Log($"Baseline exposure captured: {originalExposure} EV");

        hdriSky.hdriSky.Override(dayCycle[0]);

        if (autoStart)
            StartCycle();
    }

    // ─── Public Controls ───────────────────────────────────────────

    public void StartCycle()
    {
        if (cycleCoroutine != null)
            StopCoroutine(cycleCoroutine);

        cycleCoroutine = StartCoroutine(DayCycleRoutine());
    }

    public void StopCycle()
    {
        if (cycleCoroutine != null)
        {
            StopCoroutine(cycleCoroutine);
            cycleCoroutine = null;
        }

        StopRain();
        Log("Day cycle stopped.");
    }

    public void RestartCycle()
    {
        currentPhase = 0;
        StopRain();
        StartCycle();
    }

    public void JumpToPhase(int index)
    {
        if (isRaining) return;
        currentPhase = Mathf.Clamp(index, 0, dayCycle.Length - 1);
        FadeTo(dayCycle[currentPhase]);
    }

    public void TriggerRain(float duration = -1f)
    {
        if (rainySkybox == null)
        {
            Log("No rainy skybox assigned!", isWarning: true);
            return;
        }

        if (rainCoroutine != null)
            StopCoroutine(rainCoroutine);

        float d = duration > 0 ? duration : Random.Range(rainMinDuration, rainMaxDuration);
        rainCoroutine = StartCoroutine(RainRoutine(d));
    }

    public void StopRain()
    {
        if (rainCoroutine != null)
        {
            StopCoroutine(rainCoroutine);
            rainCoroutine = null;
        }

        if (isRaining)
        {
            isRaining = false;
            SetRainObject(false);
            FadeTo(dayCycle[currentPhase]);
            Log("Rain stopped early.");
        }
    }

    public bool IsRaining() => isRaining;
    public string GetCurrentPhaseName() => isRaining ? $"{phaseNames[currentPhase]} (Rainy)" : phaseNames[currentPhase];

    // ─── Coroutines ────────────────────────────────────────────────

    private IEnumerator DayCycleRoutine()
    {
        Log("Day cycle started.");

        do
        {
            for (int i = 0; i < dayCycle.Length; i++)
            {
                currentPhase = i;

                bool isNight = i == 3;
                float chance = isNight ? nightRainChance : rainChance;

                bool shouldRain = rainySkybox != null
                               && !isRaining
                               && Random.value < chance;

                if (shouldRain)
                {
                    float rainDur = Random.Range(rainMinDuration, rainMaxDuration);
                    rainDur = Mathf.Min(rainDur, timePerPhase);
                    TriggerRain(rainDur);
                }
                else if (!isRaining)
                {
                    FadeTo(dayCycle[i]);
                }

                Log($"Phase: {phaseNames[i]}{(shouldRain ? " + Rain" : "")} — {timePerPhase}s");

                yield return new WaitForSeconds(timePerPhase);
            }

        } while (loopCycle);

        Log("Day cycle complete.");
        cycleCoroutine = null;
    }

    private IEnumerator RainRoutine(float duration)
    {
        isRaining = true;
        SetRainObject(true);
        FadeTo(rainySkybox);
        Log($"Rain started for {duration:F0}s");

        yield return new WaitForSeconds(duration);

        isRaining = false;
        SetRainObject(false);
        FadeTo(dayCycle[currentPhase]);
        Log("Rain ended, restoring day phase skybox.");
        rainCoroutine = null;
    }

    private IEnumerator FadeRoutine(Cubemap next)
    {
        float halfFade = fadeDuration / 2f;
        float elapsed = 0f;
        float blackExposure = originalExposure + 1.5f;

        // ── Fade OUT ──
        while (elapsed < halfFade)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfFade);
            exposure.fixedExposure.Override(Mathf.Lerp(originalExposure, blackExposure, t));
            yield return null;
        }

        // ── Swap skybox while dimmed ──
        hdriSky.hdriSky.Override(next);
        Log($"Swapped skybox to: {next.name}");

        // ── Fade IN ──
        elapsed = 0f;

        while (elapsed < halfFade)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / halfFade);
            exposure.fixedExposure.Override(Mathf.Lerp(blackExposure, originalExposure, t));
            yield return null;
        }

        // Snap back to exact original value
        exposure.fixedExposure.Override(originalExposure);
        fadeCoroutine = null;
    }

    // ─── Core ──────────────────────────────────────────────────────

    private void FadeTo(Cubemap next)
    {
        if (next == null)
        {
            Log("FadeTo called with null cubemap, skipping.", isWarning: true);
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(next));
    }

    private void SetRainObject(bool active)
    {
        if (rainObject != null)
            rainObject.SetActive(active);
        else
            Log("No rain object assigned!", isWarning: true);
    }

    private bool InitializeVolume()
    {
        if (volume == null)
        {
            Debug.LogError("[DayNightCycle] No Volume assigned!");
            return false;
        }

        if (!volume.profile.TryGet<HDRISky>(out hdriSky))
        {
            Debug.LogError("[DayNightCycle] No HDRISky override found in the Volume Profile!");
            return false;
        }

        if (!volume.profile.TryGet<Exposure>(out exposure))
        {
            Debug.LogError("[DayNightCycle] No Exposure override found in the Volume Profile! Add one and set Mode to Fixed.");
            return false;
        }

        return true;
    }

    private void Log(string message, bool isWarning = false)
    {
        if (!showDebugLogs) return;
        if (isWarning) Debug.LogWarning($"[DayNightCycle] {message}");
        else Debug.Log($"[DayNightCycle] {message}");
    }

}
