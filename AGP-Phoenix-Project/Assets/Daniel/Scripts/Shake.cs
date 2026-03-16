using System;
using UnityEngine; 
using System.Collections; 
using Random = UnityEngine.Random;

public class Shake : MonoBehaviour
{
    public static Shake Instance { get; private set; }
    public Camera mainCamera;
    public float magnitude;
    public float duration;

    public void TriggerShake(float dur, float mag) 
        => StartCoroutine(ShakeCamera(dur, mag));
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Remove this if you don't need it to persist across scenes
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ShakeCamera(duration, magnitude));
        }
    }

    private IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            Vector3 offset = Random.insideUnitSphere * magnitude;
            offset.z = 0f;
            mainCamera.transform.localPosition = originalPosition + offset;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }         
        mainCamera.transform.localPosition = originalPosition;
    }
}