#if UNITY_EDITOR
using MakerFlightRC.Runtime.Aircraft;
using MakerFlightRC.Runtime.Input;
using UnityEditor;
using UnityEngine;

namespace MakerFlightRC.EditorTools
{
    /// <summary>
    /// Helper utility for setting up new aircraft models in the scene.
    /// Automates wiring of AircraftController components and collider configuration.
    /// </summary>
    public static class AircraftSetupHelper
    {
        private const string DefaultAircraftPath = "Assets/Data/Default_Aircraft.asset";
        private const string InputChannelPath = "Assets/Channels/InputChannel.asset";
        private const string FlightDataChannelPath = "Assets/Channels/FlightDataChannel.asset";

        [MenuItem("MakerFlight RC/Aircraft Setup/Wire Selected Aircraft")]
        public static void WireSelectedAircraft()
        {
            var selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("Setup Failed", "Please select an AircraftController GameObject", "OK");
                return;
            }

            WireAircraftComponents(selected);
            EditorUtility.DisplayDialog("Setup Complete", "Aircraft components have been wired successfully", "OK");
        }

        [MenuItem("MakerFlight RC/Aircraft Setup/Add Capsule Collider")]
        public static void AddCapsuleCollider()
        {
            var selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("Setup Failed", "Please select an AircraftController GameObject", "OK");
                return;
            }

            var collider = selected.GetComponent<CapsuleCollider>();
            if (collider == null)
            {
                collider = selected.AddComponent<CapsuleCollider>();
            }

            collider.radius = 0.5f;
            collider.height = 2f;
            collider.center = Vector3.zero;

            EditorUtility.DisplayDialog("Collider Added", "Capsule Collider configured for aircraft", "OK");
        }

        [MenuItem("MakerFlight RC/Aircraft Setup/Add Box Collider")]
        public static void AddBoxCollider()
        {
            var selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("Setup Failed", "Please select a GameObject", "OK");
                return;
            }

            var collider = selected.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = selected.AddComponent<BoxCollider>();
            }

            collider.size = new Vector3(1f, 1f, 2f);
            collider.center = Vector3.zero;

            EditorUtility.DisplayDialog("Collider Added", "Box Collider configured", "OK");
        }

        private static void WireAircraftComponents(GameObject aircraftGo)
        {
            // Load asset references
            var defaultAircraft = AssetDatabase.LoadAssetAtPath<Data.AircraftData>(DefaultAircraftPath);
            var inputChannel = AssetDatabase.LoadAssetAtPath<Runtime.Channels.InputChannelSO>(InputChannelPath);
            var flightDataChannel = AssetDatabase.LoadAssetAtPath<Runtime.Channels.FlightDataChannelSO>(FlightDataChannelPath);

            // Ensure Rigidbody exists
            var rb = aircraftGo.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = aircraftGo.AddComponent<Rigidbody>();
                rb.mass = 1.2f;
                rb.useGravity = true;
            }

            // Ensure KeyboardInputProvider exists
            var inputProvider = aircraftGo.GetComponent<KeyboardInputProvider>();
            if (inputProvider == null)
            {
                inputProvider = aircraftGo.AddComponent<KeyboardInputProvider>();
            }

            // Ensure AircraftController exists
            var controller = aircraftGo.GetComponent<AircraftController>();
            if (controller == null)
            {
                controller = aircraftGo.AddComponent<AircraftController>();
            }

            // Wire up all references
            SetFieldValue(inputProvider, "inputChannel", inputChannel);
            SetFieldValue(controller, "defaultAircraft", defaultAircraft);
            SetFieldValue(controller, "inputChannel", inputChannel);
            SetFieldValue(controller, "flightDataChannel", flightDataChannel);

            Debug.Log("Aircraft components wired successfully: " + aircraftGo.name);
        }

        private static void SetFieldValue(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(
                fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(target, value);
            }
            else
            {
                Debug.LogWarning($"Field '{fieldName}' not found on {target.GetType().Name}");
            }
        }
    }
}
#endif
