using UnityEngine;

namespace UnityGame.CharacterMovement
{
    [System.Serializable]
    public sealed class GroundProbe
    {
        private const int MaxHits = 8;

        private readonly RaycastHit[] hits = new RaycastHit[MaxHits];

        private Vector3 lastOrigin;
        private Vector3 lastBottomSphereCenter;
        private float lastRadius;
        private float lastDistance;
        private CharacterGroundState lastState = CharacterGroundState.Airborne;

        public CharacterGroundState LastState
        {
            get { return lastState; }
        }

        public CharacterGroundState Probe(
            CharacterController controller,
            Transform owner,
            CharacterMovementSettings settings,
            float distance)
        {
            if (controller == null || owner == null || settings == null)
            {
                lastState = CharacterGroundState.Airborne;
                return lastState;
            }

            float radius = Mathf.Max(0.01f, controller.radius - settings.ProbeRadiusInset);
            Vector3 up = owner.up;
            Vector3 center = owner.TransformPoint(controller.center);
            float halfCapsule = Mathf.Max(0f, controller.height * 0.5f - controller.radius);
            Vector3 bottomSphereCenter = center - up * halfCapsule;
            Vector3 origin = bottomSphereCenter + up * settings.ProbeStartOffset;
            float castDistance = Mathf.Max(0.01f, distance + settings.ProbeStartOffset);

            lastOrigin = origin;
            lastBottomSphereCenter = bottomSphereCenter;
            lastRadius = radius;
            lastDistance = castDistance;

            int hitCount = Physics.SphereCastNonAlloc(
                origin,
                radius,
                -up,
                hits,
                castDistance,
                settings.GroundLayers,
                QueryTriggerInteraction.Ignore);

            RaycastHit bestHit = default;
            bool found = false;
            float bestDistance = float.PositiveInfinity;

            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider == null || hit.collider == controller || hit.normal.sqrMagnitude <= Mathf.Epsilon)
                {
                    continue;
                }

                float slopeAngle = SlopeMath.GetSlopeAngle(hit.normal);
                if (slopeAngle > settings.MaxGroundAngle)
                {
                    continue;
                }

                if (hit.distance < bestDistance)
                {
                    bestDistance = hit.distance;
                    bestHit = hit;
                    found = true;
                }
            }

            if (!found)
            {
                lastState = CharacterGroundState.Airborne;
                return lastState;
            }

            float groundDistance = Mathf.Max(0f, bestHit.distance - settings.ProbeStartOffset);
            float angle = SlopeMath.GetSlopeAngle(bestHit.normal);
            CharacterGroundingStatus status = angle <= settings.SlopeLimit
                ? CharacterGroundingStatus.StableGround
                : CharacterGroundingStatus.SteepSlope;

            lastState = new CharacterGroundState(
                status,
                true,
                bestHit.point,
                bestHit.normal,
                groundDistance,
                angle,
                bestHit.collider);

            return lastState;
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(lastOrigin, lastRadius);
            Gizmos.DrawLine(lastOrigin, lastOrigin + Vector3.down * lastDistance);

            Gizmos.color = lastState.IsStable ? Color.green : (lastState.IsSteep ? Color.yellow : Color.gray);
            Gizmos.DrawWireSphere(lastBottomSphereCenter, lastRadius);

            if (lastState.Hit)
            {
                Gizmos.color = lastState.IsStable ? Color.green : Color.red;
                Gizmos.DrawLine(lastState.Point, lastState.Point + lastState.Normal);
            }
        }
    }
}
