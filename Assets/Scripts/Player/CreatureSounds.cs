using UnityEngine;

public class CreatureSounds : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip footstepClip;

    public void ReproducirPaso()
    {
        if (audioSource == null || footstepClip == null) return;

        audioSource.pitch = Random.Range(1.8f, 2.2f);
        audioSource.volume = Random.Range(0.3f, 0.5f);

        audioSource.PlayOneShot(footstepClip);
    }
}