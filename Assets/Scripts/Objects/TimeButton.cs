using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class TimeButton : MonoBehaviour
{
    [Header("Visuales")]
    public Transform piston;
    public float pressedHeight = 0.1f;
    public float unpressedHeight = 0.3f;
    public float speed = 10f;

    [Header("Temporizador")]
    public float waitTime = 3.0f;
    public AudioClip timerSound;

    [Header("Eventos")]
    public UnityEvent OnButtonPressed;
    public UnityEvent OnButtonReleased;

    private List<GameObject> objectsOnButton = new List<GameObject>();
    private float targetY;
    private Coroutine releaseCoroutine;
    private AudioSource audioSource;

    void Start()
    {
        targetY = unpressedHeight;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector3 newPos = piston.localPosition;
        newPos.y = Mathf.Lerp(newPos.y, targetY, Time.deltaTime * speed);
        piston.localPosition = newPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Crate"))
        {
            if (!objectsOnButton.Contains(other.gameObject))
            {
                objectsOnButton.Add(other.gameObject);
            }

            if (releaseCoroutine != null)
            {
                StopCoroutine(releaseCoroutine);

                if (audioSource != null) audioSource.Stop();

                releaseCoroutine = null;
                Debug.Log("Temporizador cancelado: alguien volvió a pisar el botón.");
            }

            if (objectsOnButton.Count == 1)
            {
                targetY = pressedHeight;
                OnButtonPressed.Invoke();
                Debug.Log("¡Botón activado!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Crate"))
        {
            if (objectsOnButton.Contains(other.gameObject))
            {
                objectsOnButton.Remove(other.gameObject);
            }

            if (objectsOnButton.Count == 0)
            {
                if (releaseCoroutine != null)
                {
                    StopCoroutine(releaseCoroutine);
                    if (audioSource != null) audioSource.Stop();
                }

                releaseCoroutine = StartCoroutine(WaitAndRelease());
            }
        }
    }

    IEnumerator WaitAndRelease()
    {
        Debug.Log($"Iniciando cuenta atrás de {waitTime} segundos...");

        if (audioSource != null && timerSound != null)
        {
            audioSource.clip = timerSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        yield return new WaitForSeconds(waitTime);

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        targetY = unpressedHeight;
        OnButtonReleased.Invoke();
        Debug.Log("¡Botón liberado por tiempo!");

        releaseCoroutine = null;
    }
}