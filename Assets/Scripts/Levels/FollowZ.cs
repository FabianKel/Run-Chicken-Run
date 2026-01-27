using UnityEngine;

public class CameraFollowZ : MonoBehaviour
{
    public Transform player;

    [Header("Límites de Movimiento")]
    public float minZ = -10f;
    public float maxZ = 100f;

    private float offsetZ;

    void Start()
    {

        offsetZ = transform.position.z - player.position.z;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 newPos = transform.position;


        float targetZ = player.position.z + offsetZ;


        newPos.z = Mathf.Clamp(targetZ, minZ, maxZ);

        transform.position = newPos;
    }
}