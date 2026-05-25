using MakerFlightRC.Data;
using MakerFlightRC.Runtime.Channels;
using UnityEngine;

namespace MakerFlightRC.Runtime.Environment
{
    public class EnvironmentManager : MonoBehaviour
    {
        [SerializeField] private EnvironmentSelectionChannelSO environmentSelectionChannel;
        [SerializeField] private EnvironmentStateChannelSO environmentStateChannel;

        private EnvironmentData currentData;

        private void OnEnable()
        {
            if (environmentSelectionChannel != null)
            {
                environmentSelectionChannel.OnRaised += HandleEnvironmentSelected;
            }
        }

        private void OnDisable()
        {
            if (environmentSelectionChannel != null)
            {
                environmentSelectionChannel.OnRaised -= HandleEnvironmentSelected;
            }
        }

        private void FixedUpdate()
        {
            if (currentData == null || environmentStateChannel == null)
            {
                return;
            }

            var wind = currentData.windDirection.normalized * currentData.windSpeed;
            var turbulence = SampleTurbulence(transform.position, Time.time, currentData.turbulence);

            var state = new EnvironmentState
            {
                wind = wind,
                turbulence = turbulence,
                airDensity = currentData.airDensity
            };

            environmentStateChannel.Raise(state);
        }

        private void HandleEnvironmentSelected(EnvironmentData data)
        {
            currentData = data;
        }

        private static Vector3 SampleTurbulence(Vector3 position, float time, TurbulenceSettings settings)
        {
            var scale = settings.spatialScale;
            var timeOffset = time * settings.frequency;
            var baseX = (position.x + settings.seed) * scale;
            var baseY = (position.z + settings.seed) * scale;

            var x = FractalNoise(baseX, baseY + 17.3f, timeOffset, settings);
            var y = FractalNoise(baseX + 23.7f, baseY + 91.1f, timeOffset, settings);
            var z = FractalNoise(baseX + 43.2f, baseY + 5.1f, timeOffset, settings);

            return new Vector3(x, y, z) * settings.amplitude;
        }

        private static float FractalNoise(float x, float y, float time, TurbulenceSettings settings)
        {
            float value = 0f;
            float amplitude = 1f;
            float frequency = 1f;

            for (int i = 0; i < settings.octaves; i++)
            {
                var noise = Mathf.PerlinNoise(x * frequency + time, y * frequency + time);
                value += (noise * 2f - 1f) * amplitude;
                amplitude *= settings.persistence;
                frequency *= 2f;
            }

            return value;
        }
    }
}
