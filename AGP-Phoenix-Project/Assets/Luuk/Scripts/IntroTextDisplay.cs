using UnityEngine;
using System.Collections;

public class IntroTextDisplay : MonoBehaviour
{
    public CanvasGroup[] textObjects;
    public float fadeDuration = 0.5f;
    public float displayDuration = 5f;
    private bool _skipped = false;

    void Start()
    {
        foreach (CanvasGroup cg in textObjects)
            cg.alpha = 0f;
        StartCoroutine(PlaySequence());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            Skip();
    }

    public void Skip()
    {
        if (_skipped) return;
        _skipped = true;
        StopAllCoroutines();

        foreach (CanvasGroup cg in textObjects)
            cg.alpha = 0f;

        for (int i = 0; i < textObjects.Length - 1; i++)
            textObjects[i].alpha = 0f;

        CanvasGroup last = textObjects[textObjects.Length - 1];
        last.alpha = 1f;
        StartCoroutine(FadeAndLoad(last));
    }

    IEnumerator FadeAndLoad(CanvasGroup last)
    {
        yield return StartCoroutine(Fade(last, 1f, 0f, fadeDuration));
        FadeManager.Instance.FadeToScene("Crew selecting");
    }

    IEnumerator PlaySequence()
    {
        foreach (CanvasGroup cg in textObjects)
        {
            yield return StartCoroutine(Fade(cg, 0f, 1f, fadeDuration));
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(Fade(cg, 1f, 0f, fadeDuration));
        }

        FadeManager.Instance.FadeToScene("Crew selecting");
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
        cg.alpha = to;
    }
}
