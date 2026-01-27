using UnityEngine;

public class ExitZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Verificamos si este objeto es el ExitTrigger del nivel actual
            if (LevelManager.Instance.IsCurrentExitTrigger(this.transform))
            {
                Debug.Log($"<color=cyan>ExitZone correcto activado: {gameObject.name}</color>");
                LevelManager.Instance.GoToNextLevel();
                gameObject.SetActive(false);
            }
            else
            {
                // Este mensaje saldrá para los otros niveles, confirmando que el filtro funciona
                Debug.Log($"Ignorando ExitZone de otro nivel: {gameObject.name}");
            }
        }
    }
}