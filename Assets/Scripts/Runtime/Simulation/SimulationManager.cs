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
        private const string EnvironmentRootName = "EnvironmentRoot";

        private void Awake()
        {
            EnsureEnvironment();
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

        private void EnsureEnvironment()
        {
            var legacyPlane = GameObject.Find("Plane");
            if (legacyPlane != null)
            {
                Destroy(legacyPlane);
            }

            var environmentRoot = GameObject.Find(EnvironmentRootName);
            if (environmentRoot == null)
            {
                environmentRoot = new GameObject(EnvironmentRootName);
            }

            var runway = GameObject.Find(RunwayName);
            if (runway == null)
            {
                runway = GameObject.CreatePrimitive(PrimitiveType.Plane);
                runway.name = RunwayName;
                runway.transform.SetParent(environmentRoot.transform, false);
            }

            runway.transform.position = Vector3.zero;
            runway.transform.rotation = Quaternion.identity;
            runway.transform.localScale = new Vector3(20f, 1f, 20f);

            var runwayRenderer = runway.GetComponent<MeshRenderer>();
            if (runwayRenderer != null)
            {
                runwayRenderer.sharedMaterial = BuildRunwayMaterial();
            }

            var meshCollider = runway.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = runway.AddComponent<MeshCollider>();
            }
            meshCollider.convex = false;

            var terrainBase = GameObject.Find("Terrain_Base");
            if (terrainBase == null)
            {
                terrainBase = GameObject.CreatePrimitive(PrimitiveType.Plane);
                terrainBase.name = "Terrain_Base";
                terrainBase.transform.SetParent(environmentRoot.transform, false);
                terrainBase.transform.position = new Vector3(0f, -0.01f, 0f);
                terrainBase.transform.localScale = new Vector3(60f, 1f, 60f);

                var terrainRenderer = terrainBase.GetComponent<MeshRenderer>();
                if (terrainRenderer != null)
                {
                    terrainRenderer.sharedMaterial = BuildSolidMaterial(new Color(0.25f, 0.45f, 0.25f));
                }
            }

            EnsureHills(environmentRoot.transform);
            EnsureTrees(environmentRoot.transform);
        }

        private GameObject EnsureAircraft()
        {
            var aircraft = GameObject.Find(AircraftName);
            if (aircraft == null)
            {
                aircraft = new GameObject(AircraftName);
            }

            aircraft.transform.position = new Vector3(0f, 0.5f, 0f);
            aircraft.transform.rotation = Quaternion.identity;
            aircraft.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            var meshRenderer = aircraft.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Destroy(meshRenderer);
            }

            var meshFilter = aircraft.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Destroy(meshFilter);
            }

            foreach (var primitiveCollider in aircraft.GetComponents<Collider>())
            {
                if (primitiveCollider is CapsuleCollider || primitiveCollider is SphereCollider)
                {
                    Destroy(primitiveCollider);
                }
            }

            var boxCollider = aircraft.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = aircraft.AddComponent<BoxCollider>();
            }
            boxCollider.center = Vector3.zero;
            boxCollider.size = new Vector3(2.5f, 0.6f, 3.0f);

            var rb = aircraft.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = aircraft.AddComponent<Rigidbody>();
            }
            rb.mass = 1.5f;
            rb.drag = 1.0f;
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

            BuildTrainerModel(aircraft.transform);

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

            // Set Main Camera position and rotation
            mainCamera.transform.position = new Vector3(0f, 3f, -10f);
            mainCamera.transform.rotation = Quaternion.Euler(15f, 0f, 0f);

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

        private static Material BuildRunwayMaterial()
        {
            var material = BuildSolidMaterial(new Color(0.35f, 0.35f, 0.35f));
            var texture = new Texture2D(256, 256, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Bilinear
            };

            var baseColor = new Color(0.32f, 0.32f, 0.32f);
            var lineColor = new Color(0.95f, 0.95f, 0.95f);
            for (var y = 0; y < texture.height; y++)
            {
                for (var x = 0; x < texture.width; x++)
                {
                    var isEdge = x < 6 || x > texture.width - 7;
                    var isCenterLine = Mathf.Abs(x - texture.width / 2) < 3 && (y / 16) % 2 == 0;
                    var color = (isEdge || isCenterLine) ? lineColor : baseColor;
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            material.mainTexture = texture;
            material.mainTextureScale = new Vector2(6f, 12f);
            return material;
        }

        private static Material BuildSolidMaterial(Color color)
        {
            var material = new Material(Shader.Find("Standard"));
            material.color = color;
            return material;
        }

        private static void EnsureHills(Transform parent)
        {
            if (parent.Find("Hills") != null)
            {
                return;
            }

            var hillsRoot = new GameObject("Hills");
            hillsRoot.transform.SetParent(parent, false);

            CreateHill(hillsRoot.transform, new Vector3(18f, 0f, 22f), new Vector3(8f, 4f, 8f));
            CreateHill(hillsRoot.transform, new Vector3(-22f, 0f, -15f), new Vector3(10f, 5f, 10f));
            CreateHill(hillsRoot.transform, new Vector3(25f, 0f, -20f), new Vector3(6f, 3f, 6f));
        }

        private static void CreateHill(Transform parent, Vector3 position, Vector3 scale)
        {
            var hill = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hill.name = "Hill";
            hill.transform.SetParent(parent, false);
            hill.transform.position = position;
            hill.transform.localScale = scale;

            var renderer = hill.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = BuildSolidMaterial(new Color(0.2f, 0.35f, 0.2f));
            }
        }

        private static void EnsureTrees(Transform parent)
        {
            if (parent.Find("Trees") != null)
            {
                return;
            }

            var treesRoot = new GameObject("Trees");
            treesRoot.transform.SetParent(parent, false);

            CreateTree(treesRoot.transform, new Vector3(12f, 0f, 12f));
            CreateTree(treesRoot.transform, new Vector3(-10f, 0f, 16f));
            CreateTree(treesRoot.transform, new Vector3(14f, 0f, -18f));
            CreateTree(treesRoot.transform, new Vector3(-16f, 0f, -14f));
            CreateTree(treesRoot.transform, new Vector3(20f, 0f, 6f));
        }

        private static void CreateTree(Transform parent, Vector3 position)
        {
            var treeRoot = new GameObject("Tree");
            treeRoot.transform.SetParent(parent, false);
            treeRoot.transform.position = position;

            var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(treeRoot.transform, false);
            trunk.transform.localPosition = new Vector3(0f, 1f, 0f);
            trunk.transform.localScale = new Vector3(0.4f, 1f, 0.4f);
            var trunkRenderer = trunk.GetComponent<MeshRenderer>();
            if (trunkRenderer != null)
            {
                trunkRenderer.sharedMaterial = BuildSolidMaterial(new Color(0.35f, 0.25f, 0.15f));
            }

            var canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            canopy.name = "Canopy";
            canopy.transform.SetParent(treeRoot.transform, false);
            canopy.transform.localPosition = new Vector3(0f, 2.4f, 0f);
            canopy.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
            var canopyRenderer = canopy.GetComponent<MeshRenderer>();
            if (canopyRenderer != null)
            {
                canopyRenderer.sharedMaterial = BuildSolidMaterial(new Color(0.2f, 0.5f, 0.2f));
            }
        }

        private static void BuildTrainerModel(Transform aircraftRoot)
        {
            if (aircraftRoot.Find("ModelRoot") != null)
            {
                return;
            }

            var modelRoot = new GameObject("ModelRoot").transform;
            modelRoot.SetParent(aircraftRoot, false);

            var bodyMat = BuildSolidMaterial(new Color(0.8f, 0.1f, 0.1f));
            var wingMat = BuildSolidMaterial(new Color(0.9f, 0.9f, 0.9f));

            var fuselage = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fuselage.name = "Fuselage";
            fuselage.transform.SetParent(modelRoot, false);
            fuselage.transform.localScale = new Vector3(1.2f, 0.3f, 2.6f);
            SetRendererMaterial(fuselage, bodyMat);

            var wing = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wing.name = "Wing";
            wing.transform.SetParent(modelRoot, false);
            wing.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            wing.transform.localScale = new Vector3(3.4f, 0.1f, 0.7f);
            SetRendererMaterial(wing, wingMat);

            var tail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tail.name = "Tail";
            tail.transform.SetParent(modelRoot, false);
            tail.transform.localPosition = new Vector3(0f, 0.1f, -1.1f);
            tail.transform.localScale = new Vector3(1.2f, 0.1f, 0.4f);
            SetRendererMaterial(tail, wingMat);

            var verticalTail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            verticalTail.name = "VerticalTail";
            verticalTail.transform.SetParent(modelRoot, false);
            verticalTail.transform.localPosition = new Vector3(0f, 0.45f, -1.2f);
            verticalTail.transform.localScale = new Vector3(0.2f, 0.8f, 0.3f);
            SetRendererMaterial(verticalTail, wingMat);

            var prop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            prop.name = "Propeller";
            prop.transform.SetParent(modelRoot, false);
            prop.transform.localPosition = new Vector3(0f, 0f, 1.4f);
            prop.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            prop.transform.localScale = new Vector3(0.1f, 0.8f, 0.1f);
            SetRendererMaterial(prop, BuildSolidMaterial(new Color(0.1f, 0.1f, 0.1f)));
        }

        private static void SetRendererMaterial(GameObject obj, Material material)
        {
            var renderer = obj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
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
