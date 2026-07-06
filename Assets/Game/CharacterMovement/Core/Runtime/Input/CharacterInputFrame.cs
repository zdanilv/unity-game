using UnityEngine;

namespace UnityGame.CharacterMovement
{
    public readonly struct CharacterInputFrame
    {
        public static readonly CharacterInputFrame Empty = new CharacterInputFrame(
            Vector2.zero,
            false,
            false,
            false,
            false,
            Vector2.zero,
            0f);

        public CharacterInputFrame(
            Vector2 move,
            bool sprintHeld,
            bool jumpPressed,
            bool jumpHeld,
            bool jumpReleased,
            Vector2 look,
            float zoom)
        {
            Move = Vector2.ClampMagnitude(move, 1f);
            SprintHeld = sprintHeld;
            JumpPressed = jumpPressed;
            JumpHeld = jumpHeld;
            JumpReleased = jumpReleased;
            Look = look;
            Zoom = zoom;
        }

        public Vector2 Move { get; }
        public bool SprintHeld { get; }
        public bool JumpPressed { get; }
        public bool JumpHeld { get; }
        public bool JumpReleased { get; }
        public Vector2 Look { get; }
        public float Zoom { get; }
    }
}
