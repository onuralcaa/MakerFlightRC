#if UNITY_EDITOR
using System.Reflection;
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
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;

namespace MakerFlightRC.EditorTools
{
    /// <summary>
    /// Builds the bootstrap scene using only free Unity packages (ProBuilder, Terrain Tools)
    /// and the legacy Standard Assets propeller aircraft (visual mesh only).
    /// </summary>
    public static class FreeInternalSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Simulation_Bootstrap.unity";
        private const string AircraftPrefabPath = "Assets/StandardAssets/Vehicles/Aircraft/Prefabs/AircraftPropeller.prefab";
        private const string TrackMaterialPath = "Assets/Art/Materials/PistTrack.mat";
        private const string TrackTexturePath = "Assets/Art/Textures/PistTrackTexture.asset";

        private const string DefaultAircraftPath = "Assets/Data/Default_Aircraft.asset";
        private const string WindyDayPath = "Assets/Data/Windy_Day.asset";
        private const string MainAirfieldPath = "Assets/Data/Main_Airfield.asset";
        private const string InputChannelPath = "Assets/Channels/InputChannel.asset";
        private const string FlightDataChannelPath = "Assets/Channels/FlightDataChannel.asset";
        private const string EnvironmentStateChannelPath = "Assets/Channels/EnvironmentStateChannel.asset";
        private const string AircraftSelectionChannelPath = "Assets/Channels/AircraftSelectionChannel.asset";
        private const string AircraftConfigChannelPath = "Assets/Channels/AircraftConfigChannel.asset";

        private const float TerrainSize = 256f;
        private const float TerrainHeight = 40f;
        private const float RunwayLength = 120f;
        private const float RunwayWidth = 24f;

        [MenuItem("MakerFlight RC/Bootstrap/Build Free Internal Visualization")]
        public static void BuildFromMenu()
        {
            BuildAndSave();
        }

        /// <summary>Unity batchmode entry: -executeMethod MakerFlightRC.EditorTools.FreeInternalSceneBuilder.ExecuteBatch</summary>
        public static void ExecuteBatch()
        {
            BuildAndSave();
            EditorApplication.Exit(0);
        }

        public static void BuildAndSave()
        {
            EnsureFolders();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            }

