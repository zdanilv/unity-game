using System.IO;
using ithappy.Creative_Characters_FREE.Controller;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGame.CharacterMovement.Ithappy;

namespace UnityGame.CharacterMovement.Editor
{
    public static class CharacterMovementPrefabSetup
    {
        public const string SourceCharacterPrefabPath =
            "Assets/ithappy/Creative_Characters_FREE/Saved_Characters/start_character/Character.prefab";

        public const string PlayerPrefabPath =
            "Assets/Game/CharacterMovement/Prefabs/PlayerCharacter.prefab";

        public const string SettingsPath =
            "Assets/Game/CharacterMovement/Config/DefaultCharacterMovementSettings.asset";

        public const string Level0ScenePath = "Assets/Level_0.unity";
        public const string GroundLayerName = "Ground";

        [MenuItem("Tools/Character Movement/Run Full Setup")]
        public static void RunFullSetup()
        {
            EnsureFolders();
            int groundLayer = EnsureGroundLayer();
            CharacterMovementSettings settings = EnsureSettingsAsset(groundLayer);
            GameObject prefab = RebuildPlayerPrefab(settings);
            CharacterMovementTestSceneBuilder.RebuildTestScene(prefab, settings, groundLayer);
            IntegrateLevel0();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Character movement setup completed.");
        }

        [MenuItem("Tools/Character Movement/Rebuild Player Character Prefab")]
        public static void RebuildPlayerPrefabMenu()
        {
            EnsureFolders();
            int groundLayer = EnsureGroundLayer();
            CharacterMovementSettings settings = EnsureSettingsAsset(groundLayer);
            RebuildPlayerPrefab(settings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Character Movement/Integrate Level_0")]
        public static void IntegrateLevel0()
        {
            EnsureFolders();
            int groundLayer = EnsureGroundLayer();
            CharacterMovementSettings settings = EnsureSettingsAsset(groundLayer);
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (playerPrefab == null)
            {
                playerPrefab = RebuildPlayerPrefab(settings);
            }

            EditorSceneManager.OpenScene(Level0ScenePath, OpenSceneMode.Single);
            Vector3 spawnPosition = Vector3.zero;
            Quaternion spawnRotation = Quaternion.identity;

            foreach (PlayerCharacterMotor existing in Object.FindObjectsByType<PlayerCharacterMotor>())
            {
                Object.DestroyImmediate(existing.gameObject);
            }

            foreach (MovePlayerInput oldInput in Object.FindObjectsByType<MovePlayerInput>())
            {
                if (oldInput != null)
                {
                    spawnPosition = oldInput.transform.position;
                    spawnRotation = oldInput.transform.rotation;
                    Object.DestroyImmediate(oldInput.gameObject);
                }
            }

            foreach (CharacterMover oldMover in Object.FindObjectsByType<CharacterMover>())
            {
                if (oldMover != null)
                {
                    spawnPosition = oldMover.transform.position;
                    spawnRotation = oldMover.transform.rotation;
                    Object.DestroyImmediate(oldMover.gameObject);
                }
            }

            GameObject player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
            if (player == null)
            {
                Debug.LogError("Unable to instantiate PlayerCharacter prefab.");
                return;
            }

            player.name = "PlayerCharacter";
            player.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            ConfigureSceneInstance(player, settings);
            AssignGroundLayerToSceneColliders(groundLayer);

            Scene activeScene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);
        }

        internal static void EnsureFolders()
        {
            EnsureAssetFolder("Assets/Game");
            EnsureAssetFolder("Assets/Game/CharacterMovement");
            EnsureAssetFolder("Assets/Game/CharacterMovement/Config");
            EnsureAssetFolder("Assets/Game/CharacterMovement/Prefabs");
            EnsureAssetFolder("Assets/Game/CharacterMovement/Scenes");
        }

        internal static CharacterMovementSettings EnsureSettingsAsset(int groundLayer)
        {
            CharacterMovementSettings settings =
                AssetDatabase.LoadAssetAtPath<CharacterMovementSettings>(SettingsPath);

            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<CharacterMovementSettings>();
                settings.ResetToRecommendedDefaults();
                settings.ConfigureGroundLayerMask(1 << groundLayer);
                AssetDatabase.CreateAsset(settings, SettingsPath);
            }
            else
            {
                settings.ConfigureGroundLayerMask(1 << groundLayer);
                EditorUtility.SetDirty(settings);
            }

            return settings;
        }

        internal static int EnsureGroundLayer()
        {
            int existing = LayerMask.NameToLayer(GroundLayerName);
            if (existing >= 0)
            {
                return existing;
            }

            Object tagManagerAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
            SerializedObject tagManager = new SerializedObject(tagManagerAsset);
            SerializedProperty layers = tagManager.FindProperty("layers");

            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layer = layers.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(layer.stringValue))
                {
                    layer.stringValue = GroundLayerName;
                    tagManager.ApplyModifiedProperties();
                    return i;
                }
            }

