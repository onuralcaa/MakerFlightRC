using UnityEngine;

namespace MakerFlightRC.Data
{
    [CreateAssetMenu(menuName = "MakerFlight RC/Data/Level")]
    public class LevelData : ScriptableObject
    {
        public string levelId = "level";
        public string displayName = "Level";
        public string sceneName = "";
        public Vector3 spawnPosition = Vector3.zero;
        public Vector3 spawnEuler = Vector3.zero;
    }
}
