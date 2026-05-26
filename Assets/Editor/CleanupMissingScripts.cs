using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#if UNITY_EDITOR
namespace MakerFlightRC.EditorTools
{
    /// <summary>
    /// Cleans up missing script components in the scene, particularly for AircraftPropellerVisual
    /// which may have lingering Missing (MonoBehaviour) components.
    /// </summary>
    public class CleanupMissingScripts
    {
        [MenuItem("MakerFlightRC/Cleanup/Remove Missing Scripts")]
        public static void RemoveMissingScriptsFromScene()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                EditorUtility.DisplayDialog("Error", "No active scene found.", "OK");
                return;
            }

            var rootObjects = scene.GetRootGameObjects();
            int removedCount = 0;

            foreach (var root in rootObjects)
            {
                removedCount += RemoveMissingScriptsRecursive(root);
            }

            EditorSceneManager.SaveScene(scene);
            EditorUtility.DisplayDialog("Success", 
                $"Removed {removedCount} missing script component(s) from scene.\nScene saved.", 
                "OK");
        }

        private static int RemoveMissingScriptsRecursive(GameObject go)
        {
            int count = 0;

            // Remove missing scripts from this GameObject
            var components = go.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component == null)
                {
                    // This is a missing script; destroy it
                    Object.DestroyImmediate(component, true);
                    count++;
                }
            }

            // Recursively process children
            foreach (Transform child in go.transform)
            {
                count += RemoveMissingScriptsRecursive(child.gameObject);
            }

            return count;
        }

        [MenuItem("MakerFlightRC/Cleanup/Remove AircraftPropellerVisual GameObject")]
        public static void RemoveAircraftPropellerVisual()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                EditorUtility.DisplayDialog("Error", "No active scene found.", "OK");
                return;
            }

            var rootObjects = scene.GetRootGameObjects();
            var propellerVisual = FindGameObjectByName(rootObjects, "AircraftPropellerVisual");

            if (propellerVisual == null)
            {
                EditorUtility.DisplayDialog("Info", "AircraftPropellerVisual not found in scene.", "OK");
                return;
            }

            // Destroy the GameObject entirely
            Object.DestroyImmediate(propellerVisual, true);
            EditorSceneManager.SaveScene(scene);
            EditorUtility.DisplayDialog("Success", 
                "AircraftPropellerVisual removed from scene and saved.", 
                "OK");
        }

        private static GameObject FindGameObjectByName(GameObject[] roots, string name)
        {
            foreach (var root in roots)
            {
                var result = FindGameObjectByNameRecursive(root, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private static GameObject FindGameObjectByNameRecursive(GameObject go, string name)
        {
            if (go.name == name)
            {
                return go;
            }

            foreach (Transform child in go.transform)
            {
                var result = FindGameObjectByNameRecursive(child.gameObject, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
#endif
