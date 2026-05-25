#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MakerFlightRC.EditorTools
{
    [InitializeOnLoad]
    public static class AutoPopulateBootstrapSceneOnLoad
    {
        private const string ScenePath = "Assets/Scenes/Simulation_Bootstrap.unity";
        private const string PrefKey = "MakerFlightRC_BootstrapPopulated";

        static AutoPopulateBootstrapSceneOnLoad()
        {
            EditorApplication.delayCall += TryPopulate;
        }

        private static void TryPopulate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            var scene = SceneManager.GetActiveScene();
            if (scene.path != ScenePath)
            {
                return;
            }

            if (EditorPrefs.GetBool(PrefKey, false))
            {
                return;
            }

            if (HasVisibleGeometry(scene))
            {
                EditorPrefs.SetBool(PrefKey, true);
                return;
            }

            PopulateBootstrapScene.PopulateScene();
            EditorPrefs.SetBool(PrefKey, true);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        private static bool HasVisibleGeometry(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.GetComponentInChildren<MeshRenderer>(true) != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
#endif