            ClearSceneRoots();
            BuildSceneContent();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("Free internal visualization scene built and saved.");
        }

        private static void EnsureFolders()
        {
            CreateFolderChain("Assets/Art/Materials");
            CreateFolderChain("Assets/Art/Textures");
            CreateFolderChain("Assets/Scenes");
        }

        private static void CreateFolderChain(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parts = path.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }
                current = next;
            }
        }

        private static void ClearSceneRoots()
        {
            var scene = SceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                UnityEngine.Object.DestroyImmediate(root);
            }
        }

        private static void BuildSceneContent()
        {
            var defaultAircraft = LoadAsset<AircraftData>(DefaultAircraftPath);
            var windyDay = LoadAsset<EnvironmentData>(WindyDayPath);
            var mainAirfield = LoadAsset<LevelData>(MainAirfieldPath);
            var inputChannel = LoadAsset<InputChannelSO>(InputChannelPath);
            var flightDataChannel = LoadAsset<FlightDataChannelSO>(FlightDataChannelPath);
            var environmentStateChannel = LoadAsset<EnvironmentStateChannelSO>(EnvironmentStateChannelPath);
            var aircraftSelectionChannel = LoadAsset<AircraftSelectionChannelSO>(AircraftSelectionChannelPath);
            var aircraftConfigChannel = LoadAsset<AircraftConfigChannelSO>(AircraftConfigChannelPath);

            var trackMaterial = EnsureTrackMaterial(out var trackTexture);
            var environmentRoot = new GameObject("[ENVIRONMENT]");
            CreateCenteredTerrain(environmentRoot.transform, trackMaterial, trackTexture);
            CreateProBuilderRunway(environmentRoot.transform, trackMaterial);

            var managers = new GameObject("[MANAGERS]");
            var simulationManager = managers.AddComponent<SimulationManager>();
            var environmentManager = managers.AddComponent<EnvironmentManager>();
            SetField(simulationManager, "aircraftSelectionChannel", aircraftSelectionChannel);
            SetField(simulationManager, "defaultAircraft", defaultAircraft);
            SetField(simulationManager, "defaultEnvironment", windyDay);
            SetField(simulationManager, "defaultLevel", mainAirfield);
            SetField(environmentManager, "environmentStateChannel", environmentStateChannel);
            SetField(environmentManager, "defaultEnvironment", windyDay);

            var spawnerRoot = new GameObject("[AIRCRAFT_SPAWNER]");
            var aircraft = CreateAircraftRoot(
                defaultAircraft,
                inputChannel,
                flightDataChannel,
                environmentStateChannel,
                aircraftSelectionChannel,
                aircraftConfigChannel);
            aircraft.transform.SetParent(spawnerRoot.transform, false);

            CreateMainCamera(aircraft.transform);
            CreateDirectionalLight();
        }

        private static T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                Debug.LogWarning($"Missing asset at {path}");
            }
            return asset;
        }

        private static Material EnsureTrackMaterial(out Texture2D trackTexture)
        {
            trackTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(TrackTexturePath);
            if (trackTexture == null)
            {
                trackTexture = CreateRunwayTexture();
                AssetDatabase.CreateAsset(trackTexture, TrackTexturePath);
            }

            var material = AssetDatabase.LoadAssetAtPath<Material>(TrackMaterialPath);
            if (material == null)
            {
                material = new Material(Shader.Find("Standard"))
                {
                    name = "PistTrack",
                    mainTexture = trackTexture,
                    mainTextureScale = new Vector2(8f, 32f),
                    color = new Color(0.35f, 0.35f, 0.38f)
                };
                AssetDatabase.CreateAsset(material, TrackMaterialPath);
            }
            else
            {
                material.mainTexture = trackTexture;
            }

            return material;
        }

        private static Texture2D CreateRunwayTexture()
        {
            const int width = 128;
            const int height = 512;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = "PistTrackTexture",
                wrapMode = TextureWrapMode.Repeat,
                filterMode = FilterMode.Bilinear
            };

            var grass = new Color(0.22f, 0.42f, 0.18f);
            var asphalt = new Color(0.28f, 0.28f, 0.30f);
            var stripe = new Color(0.95f, 0.95f, 0.95f);

            for (var y = 0; y < height; y++)
            {
                var onRunway = y > height * 0.2f && y < height * 0.8f;
                for (var x = 0; x < width; x++)
                {
                    var color = onRunway ? asphalt : grass;
                    if (onRunway)
                    {
                        var centerLine = Mathf.Abs(x - width * 0.5f) < 1.5f && (y % 28) < 14;
                        var edgeLine = x < 4 || x > width - 5;
                        if (centerLine || edgeLine)
                        {
                            color = stripe;
                        }
                    }

                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        private static void CreateCenteredTerrain(Transform parent, Material trackMaterial, Texture2D trackTexture)
        {
            var terrainData = new TerrainData
            {
                heightmapResolution = 513,
                alphamapResolution = 512,
                baseMapResolution = 1024,
                size = new Vector3(TerrainSize, TerrainHeight, TerrainSize)
            };

            var heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
            var center = terrainData.heightmapResolution * 0.5f;
            var runwayRadiusX = (RunwayWidth / TerrainSize) * terrainData.heightmapResolution * 0.5f;
            var runwayRadiusZ = (RunwayLength / TerrainSize) * terrainData.heightmapResolution * 0.5f;

            for (var z = 0; z < terrainData.heightmapResolution; z++)
            {
                for (var x = 0; x < terrainData.heightmapResolution; x++)
                {
                    var dx = (x - center) / runwayRadiusX;
                    var dz = (z - center) / runwayRadiusZ;
                    var bowl = Mathf.Exp(-(dx * dx + dz * dz) * 0.35f) * 0.02f;
                    heights[z, x] = bowl;
                }
            }

            terrainData.SetHeights(0, 0, heights);

            var layer = new TerrainLayer
            {
                diffuseTexture = trackTexture,
                tileSize = new Vector2(32f, 32f),
                smoothness = 0.15f,
                metallic = 0f
            };
            terrainData.terrainLayers = new[] { layer };
            terrainData.SetAlphamaps(0, 0, CreateRunwayAlphamap(terrainData, center, runwayRadiusX, runwayRadiusZ));

            var terrainObject = Terrain.CreateTerrainGameObject(terrainData);
            terrainObject.name = "Pist_Terrain";
            terrainObject.transform.SetParent(parent, false);
            terrainObject.transform.position = new Vector3(-TerrainSize * 0.5f, 0f, -TerrainSize * 0.5f);

            var terrainCollider = terrainObject.GetComponent<TerrainCollider>();
            if (terrainCollider != null)
            {
                terrainCollider.terrainData = terrainData;
            }
        }

        private static float[,,] CreateRunwayAlphamap(
            TerrainData terrainData,
            float center,
            float runwayRadiusX,
            float runwayRadiusZ)
        {
            var mapWidth = terrainData.alphamapWidth;
            var mapHeight = terrainData.alphamapHeight;
            var alphamaps = new float[mapHeight, mapWidth, 1];

            for (var z = 0; z < mapHeight; z++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    var dx = (x - center) / runwayRadiusX;
                    var dz = (z - center) / runwayRadiusZ;
                    var onRunway = dx * dx + dz * dz <= 1f;
                    alphamaps[z, x, 0] = onRunway ? 1f : 0.15f;
                }
            }

            return alphamaps;
        }

        private static void CreateProBuilderRunway(Transform parent, Material trackMaterial)
        {
            var runway = ShapeGenerator.GeneratePlane(PivotLocation.Center, RunwayWidth, RunwayLength, 0, 1, Axis.Up);
            runway.name = "Pist_Runway_ProBuilder";
            runway.transform.SetParent(parent, false);
            runway.transform.localPosition = new Vector3(0f, 0.08f, 0f);
            runway.transform.localRotation = Quaternion.identity;

            var renderer = runway.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = trackMaterial;
            }

            var meshCollider = runway.gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = runway.gameObject.AddComponent<MeshCollider>();
            }

            meshCollider.sharedMesh = runway.GetComponent<MeshFilter>().sharedMesh;
            meshCollider.convex = false;
        }

        private static GameObject CreateAircraftRoot(
            AircraftData defaultAircraft,
            InputChannelSO inputChannel,
            FlightDataChannelSO flightDataChannel,
            EnvironmentStateChannelSO environmentStateChannel,
            AircraftSelectionChannelSO aircraftSelectionChannel,
            AircraftConfigChannelSO aircraftConfigChannel)
        {
            var aircraftGo = new GameObject("AircraftController");
            aircraftGo.transform.position = new Vector3(0f, 1.2f, 0f);
            aircraftGo.transform.rotation = Quaternion.identity;

            var rb = aircraftGo.AddComponent<Rigidbody>();
            rb.mass = 1.2f;
            rb.useGravity = true;
            rb.drag = 1.5f;
            rb.angularDrag = 2f;

            var boxCollider = aircraftGo.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0f, 0.35f, 0f);
            boxCollider.size = new Vector3(2.4f, 1.4f, 4.8f);

            var inputProvider = aircraftGo.AddComponent<KeyboardInputProvider>();
            var controller = aircraftGo.AddComponent<AircraftController>();

            SetField(inputProvider, "inputChannel", inputChannel);
            SetField(controller, "aircraftSelectionChannel", aircraftSelectionChannel);
            SetField(controller, "aircraftConfigChannel", aircraftConfigChannel);
            SetField(controller, "environmentStateChannel", environmentStateChannel);
            SetField(controller, "inputChannel", inputChannel);
            SetField(controller, "flightDataChannel", flightDataChannel);
            SetField(controller, "inputProvider", inputProvider);
            SetField(controller, "defaultAircraft", defaultAircraft);

            AttachStandardAircraftVisual(aircraftGo.transform, boxCollider);

            return aircraftGo;
        }

        private static void AttachStandardAircraftVisual(Transform aircraftRoot, BoxCollider boxCollider)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AircraftPrefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Aircraft prefab not found at {AircraftPrefabPath}. Skipping Standard Assets visual attachment. SimulationManager will provide primitive visuals.");
                return;
            }

            var modelInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (modelInstance == null)
            {
                Debug.LogWarning("Failed to instantiate Standard Assets aircraft prefab. Skipping attachment.");
                return;
            }

            modelInstance.name = "AircraftPropellerVisual";
            modelInstance.transform.SetParent(aircraftRoot, false);
            modelInstance.transform.localPosition = Vector3.zero;
            modelInstance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            modelInstance.transform.localScale = Vector3.one;

            StripNonVisualComponents(modelInstance);

            var renderers = modelInstance.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                var bounds = renderers[0].bounds;
                for (var i = 1; i < renderers.Length; i++)
                {
                    bounds.Encapsulate(renderers[i].bounds);
                }

                var localCenter = aircraftRoot.InverseTransformPoint(bounds.center);
                var localSize = bounds.size;
                boxCollider.center = localCenter;
                boxCollider.size = new Vector3(
                    Mathf.Max(localSize.x, 1.5f),
                    Mathf.Max(localSize.y, 1f),
                    Mathf.Max(localSize.z, 2f));
            }
        }

        private static void StripNonVisualComponents(GameObject root)
        {
            var components = root.GetComponents<Component>();
            for (var i = components.Length - 1; i >= 0; i--)
            {
                var component = components[i];
                if (component == null)
                {
                    continue;
                }

                if (component is Transform
                    || component is MeshFilter
                    || component is MeshRenderer
                    || component is SkinnedMeshRenderer
                    || component is Animator)
                {
                    continue;
                }

                UnityEngine.Object.DestroyImmediate(component);
            }

            for (var c = 0; c < root.transform.childCount; c++)
            {
                StripNonVisualComponents(root.transform.GetChild(c).gameObject);
            }
        }

        private static void CreateMainCamera(Transform aircraftTarget)
        {
            var mainCamera = new GameObject("Main Camera");
            mainCamera.tag = "MainCamera";
            var camera = mainCamera.AddComponent<Camera>();
            mainCamera.AddComponent<AudioListener>();
            var cameraFollow = mainCamera.AddComponent<CameraFollow>();
            SetField(cameraFollow, "target", aircraftTarget);
            mainCamera.transform.position = new Vector3(0f, 3f, -10f);
            mainCamera.transform.rotation = Quaternion.Euler(10f, 0f, 0f);
            camera.fieldOfView = 60f;
        }

        private static void CreateDirectionalLight()
        {
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.shadows = LightShadows.Soft;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void SetField(object target, string fieldName, object value)
        {
            if (target == null)
            {
                return;
            }

            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(target, value);
        }
    }
}
#endif
