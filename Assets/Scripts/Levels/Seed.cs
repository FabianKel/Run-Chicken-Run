using UnityEngine;

public class Seed : MonoBehaviour
{
    private bool collected = false;

    [Header("Configuración de Audio")]
    public AudioClip[] collectSounds;
    [Range(0f, 1f)] public float volumen = 1f; // Control deslizante en el Inspector

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

                Vector3 posicionSonido = Camera.main != null ? Camera.main.transform.position : transform.position;

                AudioSource.PlayClipAtPoint(clipSeleccionado, posicionSonido, volumen);
            }
        }
    }
}