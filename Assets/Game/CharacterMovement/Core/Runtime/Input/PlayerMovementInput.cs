using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityGame.CharacterMovement
{
    [DisallowMultipleComponent]
    public sealed class PlayerMovementInput : MonoBehaviour, ICharacterInputSource
    {
        [Header("Mouse")]
        [SerializeField, Min(0f)] private float mouseLookScale = 0.02f;
        [SerializeField, Min(0f)] private float mouseZoomScale = 0.01f;

        [Header("Gamepad")]
        [SerializeField, Min(0f)] private float gamepadLookScale = 2.2f;
        [SerializeField, Min(0f)] private float gamepadZoomScale = 1f;

        private PlayerControls controls;
        private CharacterInputFrame currentFrame;

        public CharacterInputFrame CurrentFrame
        {
            get { return currentFrame; }
        }

        private void Awake()
        {
            controls = new PlayerControls();
        }

        private void OnEnable()
        {
            if (controls == null)
            {
                controls = new PlayerControls();
            }

            controls.Player.Enable();
            currentFrame = CharacterInputFrame.Empty;
        }

        private void OnDisable()
        {
            if (controls != null)
            {
                controls.Player.Disable();
            }

            currentFrame = CharacterInputFrame.Empty;
        }

        private void OnDestroy()
        {
            if (controls != null)
            {
                controls.Dispose();
                controls = null;
            }
        }

        private void Update()
        {
            currentFrame = ReadInput();
        }

        public CharacterInputFrame ReadInput()
        {
            if (controls == null)
            {
                return CharacterInputFrame.Empty;
            }

            PlayerControls.PlayerActions player = controls.Player;
            Vector2 move = Vector2.ClampMagnitude(player.Move.ReadValue<Vector2>(), 1f);
            Vector2 look = player.Look.ReadValue<Vector2>();
            float zoom = player.Zoom.ReadValue<float>();

            InputControl lookControl = player.Look.activeControl;
            if (lookControl != null && lookControl.device is Mouse)
            {
                look *= mouseLookScale;
            }
            else
            {
                look *= gamepadLookScale * Time.deltaTime;
            }

            InputControl zoomControl = player.Zoom.activeControl;
            if (zoomControl != null && zoomControl.device is Mouse)
            {
                zoom *= mouseZoomScale;
            }
            else
            {
                zoom *= gamepadZoomScale * Time.deltaTime;
            }

            currentFrame = new CharacterInputFrame(
                move,
                player.Sprint.IsPressed(),
                player.Jump.WasPressedThisFrame(),
                player.Jump.IsPressed(),
                player.Jump.WasReleasedThisFrame(),
                look,
                zoom);

            return currentFrame;
        }
    }
}
