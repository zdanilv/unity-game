using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityGame.CharacterMovement.Editor
{
    public static class CharacterMovementTestSceneBuilder
    {
        public const string TestScenePath =
            "Assets/Game/CharacterMovement/Scenes/CharacterMovementTest.unity";

        [MenuItem("Tools/Character Movement/Rebuild Test Scene")]
        public static void RebuildTestSceneMenu()
        {
            CharacterMovementPrefabSetup.EnsureFolders();
            int groundLayer = CharacterMovementPrefabSetup.EnsureGroundLayer();
            CharacterMovementSettings settings = CharacterMovementPrefabSetup.EnsureSettingsAsset(groundLayer);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CharacterMovementPrefabSetup.PlayerPrefabPath);
            if (prefab == null)
            {
                prefab = CharacterMovementPrefabSetup.RebuildPlayerPrefab(settings);
            }

            RebuildTestScene(prefab, settings, groundLayer);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal static void RebuildTestScene(
            GameObject playerPrefab,
            CharacterMovementSettings settings,
            int groundLayer)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "CharacterMovementTest";

            GameObject light = new GameObject("Directional Light");
            Light lightComponent = light.AddComponent<Light>();
            lightComponent.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            camera.transform.position = new Vector3(0f, 7f, -12f);
            camera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, -0.45f, 1f), Vector3.up);

            CreateCube("Ground_Flat", new Vector3(0f, -0.5f, 0f), new Vector3(28f, 1f, 28f), Quaternion.identity, groundLayer);
            CreateRamp("Ramp_30", 30f, new Vector3(-8f, 0.12f, 5f), groundLayer);
            CreateRamp("Ramp_45", 45f, new Vector3(0f, 0.12f, 5f), groundLayer);
            CreateRamp("Ramp_60_Steep", 60f, new Vector3(8f, 0.12f, 5f), groundLayer);

            CreateCube("Step_015", new Vector3(-7f, 0.075f, -3f), new Vector3(1.4f, 0.15f, 1.2f), Quaternion.identity, groundLayer);
            CreateCube("Step_025", new Vector3(-5.2f, 0.125f, -3f), new Vector3(1.4f, 0.25f, 1.2f), Quaternion.identity, groundLayer);
            CreateCube("Step_035_Blocking", new Vector3(-3.4f, 0.175f, -3f), new Vector3(1.4f, 0.35f, 1.2f), Quaternion.identity, groundLayer);

            CreateCube("Wall", new Vector3(4f, 1f, -3f), new Vector3(0.35f, 2f, 4f), Quaternion.identity, groundLayer);
            CreateCube("Ceiling", new Vector3(0f, 2.35f, -5f), new Vector3(4f, 0.3f, 4f), Quaternion.identity, groundLayer);
            CreateCube("Ledge", new Vector3(7f, 0.25f, -5f), new Vector3(4f, 0.5f, 4f), Quaternion.identity, groundLayer);

            if (playerPrefab != null)
            {
                GameObject player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
                if (player != null)
                {
                    player.name = "PlayerCharacter";
                    player.transform.position = new Vector3(0f, 0f, -8f);
                    CharacterMovementPrefabSetup.ConfigureSceneInstance(player, settings);
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, TestScenePath);
        }

        private static GameObject CreateRamp(string name, float angle, Vector3 position, int layer)
        {
            Quaternion rotation = Quaternion.Euler(angle, 0f, 0f);
            return CreateCube(name, position, new Vector3(4f, 0.2f, 6f), rotation, layer);
        }

        private static GameObject CreateCube(
            string name,
            Vector3 position,
            Vector3 scale,
            Quaternion rotation,
            int layer)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.layer = layer;
            cube.transform.SetPositionAndRotation(position, rotation);
            cube.transform.localScale = scale;
            return cube;
        }
    }
}
