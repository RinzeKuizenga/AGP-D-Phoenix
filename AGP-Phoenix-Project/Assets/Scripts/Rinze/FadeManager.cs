using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [SerializeField] private Animator fadeAnimator;

    private Action onFadeComplete;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        fadeAnimator = GetComponent<Animator>();    
    }

    public void FadeToScene(string sceneName)
    {
        StartFade(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    public void FadeQuitGame()
    {
        StartFade(() =>
        {
            Application.Quit();
        });
    }

    private void StartFade(Action afterFadeAction)
    {
        UIManager.Instance.SetAnimating(true);

        onFadeComplete = afterFadeAction;

        fadeAnimator.SetTrigger("Fade");
    }

    public void SetDarken(bool state)
    {
        if (state)
            fadeAnimator.SetTrigger("Darken");
        else
            fadeAnimator.SetTrigger("Lighten");
    }

    public void OnFadeAnimationFinished()
    {
        onFadeComplete?.Invoke();
        onFadeComplete = null;

        UIManager.Instance.SetAnimating(false);
    }
}