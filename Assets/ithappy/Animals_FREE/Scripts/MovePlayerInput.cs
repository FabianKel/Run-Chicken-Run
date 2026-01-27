using UnityEngine;
using UnityEngine.InputSystem;

namespace ithappy.Animals_FREE
{
    [RequireComponent(typeof(CreatureMover))]
    public class MovePlayerInput : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField]
        private PlayerCamera m_Camera;


        private CreatureMover m_Mover;

        private Vector2 m_Axis;
        private bool m_IsRun;
        private bool m_IsJump;

        private Vector3 m_Target;
        private Vector2 m_MouseDelta;
        private float m_Scroll;

        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover>();
        }

        private void Update()
        {
            GatherInput();
            SetInput();
        }

        public void GatherInput()
        {
            // 1. REINICIAR VALORES
            m_Axis = Vector2.zero;
            m_IsRun = false;
            m_IsJump = false;
            m_MouseDelta = Vector2.zero;
            m_Scroll = 0f;

            // 2. LEER TECLADO
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) m_Axis.y += 1;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) m_Axis.y -= 1;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) m_Axis.x -= 1;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) m_Axis.x += 1;

                // Correr (Shift)
                //m_IsRun = Keyboard.current.shiftKey.isPressed;
                m_IsRun = true;

                // Saltar (Espacio)
                m_IsJump = Keyboard.current.spaceKey.isPressed;
            }

            // 3. LEER MOUSE (Para la cámara)
            if (Mouse.current != null)
            {
                // Leemos el movimiento del mouse (Delta)
                m_MouseDelta = Mouse.current.delta.ReadValue();

                // Leemos la rueda del mouse (Scroll)
                // Nota: El nuevo sistema devuelve valores muy altos (ej. 120), lo normalizamos un poco.
                m_Scroll = Mouse.current.scroll.ReadValue().y * 0.01f;
            }

            // 4. (OPCIONAL) LEER GAMEPAD
            if (Gamepad.current != null)
            {
                // Si el jugador toca el stick, sobreescribimos el teclado
                Vector2 stickInput = Gamepad.current.leftStick.ReadValue();
                if (stickInput.magnitude > 0.1f)
                {
                    m_Axis = stickInput;
                    m_IsRun = Gamepad.current.buttonSouth.isPressed; // Botón A/X para correr (ejemplo)
                }

                // Stick derecho para cámara
                Vector2 cameraStick = Gamepad.current.rightStick.ReadValue();
                if (cameraStick.magnitude > 0.1f)
                {
                    m_MouseDelta += cameraStick * 10f; // Multiplicador de sensibilidad para gamepad
                }
            }

            // Obtenemos el target de la cámara (hacia donde miramos)
            m_Target = (m_Camera == null) ? Vector3.zero : m_Camera.Target;
        }

        public void BindMover(CreatureMover mover)
        {
            m_Mover = mover;
        }

        public void SetInput()
        {
            if (m_Mover != null)
            {
                // Enviamos los datos al script de movimiento
                m_Mover.SetInput(in m_Axis, in m_Target, in m_IsRun, m_IsJump);
            }

            if (m_Camera != null)
            {
                // Enviamos los datos a la cámara
                m_Camera.SetInput(in m_MouseDelta, m_Scroll);
            }
        }
    }
}