using UnityEngine;
using ithappy.Animals_FREE;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float waitTime = 1.0f;
    public bool isRunning = true;
    public float stoppingDistance = 0.6f; // Margen para considerar que llegó

    private CreatureMover m_Mover;
    private int m_CurrentWaypointIndex = 0;
    private float m_WaitTimer;
    private bool m_IsWaiting;

    void Start()
    {
        m_Mover = GetComponent<CreatureMover>();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        if (m_IsWaiting)
        {
            m_WaitTimer -= Time.deltaTime;
            // Mirar hacia adelante mientras espera
            m_Mover.SetInput(Vector2.zero, transform.position + transform.forward, false, false);
            if (m_WaitTimer <= 0) m_IsWaiting = false;
            return;
        }

        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[m_CurrentWaypointIndex];
        Vector3 directionToPoint = target.position - transform.position;
        directionToPoint.y = 0;


        if (directionToPoint.magnitude < stoppingDistance)
        {
            m_IsWaiting = true;
            m_WaitTimer = waitTime;
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            return;
        }

        Vector3 normalizedDir = directionToPoint.normalized;


        Vector2 directInput = new Vector2(normalizedDir.x, normalizedDir.z);
        m_Mover.SetInput(directInput, target.position, isRunning, false);
    }

}