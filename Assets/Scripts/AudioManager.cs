using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Configuración de Fuentes")]
    public AudioSource backgroundMusic;
    public AudioSource winSound;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayWinSequence()
    {
        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
        }

        if (winSound != null)
        {
            winSound.Play();
        }
    }

    public void ResumeBackgroundMusic()
    {
        if (winSound != null) winSound.Stop();

        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }
}