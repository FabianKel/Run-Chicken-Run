using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicalButton : MonoBehaviour
{
    public Transform piston;
    public float pressedHeight = 0.1f;
    public float unpressedHeight = 0.3f;
    public float speed = 10f;

    public UnityEvent OnButtonPressed;
    public UnityEvent OnButtonReleased;

    private List<GameObject> objectsOnButton = new List<GameObject>();
    private float targetY;

    void Start()
    {
        targetY = unpressedHeight;
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
                Debug.Log($"{other.name} entró. Objetos encima: {objectsOnButton.Count}");
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
                Debug.Log($"{other.name} salió. Objetos restantes: {objectsOnButton.Count}");
            }

            if (objectsOnButton.Count == 0)
            {
                targetY = unpressedHeight;
                OnButtonReleased.Invoke();
                Debug.Log("¡Botón liberado!");
            }
        }
    }
}