            Debug.LogError("No free layer slot is available for the Ground layer.");
            return 0;
        }

        internal static GameObject RebuildPlayerPrefab(CharacterMovementSettings settings)
        {
            GameObject sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SourceCharacterPrefabPath);
            if (sourcePrefab == null)
            {
                Debug.LogError("Source ithappy character prefab was not found: " + SourceCharacterPrefabPath);
                return null;
            }

            EnsureAssetFolder(Path.GetDirectoryName(PlayerPrefabPath).Replace('\\', '/'));

            GameObject root = new GameObject("PlayerCharacter");
            CharacterController controller = root.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.4f;
            controller.center = new Vector3(0f, 0.95f, 0f);
            controller.slopeLimit = 45f;
            controller.stepOffset = 0.3f;
            controller.skinWidth = 0.08f;
            controller.minMoveDistance = 0.001f;

            PlayerMovementInput input = root.AddComponent<PlayerMovementInput>();
            PlayerCharacterMotor motor = root.AddComponent<PlayerCharacterMotor>();
            PlayerLocomotionAnimator locomotionAnimator = root.AddComponent<PlayerLocomotionAnimator>();
            IthappyCameraInputAdapter cameraAdapter = root.AddComponent<IthappyCameraInputAdapter>();

            GameObject visual = PrefabUtility.InstantiatePrefab(sourcePrefab) as GameObject;
            if (visual == null)
            {
                Object.DestroyImmediate(root);
                Debug.LogError("Unable to instantiate source ithappy character prefab.");
                return null;
            }

            visual.name = "CharacterVisual";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;

            RemoveComponentIfExists<MovePlayerInput>(visual);
            RemoveComponentIfExists<CharacterMover>(visual);
            RemoveComponentIfExists<CharacterController>(visual);

            Animator animator = visual.GetComponent<Animator>();
            if (animator != null)
            {
                animator.applyRootMotion = false;
            }

            SetObjectReference(motor, "settings", settings);
            SetObjectReference(motor, "inputSourceBehaviour", input);
            SetObjectReference(locomotionAnimator, "motor", motor);
            SetObjectReference(locomotionAnimator, "animator", animator);
            SetObjectReference(cameraAdapter, "inputSourceBehaviour", input);
            SetObjectReference(cameraAdapter, "player", root.transform);

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefabPath);
            Object.DestroyImmediate(root);
            return saved;
        }

        internal static void ConfigureSceneInstance(GameObject player, CharacterMovementSettings settings)
        {
            if (player == null)
            {
                return;
            }

            PlayerMovementInput input = player.GetComponent<PlayerMovementInput>();
            PlayerCharacterMotor motor = player.GetComponent<PlayerCharacterMotor>();
            PlayerLocomotionAnimator locomotionAnimator = player.GetComponent<PlayerLocomotionAnimator>();
            IthappyCameraInputAdapter cameraAdapter = player.GetComponent<IthappyCameraInputAdapter>();
            Animator animator = player.GetComponentInChildren<Animator>();
            PlayerCamera playerCamera = Object.FindAnyObjectByType<PlayerCamera>();
            Transform orientation = playerCamera != null ? playerCamera.transform : null;

            if (orientation == null && Camera.main != null)
            {
                orientation = Camera.main.transform;
            }

            SetObjectReference(motor, "settings", settings);
            SetObjectReference(motor, "inputSourceBehaviour", input);
            SetObjectReference(motor, "orientation", orientation);
            SetObjectReference(locomotionAnimator, "motor", motor);
            SetObjectReference(locomotionAnimator, "animator", animator);
            SetObjectReference(cameraAdapter, "inputSourceBehaviour", input);
            SetObjectReference(cameraAdapter, "cameraTarget", playerCamera);
            SetObjectReference(cameraAdapter, "player", player.transform);

            if (playerCamera != null)
            {
                playerCamera.SetPlayer(player.transform);
            }
        }

        internal static void AssignGroundLayerToSceneColliders(int groundLayer)
        {
            foreach (MeshCollider meshCollider in Object.FindObjectsByType<MeshCollider>())
            {
                if (meshCollider.GetComponentInParent<PlayerCharacterMotor>() == null)
                {
                    meshCollider.gameObject.layer = groundLayer;
                }
            }

            foreach (BoxCollider boxCollider in Object.FindObjectsByType<BoxCollider>())
            {
                if (boxCollider.GetComponentInParent<PlayerCharacterMotor>() == null)
                {
                    boxCollider.gameObject.layer = groundLayer;
                }
            }
        }

        private static void EnsureAssetFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder) || AssetDatabase.IsValidFolder(folder))
            {
                return;
            }

            string parent = Path.GetDirectoryName(folder).Replace('\\', '/');
            string child = Path.GetFileName(folder);
            EnsureAssetFolder(parent);
            AssetDatabase.CreateFolder(parent, child);
        }

        private static void RemoveComponentIfExists<T>(GameObject target)
            where T : Component
        {
            T component = target.GetComponent<T>();
            if (component != null)
            {
                Object.DestroyImmediate(component);
            }
        }

        private static void SetObjectReference(Object target, string propertyName, Object value)
        {
            if (target == null)
            {
                return;
            }

            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                Debug.LogError("Serialized property was not found: " + propertyName, target);
                return;
            }

            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }
}
