using NUnit.Framework;
using UnityEngine;

namespace UnityGame.CharacterMovement.Tests
{
    public sealed class SlopeMathTests
    {
        [Test]
        public void GetSlopeAngle_ReturnsExpectedAngles()
        {
            Assert.That(SlopeMath.GetSlopeAngle(Vector3.up), Is.EqualTo(0f).Within(0.01f));

            Vector3 thirtyDegreeNormal = Quaternion.AngleAxis(30f, Vector3.right) * Vector3.up;
            Vector3 sixtyDegreeNormal = Quaternion.AngleAxis(60f, Vector3.right) * Vector3.up;

            Assert.That(SlopeMath.GetSlopeAngle(thirtyDegreeNormal), Is.EqualTo(30f).Within(0.01f));
            Assert.That(SlopeMath.GetSlopeAngle(sixtyDegreeNormal), Is.EqualTo(60f).Within(0.01f));
        }

        [Test]
        public void DirectionalSlopeAngle_DistinguishesUphillDownhillAndCrossSlope()
        {
            Vector3 normal = Quaternion.AngleAxis(30f, Vector3.right) * Vector3.up;
            Vector3 downhill = SlopeMath.GetDownhillDirection(normal);
            Vector3 uphill = -downhill;
            Vector3 crossSlope = Vector3.Cross(normal, downhill).normalized;

            Assert.That(SlopeMath.GetDirectionalSlopeAngle(uphill, normal), Is.GreaterThan(29f));
            Assert.That(SlopeMath.GetDirectionalSlopeAngle(downhill, normal), Is.LessThan(-29f));
            Assert.That(SlopeMath.GetDirectionalSlopeAngle(crossSlope, normal), Is.EqualTo(0f).Within(0.01f));
        }

        [Test]
        public void ProjectOnGround_ReturnsUnitDirectionOnPlane()
        {
            Vector3 normal = Quaternion.AngleAxis(30f, Vector3.right) * Vector3.up;
            Vector3 projected = SlopeMath.ProjectOnGround(Vector3.forward, normal);

            Assert.That(projected.magnitude, Is.EqualTo(1f).Within(0.001f));
            Assert.That(Vector3.Dot(projected, normal), Is.EqualTo(0f).Within(0.001f));
        }
    }
}
