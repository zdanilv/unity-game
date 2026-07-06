using UnityEngine;

namespace UnityGame.CharacterMovement
{
    public readonly struct CharacterMotionState
    {
        public CharacterMotionState(
            Vector3 desiredVelocity,
            Vector3 planarVelocity,
            Vector3 slideVelocity,
            Vector3 actualVelocity,
            float verticalVelocity,
            CharacterGroundState ground,
            CollisionFlags collisionFlags,
            bool isJumping)
        {
            DesiredVelocity = desiredVelocity;
            PlanarVelocity = planarVelocity;
            SlideVelocity = slideVelocity;
            ActualVelocity = actualVelocity;
            VerticalVelocity = verticalVelocity;
            Ground = ground;
            CollisionFlags = collisionFlags;
            IsJumping = isJumping;
        }

        public Vector3 DesiredVelocity { get; }
        public Vector3 PlanarVelocity { get; }
        public Vector3 SlideVelocity { get; }
        public Vector3 ActualVelocity { get; }
        public float VerticalVelocity { get; }
        public CharacterGroundState Ground { get; }
        public CollisionFlags CollisionFlags { get; }
        public bool IsJumping { get; }

        public bool IsGrounded
        {
            get { return Ground.IsStable; }
        }

        public bool IsSliding
        {
            get { return Ground.IsSteep && SlideVelocity.sqrMagnitude > 0.01f; }
        }

        public static CharacterMotionState Empty
        {
            get
            {
                return new CharacterMotionState(
                    Vector3.zero,
                    Vector3.zero,
                    Vector3.zero,
                    Vector3.zero,
                    0f,
                    CharacterGroundState.Airborne,
                    CollisionFlags.None,
                    false);
            }
        }
    }
}
