using UnityEngine;

public class Seed : MonoBehaviour
{
    private bool collected = false;

    [Header("Sonidos de Recolección")]
    public AudioClip[] collectSounds;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !collected)
        {
            collected = true;

            // Avisar al Manager
            if (LevelManager.Instance != null)
                LevelManager.Instance.SeedCollected();

            // Lógica Aleatoria
            ReproducirSonidoAleatorio();

            Destroy(gameObject);
        }
    }

    private void ReproducirSonidoAleatorio()
    {
        if (collectSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, collectSounds.Length);
            AudioClip clipSeleccionado = collectSounds[randomIndex];

            if (clipSeleccionado != null)
            {
                AudioSource.PlayClipAtPoint(clipSeleccionado, transform.position, 1.0f);
            }
        }
    }
}