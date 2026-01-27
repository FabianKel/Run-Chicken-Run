using UnityEngine;
using static GameEnums;

public class DoorController : MonoBehaviour
{
    [Header("Configuración")]
    public DoorDirection direction = DoorDirection.Arriba;
    public ChannelColor doorColor = ChannelColor.Rojo;

    [Header("Movimiento")]
    public float moveDistance = 3.0f;
    public float speed = 5.0f;

    private Vector3 initialPos;
    private Vector3 targetPos;
    private bool isOpen = false;

    void Start()
    {
        // Guardamos la posición original (cerrada)
        initialPos = transform.localPosition;
    }

    void Update()
    {
        // Definimos a dónde queremos ir dependiendo de si está abierta o cerrada
        Vector3 destination = isOpen ? GetTargetPosition() : initialPos;

        // Movemos la puerta suavemente hacia el destino
        transform.localPosition = Vector3.Lerp(transform.localPosition, destination, Time.deltaTime * speed);
    }

    // Calcula la posición final basada en el Enum de dirección
    private Vector3 GetTargetPosition()
    {
        Vector3 dirVector = Vector3.zero;

        switch (direction)
        {
            case DoorDirection.Arriba: dirVector = Vector3.up; break;
            case DoorDirection.Abajo: dirVector = Vector3.down; break;
            case DoorDirection.Izquierda: dirVector = Vector3.left; break;
            case DoorDirection.Derecha: dirVector = Vector3.right; break;
        }

        return initialPos + (dirVector * moveDistance);
    }

    // --- MÉTODOS PÚBLICOS PARA EL BOTÓN ---

    public void OpenDoor()
    {
        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
    }

    // Si quieres alternar el estado
    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}