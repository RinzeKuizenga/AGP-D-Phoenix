using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void StopSFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.Stop();
    }
}
