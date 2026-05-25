#if UNITY_EDITOR
using MakerFlightRC.Data;
using MakerFlightRC.Runtime.Aircraft;
using MakerFlightRC.Runtime.CameraRig;
using MakerFlightRC.Runtime.Channels;
using MakerFlightRC.Runtime.Input;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakerFlightRC.EditorTools
{
    public static class PopulateBootstrapScene
    {
        private const string ScenePath = "Assets/Scenes/Simulation_Bootstrap.unity";
        private const string DefaultAircraftPath = "Assets/Data/Default_Aircraft.asset";
        private const string InputChannelPath = "Assets/Channels/InputChannel.asset";
        private const string FlightDataChannelPath = "Assets/Channels/FlightDataChannel.asset";

        [MenuItem("MakerFlight RC/Bootstrap/Populate Scene Hierarchy")]
        public static void PopulateScene()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Debug.LogError("Failed to open bootstrap scene.");
                return;
            }

            ClearScene(scene);
            BuildScene();

            EditorSceneManager.SaveScene(scene);
            Debug.Log("Bootstrap scene populated and saved successfully.");
        }

        private static void ClearScene(Scene scene)
        {
            foreach (var go in scene.GetRootGameObjects())
            {
                if (go.name != "Main Camera")
                {
                    Object.DestroyImmediate(go);
                }
            }
        }

        private static void BuildScene()
        {
            var defaultAircraft = AssetDatabase.LoadAssetAtPath<AircraftData>(DefaultAircraftPath);
            var inputChannel = AssetDatabase.LoadAssetAtPath<InputChannelSO>(InputChannelPath);
            var flightDataChannel = AssetDatabase.LoadAssetAtPath<FlightDataChannelSO>(FlightDataChannelPath);

            CreateTerrain();
            var aircraftController = CreateAircraft(defaultAircraft, inputChannel, flightDataChannel);
            WireCamera(aircraftController);
        }

        private static void CreateTerrain()
        {
            var terrain = GameObject.CreatePrimitive(PrimitiveType.Plane);
            terrain.name = "Pist_Terrain";
            terrain.transform.position = Vector3.zero;
            terrain.transform.rotation = Quaternion.identity;
            terrain.transform.localScale = Vector3.one;

            var collider = terrain.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }

            var meshCollider = terrain.AddComponent<MeshCollider>();
            meshCollider.convex = false;
        }

        private static GameObject CreateAircraft(AircraftData aircraft, InputChannelSO inputChannel, FlightDataChannelSO flightDataChannel)
        {
            var aircraftGo = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            aircraftGo.name = "AircraftController";
            aircraftGo.transform.position = new Vector3(0f, 1f, 0f);
            aircraftGo.transform.rotation = Quaternion.identity;

            var collider = aircraftGo.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }

            var rb = aircraftGo.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = aircraftGo.AddComponent<Rigidbody>();
            }
            rb.mass = 1.2f;
            rb.useGravity = true;

            var inputProvider = aircraftGo.AddComponent<KeyboardInputProvider>();
            var controller = aircraftGo.AddComponent<AircraftController>();

            SetFieldValue(inputProvider, "inputChannel", inputChannel);
            SetFieldValue(controller, "defaultAircraft", aircraft);
            SetFieldValue(controller, "inputChannel", inputChannel);
            SetFieldValue(controller, "flightDataChannel", flightDataChannel);

            return aircraftGo;
        }

        private static void WireCamera(GameObject aircraft)
        {
            var mainCamera = GameObject.Find("Main Camera");
            if (mainCamera == null)
            {
                Debug.LogWarning("Main Camera not found in scene.");
                return;
            }

            var cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow == null)
            {
                cameraFollow = mainCamera.AddComponent<CameraFollow>();
            }

            SetFieldValue(cameraFollow, "target", aircraft.transform);
        }

        private static void SetFieldValue(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
            }
        }
    }
}
#endif
