using UnityEngine;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    public Transform cameraTransform; // Arrastra tu cámara principal aquí
    public Vector3 cameraOffset;      // La posición relativa a la nueva zona
    public float transitionDuration = 1.5f;

    [Header("Zonas")]
    public GameObject nextArea;       // El objeto que contiene la Fase 2
    public Transform playerSpawnPoint; // Dónde aparecerá la gallina en la zona nueva

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(TransitionRoutine(other.gameObject));
        }
    }

    IEnumerator TransitionRoutine(GameObject player)
    {
        // 1. Bloquear movimiento del jugador para que no camine durante la animación
        player.GetComponent<CharacterController>().enabled = false;

        // 2. Activar la nueva zona (Optimización)
        if (nextArea != null) nextArea.SetActive(true);

        // 3. Calcular destino de la cámara
        Vector3 startCamPos = cameraTransform.position;
        Vector3 endCamPos = playerSpawnPoint.position + cameraOffset;

        float elapsed = 0;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / transitionDuration;

            // Usamos una curva suave (SmoothStep)
            cameraTransform.position = Vector3.Lerp(startCamPos, endCamPos, Mathf.SmoothStep(0, 1, percent));
            yield return null;
        }

        // 4. Reposicionar al jugador y devolverle el control
        player.transform.position = playerSpawnPoint.position;
        player.GetComponent<CharacterController>().enabled = true;

        // 5. Desactivar la zona anterior para optimizar (Opcional)
        // transform.parent.gameObject.SetActive(false); 
    }
}