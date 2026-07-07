using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityGame.CharacterMovement.Tests
{
    public sealed class CharacterMotorPlayModeTests
    {
        private readonly System.Collections.Generic.List<Object> createdObjects =
            new System.Collections.Generic.List<Object>();

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = createdObjects.Count - 1; i >= 0; i--)
            {
                if (createdObjects[i] != null)
                {
                    Object.Destroy(createdObjects[i]);
                }
            }

            createdObjects.Clear();
            yield return null;
        }

        [UnityTest]
        public IEnumerator MotorMovesForwardRelativeToOrientation()
        {
            CharacterMovementSettings settings = CreateSettings();
            CreateFloor();

            Transform orientation = CreateOrientation(Quaternion.identity);
            GameObject player = CreatePlayer(settings, orientation, out TestInputSource input, out PlayerCharacterMotor motor);
            input.Frame = new CharacterInputFrame(Vector2.up, false, false, false, false, Vector2.zero, 0f);

            for (int i = 0; i < 90; i++)
            {
                motor.Simulate(input.Frame, 1f / 60f);
                yield return null;
            }

            Assert.That(player.transform.position.z, Is.GreaterThan(1f));
        }

        [UnityTest]
        public IEnumerator MotorJumpRaisesCharacterAfterBufferedPress()
        {
            CharacterMovementSettings settings = CreateSettings();
            CreateFloor();

            Transform orientation = CreateOrientation(Quaternion.identity);
            GameObject player = CreatePlayer(settings, orientation, out TestInputSource input, out PlayerCharacterMotor motor);
            float startY = player.transform.position.y;

            for (int i = 0; i < 8; i++)
            {
                input.Frame = CharacterInputFrame.Empty;
                motor.Simulate(input.Frame, 1f / 60f);
                yield return null;
            }

            input.Frame = new CharacterInputFrame(Vector2.zero, false, true, true, false, Vector2.zero, 0f);
            motor.Simulate(input.Frame, 1f / 60f);
            yield return null;
            input.Frame = new CharacterInputFrame(Vector2.zero, false, false, true, false, Vector2.zero, 0f);

            float maxY = startY;
            for (int i = 0; i < 45; i++)
            {
                motor.Simulate(input.Frame, 1f / 60f);
                maxY = Mathf.Max(maxY, player.transform.position.y);
                yield return null;
            }

            Assert.That(maxY, Is.GreaterThan(startY + 0.35f));
        }

        private CharacterMovementSettings CreateSettings()
        {
            CharacterMovementSettings settings = ScriptableObject.CreateInstance<CharacterMovementSettings>();
            settings.ResetToRecommendedDefaults();
            settings.ConfigureGroundLayerMask(~0);
            createdObjects.Add(settings);
            return settings;
        }

        private void CreateFloor()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Test_Floor";
            floor.transform.position = new Vector3(0f, -0.5f, 0f);
            floor.transform.localScale = new Vector3(20f, 1f, 20f);
            createdObjects.Add(floor);
        }

        private Transform CreateOrientation(Quaternion rotation)
        {
            GameObject orientationObject = new GameObject("Test_Orientation");
            orientationObject.transform.rotation = rotation;
            createdObjects.Add(orientationObject);
            return orientationObject.transform;
        }

        private GameObject CreatePlayer(
            CharacterMovementSettings settings,
            Transform orientation,
            out TestInputSource input,
            out PlayerCharacterMotor motor)
        {
            GameObject player = new GameObject("Test_Player");
            player.transform.position = new Vector3(0f, 0f, 0f);
            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.4f;
            controller.center = new Vector3(0f, 0.95f, 0f);
            controller.slopeLimit = 45f;
            controller.stepOffset = 0.3f;
            controller.skinWidth = 0.08f;

            input = player.AddComponent<TestInputSource>();
            motor = player.AddComponent<PlayerCharacterMotor>();
            motor.Configure(settings, input, orientation);
            motor.enabled = false;

            createdObjects.Add(player);
            return player;
        }

        private sealed class TestInputSource : MonoBehaviour, ICharacterInputSource
        {
            public CharacterInputFrame Frame { get; set; }

            public CharacterInputFrame CurrentFrame
            {
                get { return Frame; }
            }

            public CharacterInputFrame ReadInput()
            {
                return Frame;
            }
        }
    }
}
