using UnityEngine;

namespace UnityGame.CharacterMovement
{
    [DisallowMultipleComponent]
    public sealed class PlayerLocomotionAnimator : MonoBehaviour
    {
        [SerializeField] private PlayerCharacterMotor motor;
        [SerializeField] private Animator animator;
        [SerializeField] private string horizontalParameter = "Hor";
        [SerializeField] private string verticalParameter = "Vert";
        [SerializeField] private string stateParameter = "State";
        [SerializeField] private string jumpParameter = "IsJump";
        [SerializeField, Min(0f)] private float damping = 0.12f;

        private int horizontalHash;
        private int verticalHash;
        private int stateHash;
        private int jumpHash;

        private void Awake()
        {
            if (motor == null)
            {
                motor = GetComponentInParent<PlayerCharacterMotor>();
            }

            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            horizontalHash = Animator.StringToHash(horizontalParameter);
            verticalHash = Animator.StringToHash(verticalParameter);
            stateHash = Animator.StringToHash(stateParameter);
            jumpHash = Animator.StringToHash(jumpParameter);

            if (animator != null)
            {
                animator.applyRootMotion = false;
            }
        }

        private void Update()
        {
            if (motor == null || animator == null)
            {
                return;
            }

            CharacterMotionState state = motor.MotionState;
            Vector3 actual = state.ActualVelocity;
            Vector3 local = transform.InverseTransformDirection(actual);
            CharacterMovementSettings movementSettings = motor.MovementSettings;
            float maxSpeed = movementSettings != null ? movementSettings.SprintSpeed : Mathf.Max(0.01f, actual.magnitude);
            float walkSpeed = movementSettings != null ? movementSettings.WalkSpeed : maxSpeed * 0.5f;
            float planarSpeed = Vector3.ProjectOnPlane(actual, Vector3.up).magnitude;

            float horizontal = Mathf.Clamp(local.x / Mathf.Max(0.01f, maxSpeed), -1f, 1f);
            float vertical = Mathf.Clamp(local.z / Mathf.Max(0.01f, maxSpeed), -1f, 1f);
            float moveState = Mathf.Clamp01(Mathf.InverseLerp(walkSpeed * 0.75f, maxSpeed, planarSpeed));

            animator.SetFloat(horizontalHash, horizontal, damping, Time.deltaTime);
            animator.SetFloat(verticalHash, vertical, damping, Time.deltaTime);
            animator.SetFloat(stateHash, moveState, damping, Time.deltaTime);
            animator.SetBool(jumpHash, !state.IsGrounded);
        }
    }
}
