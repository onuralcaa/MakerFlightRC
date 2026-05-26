using MakerFlightRC.Data;
using UnityEditor;
using UnityEngine;

namespace MakerFlightRC.Editor
{
    public class ConfigureDefaultAircraft
    {
        [MenuItem("MakerFlightRC/Configure Default Aircraft")]
        public static void ConfigureAircraft()
        {
            const string assetPath = "Assets/Data/Default_Aircraft.asset";
            var aircraft = AssetDatabase.LoadAssetAtPath<AircraftData>(assetPath);

            if (aircraft == null)
            {
                EditorUtility.DisplayDialog("Error", $"Could not load aircraft data from {assetPath}", "OK");
                return;
            }

            // Set baseThrust to 10 for gentler acceleration
            aircraft.baseThrust = 10f;

            // Set baseMass to 1.0 for better stability
            aircraft.baseMass = 1.0f;

            // Reduce lift coefficient to prevent vertical takeoff
            // Assuming liftCoefficient exists; adjust if different field name
            aircraft.liftCoefficient = 0.3f;

            // Reduce drag coefficient if necessary for better control
            aircraft.dragCoefficient = 0.05f;

            EditorUtility.SetDirty(aircraft);
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Success", 
                "Default Aircraft configured:\n" +
                "- baseThrust = 10\n" +
                "- baseMass = 1.0\n" +
                "- liftCoefficient = 0.3\n" +
                "- dragCoefficient = 0.05", 
                "OK");
        }
    }
}
