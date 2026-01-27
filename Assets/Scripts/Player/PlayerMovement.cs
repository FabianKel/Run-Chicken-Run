using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;

    void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Keyboard.current != null)
        {
            float horizontalInput = 0.0f;
            float verticalInput = 0.0f;

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontalInput = 1.0f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontalInput = -1.0f;


            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                verticalInput = 1.0f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                verticalInput = -1.0f;

            movement = new Vector3(horizontalInput, 0.0f, verticalInput);
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
}