using UnityEngine;

namespace UnityGame.CharacterMovement
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerCharacterMotor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterMovementSettings settings;
        [SerializeField] private MonoBehaviour inputSourceBehaviour;
        [SerializeField] private Transform orientation;

        [Header("Debug")]
        [SerializeField] private bool drawDebugGizmos = true;

        private CharacterController controller;
        private readonly GroundProbe groundProbe = new GroundProbe();
        private ICharacterInputSource inputSource;
        private ICharacterInputSource configuredInputSource;
        private Vector3 planarVelocity;
        private Vector3 slideVelocity;
        private Vector3 desiredVelocity;
        private Vector3 actualVelocity;
        private float verticalVelocity;
        private float coyoteTimer;
        private float jumpBufferTimer;
        private float timeSinceJump = 1000f;
        private bool wasStableGrounded;
        private bool isJumping;
        private bool missingSettingsReported;
        private bool invalidInputReported;
        private CharacterGroundState groundState = CharacterGroundState.Airborne;
        private CollisionFlags collisionFlags;

        public CharacterMotionState MotionState { get; private set; } = CharacterMotionState.Empty;

        public CharacterMovementSettings MovementSettings
        {
            get { return settings; }
        }

        public CharacterGroundState GroundState
        {
            get { return groundState; }
        }

        public Vector3 ActualVelocity
        {
            get { return actualVelocity; }
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            ResolveInputSource();
            ResolveOrientation();
        }

        private void Update()
        {
            if (Time.deltaTime <= 0f || !CanSimulate())
            {
                return;
            }

            Simulate(inputSource.ReadInput(), Time.deltaTime);
        }

        public void Configure(
            CharacterMovementSettings movementSettings,
            ICharacterInputSource source,
            Transform orientationOverride)
        {
            settings = movementSettings;
            configuredInputSource = source;
            inputSource = source;
            orientation = orientationOverride;
            missingSettingsReported = false;
            invalidInputReported = false;
        }

        public void SetOrientation(Transform orientationOverride)
        {
            orientation = orientationOverride;
        }

        public void Simulate(CharacterInputFrame input, float deltaTime)
        {
            if (deltaTime <= 0f || !CanSimulate())
            {
                return;
            }

            timeSinceJump += deltaTime;
            UpdateJumpBuffer(input, deltaTime);

            float probeDistance = Mathf.Max(settings.ProbeDistance, settings.GroundSnapDistance);
            groundState = groundProbe.Probe(controller, transform, settings, probeDistance);
            bool snappedToStableGround = CanSnapToGround(groundState);
            bool stableGrounded = groundState.IsStable && (groundState.Distance <= settings.ProbeDistance || snappedToStableGround);

            if (stableGrounded)
            {
                coyoteTimer = settings.CoyoteTime;
            }
            else
            {
                coyoteTimer = Mathf.Max(0f, coyoteTimer - deltaTime);
            }

            bool jumpedThisFrame = TryConsumeJump(stableGrounded, deltaTime);
            if (jumpedThisFrame)
            {
                stableGrounded = false;
                groundState = CharacterGroundState.Airborne;
            }

            UpdateStepOffset(stableGrounded);
            UpdatePlanarVelocity(input, stableGrounded, deltaTime);
            UpdateVerticalVelocity(input, stableGrounded, deltaTime);
            RotateTowardMotion(deltaTime);

            Vector3 previousPosition = transform.position;
            Vector3 totalVelocity = planarVelocity + slideVelocity + Vector3.up * verticalVelocity;
            collisionFlags = controller.Move(totalVelocity * deltaTime);
            actualVelocity = (transform.position - previousPosition) / deltaTime;

            if ((collisionFlags & CollisionFlags.Above) != 0 && verticalVelocity > 0f)
            {
                verticalVelocity = 0f;
            }

            if ((collisionFlags & CollisionFlags.Below) != 0 && verticalVelocity < 0f)
            {
                verticalVelocity = settings.GroundStickVelocity;
                isJumping = false;
            }

            wasStableGrounded = stableGrounded || ((collisionFlags & CollisionFlags.Below) != 0 && !groundState.IsSteep);

            MotionState = new CharacterMotionState(
                desiredVelocity,
                planarVelocity,
                slideVelocity,
                actualVelocity,
                verticalVelocity,
                groundState,
                collisionFlags,
                isJumping);
        }

        private bool CanSimulate()
        {
            if (controller == null)
            {
                controller = GetComponent<CharacterController>();
            }

            if (settings == null)
            {
                if (!missingSettingsReported)
                {
                    Debug.LogError("PlayerCharacterMotor requires CharacterMovementSettings.", this);
                    missingSettingsReported = true;
                }

                return false;
            }

            return ResolveInputSource();
        }

        private bool ResolveInputSource()
        {
            if (configuredInputSource != null)
            {
                inputSource = configuredInputSource;
                return true;
            }

            if (inputSourceBehaviour == null)
            {
                MonoBehaviour[] behaviours = GetComponents<MonoBehaviour>();
                for (int i = 0; i < behaviours.Length; i++)
                {
                    if (behaviours[i] is ICharacterInputSource source)
                    {
                        inputSourceBehaviour = behaviours[i];
                        inputSource = source;
                        return true;
                    }
                }
            }

            if (inputSourceBehaviour is ICharacterInputSource validSource)
            {
                inputSource = validSource;
                return true;
            }

            if (inputSourceBehaviour != null && !invalidInputReported)
            {
                Debug.LogError("Assigned input source does not implement ICharacterInputSource.", this);
                invalidInputReported = true;
                enabled = false;
            }

            return false;
        }

        private void ResolveOrientation()
        {
            if (orientation == null && Camera.main != null)
            {
                orientation = Camera.main.transform;
            }
        }

        private void UpdateJumpBuffer(CharacterInputFrame input, float deltaTime)
        {
            if (input.JumpPressed)
            {
                jumpBufferTimer = settings.JumpBufferTime;
            }
            else
            {
                jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - deltaTime);
            }
        }

        private bool TryConsumeJump(bool stableGrounded, float deltaTime)
        {
            bool hasBufferedJump = jumpBufferTimer > 0f;
            bool canUseCoyote = coyoteTimer > 0f && !groundState.IsSteep;
            if (!hasBufferedJump || !canUseCoyote)
            {
                return false;
            }

            verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(settings.Gravity) * settings.JumpHeight);
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
            timeSinceJump = 0f;
            isJumping = true;
            return true;
        }

        private bool CanSnapToGround(CharacterGroundState ground)
        {
            return wasStableGrounded
                   && !isJumping
                   && timeSinceJump >= settings.PostJumpSnapGrace
                   && ground.IsStable
                   && ground.Distance <= settings.GroundSnapDistance;
        }

        private void UpdateStepOffset(bool stableGrounded)
        {
            controller.stepOffset = stableGrounded && !isJumping
                ? settings.GroundedStepOffset
                : settings.AirborneStepOffset;
        }

        private void UpdatePlanarVelocity(CharacterInputFrame input, bool stableGrounded, float deltaTime)
        {
            Vector3 inputDirection = GetCameraRelativeDirection(input.Move);
            float inputAmount = input.Move.magnitude;
            bool hasInput = inputAmount > settings.InputDeadZone && inputDirection.sqrMagnitude > Mathf.Epsilon;

            if (stableGrounded)
            {
                slideVelocity = Vector3.MoveTowards(slideVelocity, Vector3.zero, settings.Braking * deltaTime);
                Vector3 desiredDirection = hasInput
                    ? SlopeMath.ProjectOnGround(inputDirection, groundState.Normal)
                    : Vector3.zero;
                float targetSpeed = GetGroundSpeed(input, desiredDirection, inputAmount);
                desiredVelocity = desiredDirection * targetSpeed;
                float rate = desiredVelocity.sqrMagnitude > planarVelocity.sqrMagnitude
                    ? settings.Acceleration
                    : settings.Braking;
                planarVelocity = Vector3.MoveTowards(planarVelocity, desiredVelocity, rate * deltaTime);
                return;
            }

            if (groundState.IsSteep)
            {
                Vector3 downhill = SlopeMath.GetDownhillDirection(groundState.Normal);
                Vector3 targetSlide = downhill * settings.MaxSlideSpeed;
                slideVelocity = Vector3.MoveTowards(
                    slideVelocity,
                    targetSlide,
                    settings.SteepSlideAcceleration * deltaTime);

                Vector3 lateralDirection = Vector3.zero;
                if (hasInput)
                {
                    Vector3 directionOnSlope = SlopeMath.ProjectOnGround(inputDirection, groundState.Normal);
                    lateralDirection = SlopeMath.RemoveUphillComponent(directionOnSlope, groundState.Normal);
                }

                desiredVelocity = lateralDirection * settings.WalkSpeed * inputAmount * settings.SteepSlopeLateralControl;
                planarVelocity = Vector3.MoveTowards(
                    planarVelocity,
                    desiredVelocity,
                    settings.Acceleration * settings.SteepSlopeLateralControl * deltaTime);
                return;
            }

            slideVelocity = Vector3.MoveTowards(slideVelocity, Vector3.zero, settings.Braking * deltaTime);
            float airSpeed = (input.SprintHeld ? settings.SprintSpeed : settings.WalkSpeed)
                             * inputAmount
                             * settings.AirControl;
            desiredVelocity = hasInput ? inputDirection * airSpeed : Vector3.zero;
            planarVelocity = Vector3.MoveTowards(
                planarVelocity,
                desiredVelocity,
                settings.AirAcceleration * deltaTime);
        }

        private float GetGroundSpeed(CharacterInputFrame input, Vector3 desiredDirection, float inputAmount)
        {
            float baseSpeed = input.SprintHeld ? settings.SprintSpeed : settings.WalkSpeed;
            float targetSpeed = baseSpeed * Mathf.Clamp01(inputAmount);
            if (desiredDirection.sqrMagnitude <= Mathf.Epsilon || !groundState.Hit)
            {
                return targetSpeed;
            }

            float directionalAngle = SlopeMath.GetDirectionalSlopeAngle(desiredDirection, groundState.Normal);
            if (directionalAngle > 0f)
            {
                targetSpeed *= settings.EvaluateUphillMultiplier(directionalAngle);
                if (input.SprintHeld)
                {
                    targetSpeed = Mathf.Lerp(
                        targetSpeed,
                        Mathf.Min(targetSpeed, settings.WalkSpeed * inputAmount),
                        1f - settings.SprintUphillRetention);
                }
            }
            else if (directionalAngle < 0f)
            {
                targetSpeed *= settings.EvaluateDownhillMultiplier(-directionalAngle);
            }

            return targetSpeed;
        }

        private void UpdateVerticalVelocity(CharacterInputFrame input, bool stableGrounded, float deltaTime)
        {
            if (stableGrounded && !isJumping)
            {
                if (verticalVelocity < 0f)
                {
                    verticalVelocity = settings.GroundStickVelocity;
                }

                return;
            }

            float gravity = settings.Gravity;
            if (!input.JumpHeld && verticalVelocity > 0f)
            {
                gravity *= settings.JumpCutGravityMultiplier;
            }

            verticalVelocity = Mathf.Max(
                settings.TerminalVelocity,
                verticalVelocity + gravity * deltaTime);
        }

        private Vector3 GetCameraRelativeDirection(Vector2 move)
        {
            ResolveOrientation();

            Vector3 forward = orientation != null ? orientation.forward : transform.forward;
            Vector3 right = orientation != null ? orientation.right : transform.right;
            forward = Vector3.ProjectOnPlane(forward, Vector3.up);
            right = Vector3.ProjectOnPlane(right, Vector3.up);

            if (forward.sqrMagnitude <= Mathf.Epsilon)
            {
                forward = transform.forward;
            }

            if (right.sqrMagnitude <= Mathf.Epsilon)
            {
                right = transform.right;
            }

            Vector3 direction = forward.normalized * move.y + right.normalized * move.x;
            return direction.sqrMagnitude > 1f ? direction.normalized : direction;
        }

        private void RotateTowardMotion(float deltaTime)
        {
            Vector3 facing = Vector3.ProjectOnPlane(planarVelocity + slideVelocity, Vector3.up);
            if (facing.sqrMagnitude <= 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(facing.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                settings.RotationSpeed * deltaTime);
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawDebugGizmos)
            {
                return;
            }

            groundProbe.DrawGizmos();

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + desiredVelocity);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + actualVelocity);
        }
    }
}
