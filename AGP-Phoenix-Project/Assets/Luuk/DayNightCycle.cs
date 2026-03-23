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
    [Tooltip("Min and max duration of a rain event in seconds")]
    public float rainMinDuration = 60f;
    public float rainMaxDuration = 150f;

    [Header("Day Cycle Settings")]
    [Tooltip("How long each part of day lasts in seconds (default 150 = 2.5 min)")]
    public float timePerPhase = 60f;
    public bool autoStart = true;
    public bool loopCycle = true;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private HDRISky hdriSky;
    private Cubemap[] dayCycle;
    private string[] phaseNames = { "Dawn", "Midday", "Sunset", "Night" };
    private int currentPhase = 0;
    private bool isRaining = false;
    private Coroutine cycleCoroutine;
    private Coroutine rainCoroutine;

    void Start()
    {
        if (!InitializeVolume()) return;

        dayCycle = new Cubemap[] { dawn, midday, sunset, night };

        // Make sure rain object starts disabled
        if (rainObject != null)
            rainObject.SetActive(false);

        if (autoStart)
            StartCycle();
    }

    // ??? Public Controls ???????????????????????????????????????????

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
        ApplySkybox(currentPhase);
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
            ApplySkybox(currentPhase);
            Log("Rain stopped early.");
        }
    }

    public bool IsRaining() => isRaining;
    public string GetCurrentPhaseName() => isRaining ? $"{phaseNames[currentPhase]} (Rainy)" : phaseNames[currentPhase];

    // ??? Coroutines ????????????????????????????????????????????????

    private IEnumerator DayCycleRoutine()
    {
        Log("Day cycle started.");

        do
        {
            for (int i = 0; i < dayCycle.Length; i++)
            {
                currentPhase = i;

                bool isNight = i == 6;
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
                    ApplySkybox(i);
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
        hdriSky.hdriSky.Override(rainySkybox);
        Log($"Rain started for {duration:F0}s");

        yield return new WaitForSeconds(duration);

        isRaining = false;
        SetRainObject(false);
        ApplySkybox(currentPhase);
        Log("Rain ended, restoring day phase skybox.");
        rainCoroutine = null;
    }

    // ??? Core ??????????????????????????????????????????????????????

    private void SetRainObject(bool active)
    {
        if (rainObject != null)
            rainObject.SetActive(active);
        else
            Log("No rain object assigned!", isWarning: true);
    }

    private void ApplySkybox(int index)
    {
        Cubemap target = dayCycle[index];

        if (target == null)
        {
            Log($"No cubemap assigned for '{phaseNames[index]}', skipping.", isWarning: true);
            return;
        }

        hdriSky.hdriSky.Override(target);
        Log($"Skybox set to: {target.name}");
    }

    private bool InitializeVolume()
    {
        if (volume == null)
        {
            Debug.LogError("[HDRISkyboxChanger] No Volume assigned!");
            return false;
        }

        if (!volume.profile.TryGet<HDRISky>(out hdriSky))
        {
            Debug.LogError("[HDRISkyboxChanger] No HDRISky override found in the Volume Profile!");
            return false;
        }

        return true;
    }

    private void Log(string message, bool isWarning = false)
    {
        if (!showDebugLogs) return;
        if (isWarning) Debug.LogWarning($"[HDRISkyboxChanger] {message}");
        else Debug.Log($"[HDRISkyboxChanger] {message}");
    }

}
