#if UNITY_EDITOR
using MakerFlightRC.Data;
using MakerFlightRC.Runtime.Aircraft;
using MakerFlightRC.Runtime.CameraRig;
using MakerFlightRC.Runtime.Channels;
using MakerFlightRC.Runtime.Environment;
using MakerFlightRC.Runtime.Input;
using MakerFlightRC.Runtime.Simulation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakerFlightRC.EditorTools
{
    public static class SimulationBootstrapSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Simulation_Bootstrap.unity";
        private const string DefaultAircraftPath = "Assets/Data/Default_Aircraft.asset";
        private const string WindyDayPath = "Assets/Data/Windy_Day.asset";
        private const string MainAirfieldPath = "Assets/Data/Main_Airfield.asset";
        private const string InputChannelPath = "Assets/Channels/InputChannel.asset";
        private const string FlightDataChannelPath = "Assets/Channels/FlightDataChannel.asset";
        private const string EnvironmentStateChannelPath = "Assets/Channels/EnvironmentStateChannel.asset";

        [MenuItem("MakerFlight RC/Bootstrap/Create Simulation Bootstrap Scene")]
        public static void CreateScene()
        {
            EnsureSceneFolder();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Simulation_Bootstrap";

            var defaultAircraft = AssetDatabase.LoadAssetAtPath<AircraftData>(DefaultAircraftPath);
            var windyDay = AssetDatabase.LoadAssetAtPath<EnvironmentData>(WindyDayPath);
            var mainAirfield = AssetDatabase.LoadAssetAtPath<LevelData>(MainAirfieldPath);
            var inputChannel = AssetDatabase.LoadAssetAtPath<InputChannelSO>(InputChannelPath);
            var flightDataChannel = AssetDatabase.LoadAssetAtPath<FlightDataChannelSO>(FlightDataChannelPath);
            var environmentStateChannel = AssetDatabase.LoadAssetAtPath<EnvironmentStateChannelSO>(EnvironmentStateChannelPath);

            var managers = new GameObject("[MANAGERS]");
            var simulationManager = managers.AddComponent<SimulationManager>();
            var environmentManager = managers.AddComponent<EnvironmentManager>();
            simulationManager.GetType().GetField("defaultAircraft", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(simulationManager, defaultAircraft);
            simulationManager.GetType().GetField("defaultEnvironment", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(simulationManager, windyDay);
            simulationManager.GetType().GetField("defaultLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(simulationManager, mainAirfield);
            environmentManager.GetType().GetField("defaultEnvironment", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(environmentManager, windyDay);
            environmentManager.GetType().GetField("environmentStateChannel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(environmentManager, environmentStateChannel);

            var environmentRoot = new GameObject("[ENVIRONMENT]");
            var runway = GameObject.CreatePrimitive(PrimitiveType.Plane);
            runway.name = "Plane";
            runway.transform.SetParent(environmentRoot.transform, false);
            runway.transform.localPosition = Vector3.zero;
            runway.transform.localRotation = Quaternion.identity;
            runway.transform.localScale = Vector3.one;

            var spawnerRoot = new GameObject("[AIRCRAFT_SPAWNER]");
            var aircraftObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            aircraftObject.name = "AircraftController";
            aircraftObject.transform.SetParent(spawnerRoot.transform, false);
            aircraftObject.transform.localPosition = new Vector3(0f, 1f, 0f);
            aircraftObject.transform.localRotation = Quaternion.identity;

            var rigidbody = aircraftObject.AddComponent<Rigidbody>();
            rigidbody.mass = 1.2f;
            var inputProvider = aircraftObject.AddComponent<KeyboardInputProvider>();
            var aircraftController = aircraftObject.AddComponent<AircraftController>();
            inputProvider.GetType().GetField("inputChannel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(inputProvider, inputChannel);
            aircraftController.GetType().GetField("defaultAircraft", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(aircraftController, defaultAircraft);
            aircraftController.GetType().GetField("environmentStateChannel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(aircraftController, environmentStateChannel);
            aircraftController.GetType().GetField("inputChannel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(aircraftController, inputChannel);
            aircraftController.GetType().GetField("flightDataChannel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(aircraftController, flightDataChannel);
            aircraftController.GetType().GetField("inputProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(aircraftController, null);

            var mainCamera = new GameObject("Main Camera");
            var camera = mainCamera.AddComponent<Camera>();
            mainCamera.AddComponent<AudioListener>();
            var cameraFollow = mainCamera.AddComponent<CameraFollow>();
            cameraFollow.GetType().GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(cameraFollow, aircraftObject.transform);
            mainCamera.transform.position = new Vector3(0f, 3f, -7f);
            mainCamera.transform.rotation = Quaternion.Euler(15f, 0f, 0f);
            camera.fieldOfView = 60f;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettingsScene[] scenes = { new EditorBuildSettingsScene(ScenePath, true) };
            EditorBuildSettings.scenes = scenes;
            EditorSceneManager.OpenScene(ScenePath);
            Selection.activeGameObject = aircraftObject;
        }

        private static void EnsureSceneFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
        }
    }
}
#endif
