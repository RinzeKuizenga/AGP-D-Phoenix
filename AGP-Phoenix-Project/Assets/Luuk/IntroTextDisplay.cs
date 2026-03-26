using UnityEngine;
using System.Collections;

public class IntroTextDisplay : MonoBehaviour
{
    // Assign your 8 Text objects in the Inspector
    public CanvasGroup[] textObjects;

    public float fadeDuration = 0.5f;   // seconds to fade in / out
    public float displayDuration = 5f;   // seconds fully visible on screen

    void Start()
    {
        // Hide all texts at the start
        foreach (CanvasGroup cg in textObjects)
            cg.alpha = 0f;

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        foreach (CanvasGroup cg in textObjects)
        {
            // Fade in
            yield return StartCoroutine(Fade(cg, 0f, 1f, fadeDuration));

            // Hold on screen
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(Fade(cg, 1f, 0f, fadeDuration));
        }
    }

    IEnumerator Fade(CanvasGroup cg, float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        cg.alpha = to; // snap to final value
    }
}
