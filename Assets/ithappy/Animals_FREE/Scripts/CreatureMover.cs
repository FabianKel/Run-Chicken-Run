using System;
using UnityEngine;
using UnityEngine.InputSystem; // 1. Agregamos el Namespace del nuevo sistema

namespace ithappy.Animals_FREE
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class CreatureMover : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField]
        private float m_WalkSpeed = 2f; // Ajustado un poco para probar
        [SerializeField]
        private float m_RunSpeed = 5f;
        [SerializeField, Range(0f, 1500f)]
        private float m_RotateSpeed = 720f; // Más rápido para respuesta ágil tipo Overcooked
        [SerializeField]
        private Space m_Space = Space.World; // Cambiado a World para movimiento top-down
        [SerializeField]
        private float m_JumpHeight = 5f;

        [Header("Animator")]
        [SerializeField]
        private string m_VerticalID = "Vert";
        [SerializeField]
        private string m_StateID = "State";
        [SerializeField]
        private LookWeight m_LookWeight = new(1f, 0.3f, 0.7f, 1f);

        private Transform m_Transform;
        private CharacterController m_Controller;
        private Animator m_Animator;
        private Camera m_MainCamera;

        private MovementHandler m_Movement;
        private AnimationHandler m_Animation;

        private Vector2 m_Axis;
        private Vector3 m_Target;
        private bool m_IsRun;
        private bool m_IsMoving;

        public Vector2 Axis => m_Axis;
        public Vector3 Target => m_Target;
        public bool IsRun => m_IsRun;

        private void OnValidate()
        {
            m_WalkSpeed = Mathf.Max(m_WalkSpeed, 0f);
            m_RunSpeed = Mathf.Max(m_RunSpeed, m_WalkSpeed);
            m_Movement?.SetStats(m_WalkSpeed, m_RunSpeed, m_RotateSpeed, m_JumpHeight, m_Space);
        }

        private void Awake()
        {
            m_Transform = transform;
            m_Controller = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
            m_MainCamera = Camera.main;

            m_Movement = new MovementHandler(m_Controller, m_Transform, m_WalkSpeed, m_RunSpeed, m_RotateSpeed, m_JumpHeight, m_Space);
            m_Animation = new AnimationHandler(m_Animator, m_VerticalID, m_StateID);
        }

        private void Update()
        {
            // 2. LEER INPUT DEL NUEVO SISTEMA AQUI
            // ReadInputSystem();

            // Lógica original de movimiento
            m_Movement.Move(Time.deltaTime, in m_Axis, in m_Target, m_IsRun, m_IsMoving, out var animAxis, out var isAir);
            m_Animation.Animate(in animAxis, m_IsRun ? 1f : 0f, Time.deltaTime);
        }

        // 3. Método nuevo para procesar la entrada
        private void ReadInputSystem()
        {
            Vector2 input = Vector2.zero;
            bool runPressed = false;

            // Opción A: Leer Teclado
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1;

                runPressed = Keyboard.current.shiftKey.isPressed;
            }

            // Opción B: Leer Gamepad (si hay uno conectado)
            if (Gamepad.current != null && input == Vector2.zero)
            {
                input = Gamepad.current.leftStick.ReadValue();
                runPressed = Gamepad.current.buttonSouth.isPressed; // Botón A/X
            }

            // Normalizar para evitar moverse más rápido en diagonal
            input = Vector2.ClampMagnitude(input, 1f);

            // 4. Calcular dirección relativa a la cámara (Clave para Overcooked/Top-Down)
            if (input.sqrMagnitude > 0.01f)
            {
                // Obtenemos hacia donde mira la cámara, pero ignoramos la altura (Y)
                Vector3 camForward = m_MainCamera.transform.forward;
                Vector3 camRight = m_MainCamera.transform.right;
                camForward.y = 0;
                camRight.y = 0;
                camForward.Normalize();
                camRight.Normalize();

                // Creamos el vector de dirección en el mundo real
                Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;

                // Actualizamos las variables internas
                m_Axis = input; // Input crudo para animación
                m_IsMoving = true;

                // Hacemos que el personaje mire hacia donde se mueve (Target para IK y rotación)
                m_Target = transform.position + moveDir * 5f;
            }
            else
            {
                m_Axis = Vector2.zero;
                m_IsMoving = false;
                // Si no nos movemos, el target se queda donde estaba o al frente
                m_Target = transform.position + transform.forward;
            }

            m_IsRun = runPressed;
        }

        private void OnAnimatorIK()
        {
            m_Animation.AnimateIK(in m_Target, m_LookWeight);
        }

        // Mantenemos este método por si otro script externo quiere forzar input
        public void SetInput(in Vector2 axis, in Vector3 target, in bool isRun, in bool isJump)
        {
            m_Axis = axis;
            m_Target = target;
            m_IsRun = isRun;

            if (m_Axis.sqrMagnitude < Mathf.Epsilon)
            {
                m_Axis = Vector2.zero;
                m_IsMoving = false;
            }
            else
            {
                m_Axis = Vector3.ClampMagnitude(m_Axis, 1f);
                m_IsMoving = true;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y > m_Controller.stepOffset)
            {
                m_Movement.SetSurface(hit.normal);
            }
        }

        [Serializable]
        private struct LookWeight
        {
            public float weight;
            public float body;
            public float head;
            public float eyes;

            public LookWeight(float weight, float body, float head, float eyes)
            {
                this.weight = weight;
                this.body = body;
                this.head = head;
                this.eyes = eyes;
            }
        }

        #region Handlers
        private class MovementHandler
        {
            private readonly CharacterController m_Controller;
            private readonly Transform m_Transform;

            private float m_WalkSpeed;
            private float m_RunSpeed;
            private float m_RotateSpeed;

            private Space m_Space;

            private readonly float m_Luft = 75f;

            private float m_TargetAngle;
            private bool m_IsRotating = false;

            private Vector3 m_Normal;
            private Vector3 m_GravityAcelleration = Physics.gravity;

            private float m_jumpTimer;
            private Vector3 m_LastForward;

            public MovementHandler(CharacterController controller, Transform transform, float walkSpeed, float runSpeed, float rotateSpeed, float jumpHeight, Space space)
            {
                m_Controller = controller;
                m_Transform = transform;

                m_WalkSpeed = walkSpeed;
                m_RunSpeed = runSpeed;
                m_RotateSpeed = rotateSpeed;

                m_Space = space;
            }

            public void SetStats(float walkSpeed, float runSpeed, float rotateSpeed, float jumpHeight, Space space)
            {
                m_WalkSpeed = walkSpeed;
                m_RunSpeed = runSpeed;
                m_RotateSpeed = rotateSpeed;

                m_Space = space;
            }

            public void SetSurface(in Vector3 normal)
            {
                m_Normal = normal;
            }


            // Modifica este método para que no use la cámara si el espacio es World
            private void ConvertMovement(in Vector2 axis, in Vector3 targetForward, out Vector3 movement)
            {
                if (m_Space == Space.World)
                {
                    // El axis ya es la dirección X y Z del mundo enviada por EnemyPatrol
                    movement = new Vector3(axis.x, 0, axis.y);
                }
                else
                {
                    Vector3 forward = Camera.main.transform.forward;
                    Vector3 right = Camera.main.transform.right;
                    forward.y = 0;
                    right.y = 0;
                    forward.Normalize();
                    right.Normalize();
                    movement = (axis.x * right + axis.y * forward);
                }

                movement = Vector3.ProjectOnPlane(movement, m_Normal);
            }

            // Modifica este para asegurar que el targetForward sea la dirección real del movimiento
            public void Move(float deltaTime, in Vector2 axis, in Vector3 target, bool isRun, bool isMoving, out Vector2 animAxis, out bool isAir)
            {
                // Calculamos el movimiento primero
                ConvertMovement(in axis, Vector3.zero, out var movement);

                // Si nos estamos moviendo, el frente debe ser la dirección a la que vamos
                if (movement.sqrMagnitude > 0.01f)
                {
                    m_LastForward = movement.normalized;
                }

                CaculateGravity(deltaTime, out isAir);
                Displace(deltaTime, in movement, isRun);

                // Usamos m_LastForward para que la rotación sea hacia el movimiento
                Turn(in m_LastForward, isMoving);
                UpdateRotation(deltaTime);

                GenAnimationAxis(in movement, out animAxis);
            }

            private void Displace(float deltaTime, in Vector3 movement, bool isRun)
            {
                Vector3 displacement = (isRun ? m_RunSpeed : m_WalkSpeed) * movement;
                displacement += m_GravityAcelleration;
                displacement *= deltaTime;

                m_Controller.Move(displacement);
            }

            private void CaculateGravity(float deltaTime, out bool isAir)
            {
                m_jumpTimer = Mathf.Max(m_jumpTimer - deltaTime, 0f);

                if (m_Controller.isGrounded)
                {
                    m_GravityAcelleration = Physics.gravity;
                    isAir = false;

                    return;
                }

                isAir = true;

                m_GravityAcelleration += Physics.gravity * deltaTime;
                return;
            }

            private void GenAnimationAxis(in Vector3 movement, out Vector2 animAxis)
            {
                if(m_Space == Space.Self)
                {
                    animAxis = new Vector2(Vector3.Dot(movement, m_Transform.right), Vector3.Dot(movement, m_Transform.forward));
                }
                else
                {
                    animAxis = new Vector2(Vector3.Dot(movement, Vector3.right), Vector3.Dot(movement, Vector3.forward));
                }
            }

            private void Turn(in Vector3 targetForward, bool isMoving)
            {
                var angle = Vector3.SignedAngle(m_Transform.forward, Vector3.ProjectOnPlane(targetForward, Vector3.up), Vector3.up);

                if (!m_IsRotating)
                {
                    if (!isMoving && Mathf.Abs(angle) < m_Luft)
                    {
                        m_IsRotating = false;
                        return;
                    }

                    m_IsRotating = true;
                }

                m_TargetAngle = angle;
            }

            private void UpdateRotation(float deltaTime)
            {
                if(!m_IsRotating)
                {
                    return;
                }

                var rotDelta = m_RotateSpeed * deltaTime;
                if (rotDelta + Mathf.PI * 2f + Mathf.Epsilon >= Mathf.Abs(m_TargetAngle))
                {
                    rotDelta = m_TargetAngle;
                    m_IsRotating = false;
                }
                else
                {
                    rotDelta *= Mathf.Sign(m_TargetAngle);
                }

                m_Transform.Rotate(Vector3.up, rotDelta);
            }
        }

        private class AnimationHandler
        {
            private readonly Animator m_Animator;
            private readonly string m_VerticalID;
            private readonly string m_StateID;

            private readonly float k_InputFlow = 8.5f;

            private float m_FlowState;
            private Vector2 m_FlowAxis;

            public AnimationHandler(Animator animator, string verticalID, string stateID)
            {
                m_Animator = animator;
                m_VerticalID = verticalID;
                m_StateID = stateID;
            }

            // Dentro de AnimationHandler en CreatureMover
            public void Animate(in Vector2 axis, float state, float deltaTime)
            {
                // Si la magnitud es casi 0, reseteamos instantáneo para evitar deslizamientos
                if (axis.sqrMagnitude < 0.01f)
                {
                    m_FlowAxis = Vector2.zero;
                }
                else
                {
                    // Aumentamos la velocidad de respuesta (de 8.5f a 25f o directo)
                    m_FlowAxis = axis;
                }

                m_Animator.SetFloat(m_VerticalID, m_FlowAxis.magnitude);
                m_Animator.SetFloat(m_StateID, Mathf.Clamp01(state));
            }

            public void AnimateIK(in Vector3 target, in LookWeight lookWeight)
            {
                m_Animator.SetLookAtPosition(target);
                m_Animator.SetLookAtWeight(lookWeight.weight, lookWeight.body, lookWeight.head, lookWeight.eyes);
            }
        }
        #endregion
    }
}