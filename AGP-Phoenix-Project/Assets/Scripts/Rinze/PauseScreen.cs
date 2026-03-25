using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] private bool isPaused = false;
    [SerializeField] private AudioClip openSound;

    public Animator optionsAnim;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Canvas canvas;
    public void Update()
    {
        if (UIManager.Instance.IsLocked)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            PauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        UIManager.Instance.SetAnimating(true);
        AudioManager.Instance.PlaySFX(openSound, 0.50f);

        isPaused = true;

        Time.timeScale = 0f;

        FadeManager.Instance.SetDarken(true);
        optionsAnim.SetTrigger("Options");
        audioSource.volume = 0.040f;
    }

    public void ResumeGame()
    {
        UIManager.Instance.SetAnimating(true);

        isPaused = false;

        Time.timeScale = 1f; 

        FadeManager.Instance.SetDarken(false);
        optionsAnim.SetTrigger("OptionsBack");
        audioSource.volume = 0.132f;
    }

    public void TitleScreenPress()
    {
        canvas.sortingOrder = 7; 
        FadeManager.Instance.FadeToScene("RinzeScene 1");
    }

    public void OnPauseAnimationFinished()
    {
        UIManager.Instance.SetAnimating(false);
    }
}
