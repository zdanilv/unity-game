using UnityEngine;

namespace UnityGame.CharacterMovement
{
    public static class SlopeMath
    {
        public static float GetSlopeAngle(Vector3 normal)
        {
            if (normal.sqrMagnitude <= Mathf.Epsilon)
            {
                return 0f;
            }

            return Vector3.Angle(normal.normalized, Vector3.up);
        }

        public static bool IsStableGround(Vector3 normal, float slopeLimit)
        {
            return GetSlopeAngle(normal) <= slopeLimit;
        }

        public static Vector3 ProjectOnGround(Vector3 direction, Vector3 groundNormal)
        {
            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return Vector3.zero;
            }

            Vector3 projected = Vector3.ProjectOnPlane(direction, groundNormal);
            return projected.sqrMagnitude <= Mathf.Epsilon ? Vector3.zero : projected.normalized;
        }

        public static Vector3 GetDownhillDirection(Vector3 groundNormal)
        {
            Vector3 downhill = Vector3.ProjectOnPlane(Vector3.down, groundNormal);
            return downhill.sqrMagnitude <= Mathf.Epsilon ? Vector3.zero : downhill.normalized;
        }

        public static float GetDirectionalSlopeAngle(Vector3 surfaceDirection, Vector3 groundNormal)
        {
            if (surfaceDirection.sqrMagnitude <= Mathf.Epsilon)
            {
                return 0f;
            }

            float slopeAngle = GetSlopeAngle(groundNormal);
            Vector3 downhill = GetDownhillDirection(groundNormal);
            if (downhill.sqrMagnitude <= Mathf.Epsilon)
            {
                return 0f;
            }

            Vector3 directionOnPlane = ProjectOnGround(surfaceDirection, groundNormal);
            return -Vector3.Dot(directionOnPlane, downhill) * slopeAngle;
        }

        public static Vector3 RemoveUphillComponent(Vector3 directionOnPlane, Vector3 groundNormal)
        {
            Vector3 downhill = GetDownhillDirection(groundNormal);
            if (downhill.sqrMagnitude <= Mathf.Epsilon || directionOnPlane.sqrMagnitude <= Mathf.Epsilon)
            {
                return Vector3.zero;
            }

            Vector3 uphill = -downhill;
            Vector3 lateral = Vector3.ProjectOnPlane(directionOnPlane, uphill);
            return lateral.sqrMagnitude <= Mathf.Epsilon ? Vector3.zero : lateral.normalized;
        }
    }
}
