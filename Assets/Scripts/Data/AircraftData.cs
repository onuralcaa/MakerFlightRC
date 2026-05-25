using UnityEngine;

namespace MakerFlightRC.Data
{
    [CreateAssetMenu(menuName = "MakerFlight RC/Data/Aircraft")]
    public class AircraftData : ScriptableObject
    {
        public string aircraftId = "default";
        public string displayName = "Aircraft";
        public GameObject prefab;

        [Header("Physics")]
        public float baseMass = 2.5f;
        public float baseWingArea = 0.8f;
        public float baseWingSpan = 1.2f;
        public float baseThrust = 20f;
        public Vector3 baseCenterOfMass = Vector3.zero;

        [Header("Aerodynamics")]
        public float liftCoefficient = 1.1f;
        public float dragCoefficient = 0.05f;

        [Header("Control")]
        public Vector3 controlTorque = new Vector3(8f, 4f, 6f);

        private void OnValidate()
        {
            baseMass = Mathf.Max(0.1f, baseMass);
            baseWingArea = Mathf.Max(0.05f, baseWingArea);
            baseWingSpan = Mathf.Max(0.1f, baseWingSpan);
            baseThrust = Mathf.Max(0f, baseThrust);
            liftCoefficient = Mathf.Max(0f, liftCoefficient);
            dragCoefficient = Mathf.Max(0f, dragCoefficient);
        }
    }
}
