using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Animator titleScreen;

    private enum MenuState
    {
        Title,
        Select,
        Options
    }

    private MenuState currentState = MenuState.Title;

    [SerializeField] private AudioClip openSound;

    void Start()
    {
        Time.timeScale = 1f;
        titleScreen = GetComponent<Animator>(); 
    }

    private void Update()
    {
        if (UIManager.Instance.IsLocked)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBack();
        }
    }


    public void buttonPress()
    {
        if (UIManager.Instance.IsLocked || currentState != MenuState.Title)
            return;

        TriggerStart();
    }
    public void optionsPress()
    {
        if (UIManager.Instance.IsLocked || currentState != MenuState.Select)
            return;

        TriggerOptions();
    }

    public void newgamePress()
    {
        FadeManager.Instance.FadeToScene("RinzeScene");
    }

    public void quitPress()
    {
        FadeManager.Instance.FadeQuitGame();
    }


    private void TriggerStart()
    {
        UIManager.Instance.SetAnimating(true);
        currentState = MenuState.Select;

        titleScreen.SetTrigger("Start");
        AudioManager.Instance.PlaySFX(openSound);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void TriggerBackToTitle()
    {
        UIManager.Instance.SetAnimating(true);
        currentState = MenuState.Title;

        titleScreen.SetTrigger("Back");
        AudioManager.Instance.PlaySFX(openSound);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void TriggerOptions()
    {
        UIManager.Instance.SetAnimating(true);
        currentState = MenuState.Options;

        titleScreen.SetTrigger("Options");
        AudioManager.Instance.PlaySFX(openSound);
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void TriggerOptionsBack()
    {
        UIManager.Instance.SetAnimating(true);
        currentState = MenuState.Select;

        titleScreen.SetTrigger("OptionsBack");
        AudioManager.Instance.PlaySFX(openSound);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnMenuAnimationFinished()
    {
        UIManager.Instance.SetAnimating(false);
    }


    private void HandleBack()
    {
        switch (currentState)
        {
            case MenuState.Select:
                TriggerBackToTitle();
                break;

            case MenuState.Options:
                TriggerOptionsBack();
                break;
        }
    }
}
