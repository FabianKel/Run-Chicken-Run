using UnityEngine;

public class Level1Manager : MonoBehaviour
{
    public int seedsLeft;
    public bool phase1Completed = false;
    public GameObject door;

    void Start()
    {
        seedsLeft = GameObject.FindGameObjectsWithTag("Seed").Length;
        Debug.Log("Semillas totales: " + seedsLeft);
    }

    public void SeedCollected()
    {
        seedsLeft--;
        Debug.Log("Semillas restantes: " + seedsLeft);

        if (seedsLeft <= 0)
        {
            CompletePhase1();
        }
    }

    void CompletePhase1()
    {
        phase1Completed = true;
        Debug.Log("¡Fase 1 Completada!");

        if (door != null)
        {
            StartCoroutine(OpenDoorSmoothly());
        }
    }

    System.Collections.IEnumerator OpenDoorSmoothly()
    {
        float targetAngle = 100f;
        float currentAngle = 0f;
        float speed = 60f;

        Quaternion startRotation = door.transform.localRotation;

        while (currentAngle < targetAngle)
        {
            currentAngle += speed * Time.deltaTime;

            float angleToApply = Mathf.Min(currentAngle, targetAngle);

            door.transform.localRotation = startRotation * Quaternion.Euler(0, -angleToApply, 0);

            yield return null;
        }
    }
}