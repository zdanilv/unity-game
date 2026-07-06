using ithappy.Creative_Characters_FREE.Controller;
using UnityEngine;

namespace UnityGame.CharacterMovement.Ithappy
{
    [DisallowMultipleComponent]
    public sealed class IthappyCameraInputAdapter : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour inputSourceBehaviour;
        [SerializeField] private PlayerCamera cameraTarget;
        [SerializeField] private Transform player;

        private ICharacterInputSource inputSource;
        private bool invalidInputReported;

        private void Awake()
        {
            ResolveInputSource();
            ResolveCamera();

            if (player == null)
            {
                player = transform;
            }
        }

        private void OnEnable()
        {
            ResolveCamera();
            if (cameraTarget != null && player != null)
            {
                cameraTarget.SetPlayer(player);
            }
        }

        private void Update()
        {
            if (!ResolveInputSource())
            {
                return;
            }

            ResolveCamera();
            if (cameraTarget == null)
            {
                return;
            }

            if (player != null)
            {
                cameraTarget.SetPlayer(player);
            }

            CharacterInputFrame input = inputSource.ReadInput();
            Vector2 look = input.Look;
            float zoom = input.Zoom;
            cameraTarget.SetInput(in look, zoom);
        }

        private bool ResolveInputSource()
        {
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

        private void ResolveCamera()
        {
            if (cameraTarget != null)
            {
                return;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTarget = mainCamera.GetComponent<PlayerCamera>();
            }
        }
    }
}
