using UnityEngine;

public class PushPhysics : MonoBehaviour
{
    public float pushForce = 5f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // Validar que el objeto tenga física y no sea cinemático
        if (body == null || body.isKinematic)
        {
            return;
        }

        // Calculamos la dirección (solo en el plano horizontal)
        Vector3 pushDir = hit.gameObject.transform.position - transform.position;
        pushDir.y = 0;

        // CORRECCIÓN: Multiplicamos por Time.deltaTime para que sea independiente de los FPS
        // Y dividimos por un valor constante o ajustamos la pushForce al alza
        //body.AddForce(pushDir.normalized * pushForce / Time.deltaTime, ForceMode.Force);

        // Alternativa sugerida si la anterior se siente rara:
        body.AddForce(pushDir.normalized * pushForce, ForceMode.Impulse);
    }
}