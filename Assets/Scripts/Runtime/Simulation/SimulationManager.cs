using System.Reflection;
using MakerFlightRC.Data;
using MakerFlightRC.Runtime.Aircraft;
using MakerFlightRC.Runtime.CameraRig;
using MakerFlightRC.Runtime.Channels;
using MakerFlightRC.Runtime.Input;
using UnityEngine;

namespace MakerFlightRC.Runtime.Simulation
{
    public class SimulationManager : MonoBehaviour
    {
        [SerializeField] private AircraftSelectionChannelSO aircraftSelectionChannel;
        [SerializeField] private EnvironmentSelectionChannelSO environmentSelectionChannel;

        [SerializeField] private AircraftData defaultAircraft;
        [SerializeField] private EnvironmentData defaultEnvironment;
        [SerializeField] private LevelData defaultLevel;

        private AircraftData currentAircraft;
        private EnvironmentData currentEnvironment;
        private LevelData currentLevel;

        private const string AircraftName = "AircraftController";
        private const string RunwayName = "Pist_Terrain";

        private void Awake()
        {
            EnsureRunway();
            var aircraft = EnsureAircraft();
            WireCamera(aircraft);
        }

        private void Start()
        {
            if (defaultAircraft != null)
            {
                SelectAircraft(defaultAircraft);
            }

            if (defaultEnvironment != null)
            {
                SelectEnvironment(defaultEnvironment);
            }

            if (defaultLevel != null)
            {
                SelectLevel(defaultLevel);
            }
        }

        private void EnsureRunway()
        {
            var runway = GameObject.Find(RunwayName);
            if (runway == null)
            {
                runway = GameObject.CreatePrimitive(PrimitiveType.Plane);
                runway.name = RunwayName;
            }

            runway.transform.position = Vector3.zero;
            runway.transform.rotation = Quaternion.identity;
            runway.transform.localScale = new Vector3(10f, 1f, 10f);

            var meshCollider = runway.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = runway.AddComponent<MeshCollider>();
            }
            meshCollider.convex = false;
        }

        private GameObject EnsureAircraft()
        {
            var aircraft = GameObject.Find(AircraftName);
            if (aircraft == null)
            {
                aircraft = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                aircraft.name = AircraftName;
            }

            aircraft.transform.position = new Vector3(0f, 1f, 0f);
            aircraft.transform.rotation = Quaternion.identity;
            aircraft.transform.localScale = Vector3.one;

            var capsuleCollider = aircraft.GetComponent<Collider>();
            if (capsuleCollider != null)
            {
                Destroy(capsuleCollider);
            }

            var boxCollider = aircraft.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = aircraft.AddComponent<BoxCollider>();
            }
            boxCollider.center = Vector3.zero;
            boxCollider.size = new Vector3(1f, 1f, 2f);

            var rb = aircraft.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = aircraft.AddComponent<Rigidbody>();
            }
            rb.mass = 1.2f;
            rb.useGravity = true;

            if (aircraft.GetComponent<KeyboardInputProvider>() == null)
            {
                aircraft.AddComponent<KeyboardInputProvider>();
            }

            var controller = aircraft.GetComponent<AircraftController>();
            if (controller == null)
            {
                controller = aircraft.AddComponent<AircraftController>();
            }

            SetPrivateField(controller, "defaultAircraft", defaultAircraft);

            return aircraft;
        }

        private void WireCamera(GameObject aircraft)
        {
            if (aircraft == null)
            {
                return;
            }

            var mainCamera = GameObject.Find("Main Camera");
            if (mainCamera == null)
            {
                return;
            }

            var cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow == null)
            {
                cameraFollow = mainCamera.AddComponent<CameraFollow>();
            }

            SetPrivateField(cameraFollow, "target", aircraft.transform);
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            if (target == null)
            {
                return;
            }

            var field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
            }
        }

        public void SelectAircraft(AircraftData data)
        {
            currentAircraft = data;
            if (aircraftSelectionChannel != null)
            {
                aircraftSelectionChannel.Raise(data);
            }
        }

        public void SelectEnvironment(EnvironmentData data)
        {
            currentEnvironment = data;
            if (environmentSelectionChannel != null)
            {
                environmentSelectionChannel.Raise(data);
            }
        }

        public AircraftData GetSelectedAircraft()
        {
            return currentAircraft;
        }

        public EnvironmentData GetSelectedEnvironment()
        {
            return currentEnvironment;
        }

        public void SelectLevel(LevelData data)
        {
            currentLevel = data;
            if (currentLevel != null)
            {
                Physics.gravity = new Vector3(0f, currentLevel.gravity, 0f);
            }
        }

        public LevelData GetSelectedLevel()
        {
            return currentLevel;
        }
    }
}
