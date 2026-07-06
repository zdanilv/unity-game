using UnityEngine;

namespace UnityGame.CharacterMovement
{
    [CreateAssetMenu(
        fileName = "DefaultCharacterMovementSettings",
        menuName = "Game/Character Movement Settings")]
    public sealed class CharacterMovementSettings : ScriptableObject
    {
        [Header("Speed")]
        [SerializeField, Min(0f)] private float walkSpeed = 1.6f;
        [SerializeField, Min(0f)] private float sprintSpeed = 4.2f;
        [SerializeField, Min(0f)] private float acceleration = 16f;
        [SerializeField, Min(0f)] private float braking = 22f;
        [SerializeField, Min(0f)] private float airAcceleration = 5f;
        [SerializeField, Range(0f, 1f)] private float airControl = 0.45f;
        [SerializeField, Min(0f)] private float rotationSpeed = 540f;
        [SerializeField, Range(0f, 0.5f)] private float inputDeadZone = 0.05f;

        [Header("Ground Probe")]
        [SerializeField] private LayerMask groundLayers = Physics.DefaultRaycastLayers;
        [SerializeField, Min(0.01f)] private float probeDistance = 0.36f;
        [SerializeField, Range(0f, 0.2f)] private float probeStartOffset = 0.08f;
        [SerializeField, Range(0f, 0.2f)] private float probeRadiusInset = 0.03f;
        [SerializeField, Range(45f, 89f)] private float maxGroundAngle = 85f;

        [Header("Slopes")]
        [SerializeField, Range(0f, 89f)] private float slopeLimit = 45f;
        [SerializeField] private AnimationCurve uphillSpeedMultiplier = AnimationCurve.Linear(0f, 1f, 45f, 0.55f);
        [SerializeField] private AnimationCurve downhillSpeedMultiplier = AnimationCurve.Linear(0f, 1f, 45f, 1.18f);
        [SerializeField, Range(0f, 1f)] private float sprintUphillRetention = 0.65f;

        [Header("Steep Slopes")]
        [SerializeField, Min(0f)] private float steepSlideAcceleration = 10f;
        [SerializeField, Min(0f)] private float maxSlideSpeed = 7f;
        [SerializeField, Range(0f, 1f)] private float steepSlopeLateralControl = 0.35f;

        [Header("Jump And Gravity")]
        [SerializeField, Min(0f)] private float jumpHeight = 1.35f;
        [SerializeField] private float gravity = -24f;
        [SerializeField] private float terminalVelocity = -50f;
        [SerializeField, Min(1f)] private float jumpCutGravityMultiplier = 2.4f;
        [SerializeField, Min(0f)] private float coyoteTime = 0.12f;
        [SerializeField, Min(0f)] private float jumpBufferTime = 0.12f;
        [SerializeField] private float groundStickVelocity = -3.5f;

        [Header("Ground Snap")]
        [SerializeField, Min(0f)] private float groundSnapDistance = 0.38f;
        [SerializeField, Min(0f)] private float postJumpSnapGrace = 0.15f;
        [SerializeField, Min(0f)] private float groundedStepOffset = 0.3f;
        [SerializeField, Min(0f)] private float airborneStepOffset = 0f;

        public float WalkSpeed { get { return walkSpeed; } }
        public float SprintSpeed { get { return sprintSpeed; } }
        public float Acceleration { get { return acceleration; } }
        public float Braking { get { return braking; } }
        public float AirAcceleration { get { return airAcceleration; } }
        public float AirControl { get { return airControl; } }
        public float RotationSpeed { get { return rotationSpeed; } }
        public float InputDeadZone { get { return inputDeadZone; } }
        public int GroundLayers { get { return groundLayers.value; } }
        public float ProbeDistance { get { return probeDistance; } }
        public float ProbeStartOffset { get { return probeStartOffset; } }
        public float ProbeRadiusInset { get { return probeRadiusInset; } }
        public float MaxGroundAngle { get { return maxGroundAngle; } }
        public float SlopeLimit { get { return slopeLimit; } }
        public float SprintUphillRetention { get { return sprintUphillRetention; } }
        public float SteepSlideAcceleration { get { return steepSlideAcceleration; } }
        public float MaxSlideSpeed { get { return maxSlideSpeed; } }
        public float SteepSlopeLateralControl { get { return steepSlopeLateralControl; } }
        public float JumpHeight { get { return jumpHeight; } }
        public float Gravity { get { return gravity; } }
        public float TerminalVelocity { get { return terminalVelocity; } }
        public float JumpCutGravityMultiplier { get { return jumpCutGravityMultiplier; } }
        public float CoyoteTime { get { return coyoteTime; } }
        public float JumpBufferTime { get { return jumpBufferTime; } }
        public float GroundStickVelocity { get { return groundStickVelocity; } }
        public float GroundSnapDistance { get { return groundSnapDistance; } }
        public float PostJumpSnapGrace { get { return postJumpSnapGrace; } }
        public float GroundedStepOffset { get { return groundedStepOffset; } }
        public float AirborneStepOffset { get { return airborneStepOffset; } }

        private void OnValidate()
        {
            sprintSpeed = Mathf.Max(sprintSpeed, walkSpeed);
            acceleration = Mathf.Max(0f, acceleration);
            braking = Mathf.Max(0f, braking);
            airAcceleration = Mathf.Max(0f, airAcceleration);
            gravity = Mathf.Min(-0.01f, gravity);
            terminalVelocity = Mathf.Min(-0.01f, terminalVelocity);
            groundStickVelocity = Mathf.Min(-0.01f, groundStickVelocity);
            maxGroundAngle = Mathf.Max(maxGroundAngle, slopeLimit + 0.1f);
        }

        public float EvaluateUphillMultiplier(float angle)
        {
            return Mathf.Max(0f, uphillSpeedMultiplier.Evaluate(Mathf.Abs(angle)));
        }

        public float EvaluateDownhillMultiplier(float angle)
        {
            return Mathf.Max(0f, downhillSpeedMultiplier.Evaluate(Mathf.Abs(angle)));
        }

        public void ConfigureGroundLayerMask(int mask)
        {
            groundLayers = mask;
        }

        public void ResetToRecommendedDefaults()
        {
            walkSpeed = 1.6f;
            sprintSpeed = 4.2f;
            acceleration = 16f;
            braking = 22f;
            airAcceleration = 5f;
            airControl = 0.45f;
            rotationSpeed = 540f;
            inputDeadZone = 0.05f;
            probeDistance = 0.36f;
            probeStartOffset = 0.08f;
            probeRadiusInset = 0.03f;
            maxGroundAngle = 85f;
            slopeLimit = 45f;
            uphillSpeedMultiplier = new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(20f, 0.82f),
                new Keyframe(45f, 0.55f));
            downhillSpeedMultiplier = new AnimationCurve(
                new Keyframe(0f, 1f),
                new Keyframe(20f, 1.08f),
                new Keyframe(45f, 1.18f));
            sprintUphillRetention = 0.65f;
            steepSlideAcceleration = 10f;
            maxSlideSpeed = 7f;
            steepSlopeLateralControl = 0.35f;
            jumpHeight = 1.35f;
            gravity = -24f;
            terminalVelocity = -50f;
            jumpCutGravityMultiplier = 2.4f;
            coyoteTime = 0.12f;
            jumpBufferTime = 0.12f;
            groundStickVelocity = -3.5f;
            groundSnapDistance = 0.38f;
            postJumpSnapGrace = 0.15f;
            groundedStepOffset = 0.3f;
            airborneStepOffset = 0f;
        }
    }
}
