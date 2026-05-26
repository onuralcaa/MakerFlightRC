using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#if UNITY_EDITOR
namespace MakerFlightRC.EditorTools
{
    public class CleanupAircraftScene
    {
        [MenuItem("MakerFlightRC/Cleanup/Clean Simulation_Bootstrap Scene")]
        public static void CleanSimulationBootstrapScene()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid() || !scene.name.Contains("Simulation_Bootstrap"))
            {
                EditorUtility.DisplayDialog("Error", "Please open Simulation_Bootstrap scene first.", "OK");
                return;
            }

            var rootObjects = scene.GetRootGameObjects();
            int removed = 0;

            foreach (var root in rootObjects)
            {
                removed += CleanGameObjectRecursive(root);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            EditorUtility.DisplayDialog("Success", 
                $"Cleaned {removed} items from scene.\n" +
                $"Removed all AircraftPropellerVisual and missing components.\n" +
                $"Scene saved.", 
                "OK");
        }

        private static int CleanGameObjectRecursive(GameObject go)
        {
            int count = 0;

            // Remove all AircraftPropellerVisual children
            var propellers = go.transform.Find("AircraftPropellerVisual");
            if (propellers != null)
            {
                Object.DestroyImmediate(propellers.gameObject, true);
                count++;
            }

            // Remove all missing MonoBehaviour components
            var components = go.GetComponents<Component>();
            foreach (var comp in components)
            {
                if (comp == null)
                {
                    Object.DestroyImmediate(comp, true);
                    count++;
                }
            }

            // Recursively clean children
            foreach (Transform child in go.transform)
            {
                count += CleanGameObjectRecursive(child.gameObject);
            }

            return count;
        }
    }
}
#endif
