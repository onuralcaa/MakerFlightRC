using System;
using UnityEngine;

namespace MakerFlightRC.Data
{
    [Serializable]
    public struct TurbulenceSettings
    {
        public float amplitude;
        public float frequency;
        public float spatialScale;
        public int seed;
        public int octaves;
        public float persistence;
    }

    [CreateAssetMenu(menuName = "MakerFlight RC/Data/Environment")]
    public class EnvironmentData : ScriptableObject
    {
        public string environmentId = "clear";
        public string displayName = "Clear";

        [Header("Atmosphere")]
        public float airDensity = 1.225f;

        [Header("Wind")]
        public Vector3 windDirection = new Vector3(1f, 0f, 0f);
        public float windSpeed = 5f;

        [Header("Turbulence")]
        public TurbulenceSettings turbulence = new TurbulenceSettings
        {
            amplitude = 1.5f,
            frequency = 0.4f,
            spatialScale = 0.03f,
            seed = 7,
            octaves = 3,
            persistence = 0.5f
        };

        private void OnValidate()
        {
            airDensity = Mathf.Max(0.01f, airDensity);
            windSpeed = Mathf.Max(0f, windSpeed);
            turbulence.amplitude = Mathf.Max(0f, turbulence.amplitude);
            turbulence.frequency = Mathf.Max(0f, turbulence.frequency);
            turbulence.spatialScale = Mathf.Max(0.0001f, turbulence.spatialScale);
            turbulence.octaves = Mathf.Clamp(turbulence.octaves, 1, 6);
            turbulence.persistence = Mathf.Clamp01(turbulence.persistence);
        }
    }
}
