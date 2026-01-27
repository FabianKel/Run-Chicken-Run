using UnityEngine;

public class PushPhysics : MonoBehaviour
{
    public float pushForce = 5f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic)
        {
            return;
        }
        

        Vector3 pushDir = hit.gameObject.transform.position - transform.position;
        pushDir.y = 0;

        body.AddForce(pushDir.normalized * pushForce, ForceMode.Force);
        Debug.Log("Pushing object: " + hit.gameObject.name);
    }
}