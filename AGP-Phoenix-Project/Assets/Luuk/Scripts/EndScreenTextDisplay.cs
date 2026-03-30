using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndScreenTextDisplay : MonoBehaviour
{
    [System.Serializable]
    public class SequenceEntry
    {
        public CanvasGroup canvasGroup;
        public float displayDuration;
        public bool isPersistent;    // fades in and stays, no fade out
        public CanvasGroup[] companions;   // fade in/out together with main
    }

    public SequenceEntry[] entries;
    public float fadeDuration = 0.5f;

    private bool _skipped = false;

    void Start()
    {
        foreach (var e in entries)
        {
            e.canvasGroup.alpha = 0f;
            foreach (var c in e.companions)
                c.alpha = 0f;
        }

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

        for (int i = 0; i < entries.Length - 1; i++)
        {
            entries[i].canvasGroup.alpha = 0f;
            foreach (var c in entries[i].companions)
                c.alpha = 0f;
        }

        CanvasGroup last = entries[entries.Length - 1].canvasGroup;
        last.alpha = 1f;
        foreach (var c in entries[entries.Length - 1].companions)
            c.alpha = 1f;

        StartCoroutine(SkipToEnd(last));
    }

    IEnumerator SkipToEnd(CanvasGroup last)
    {
        var fadeOuts = new List<Coroutine>();
        fadeOuts.Add(StartCoroutine(Fade(last, 1f, 0f, fadeDuration)));
        foreach (var c in entries[entries.Length - 1].companions)
            fadeOuts.Add(StartCoroutine(Fade(c, 1f, 0f, fadeDuration)));
        foreach (var f in fadeOuts) yield return f;
    }

    IEnumerator PlaySequence()
    {
        foreach (var e in entries)
        {
            // Fade in main + companions simultaneously
            var fadeIns = new List<Coroutine>();
            fadeIns.Add(StartCoroutine(Fade(e.canvasGroup, 0f, 1f, fadeDuration)));
            foreach (var c in e.companions)
                fadeIns.Add(StartCoroutine(Fade(c, 0f, 1f, fadeDuration)));
            foreach (var f in fadeIns) yield return f;

            if (!e.isPersistent)
            {
                yield return new WaitForSeconds(e.displayDuration);

                // Fade out main + companions simultaneously
                var fadeOuts = new List<Coroutine>();
                fadeOuts.Add(StartCoroutine(Fade(e.canvasGroup, 1f, 0f, fadeDuration)));
                foreach (var c in e.companions)
                    fadeOuts.Add(StartCoroutine(Fade(c, 1f, 0f, fadeDuration)));
                foreach (var f in fadeOuts) yield return f;
            }
            // if persistent: fades in and sequence moves on, stays visible
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

        cg.alpha = to;
    }
}
