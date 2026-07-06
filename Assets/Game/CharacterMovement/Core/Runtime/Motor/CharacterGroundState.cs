using UnityEngine;

namespace UnityGame.CharacterMovement
{
    public enum CharacterGroundingStatus
    {
        Airborne,
        StableGround,
        SteepSlope
    }

    public readonly struct CharacterGroundState
    {
        public CharacterGroundState(
            CharacterGroundingStatus status,
            bool hit,
            Vector3 point,
            Vector3 normal,
            float distance,
            float slopeAngle,
            Collider collider)
        {
            Status = status;
            Hit = hit;
            Point = point;
            Normal = normal.sqrMagnitude > 0f ? normal.normalized : Vector3.up;
            Distance = distance;
            SlopeAngle = slopeAngle;
            Collider = collider;
        }

        public CharacterGroundingStatus Status { get; }
        public bool Hit { get; }
        public Vector3 Point { get; }
        public Vector3 Normal { get; }
        public float Distance { get; }
        public float SlopeAngle { get; }
        public Collider Collider { get; }

        public bool IsStable
        {
            get { return Status == CharacterGroundingStatus.StableGround; }
        }

        public bool IsSteep
        {
            get { return Status == CharacterGroundingStatus.SteepSlope; }
        }

        public static CharacterGroundState Airborne
        {
            get
            {
                return new CharacterGroundState(
                    CharacterGroundingStatus.Airborne,
                    false,
                    Vector3.zero,
                    Vector3.up,
                    float.PositiveInfinity,
                    0f,
                    null);
            }
        }
    }
}
