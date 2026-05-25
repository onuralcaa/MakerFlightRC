using MakerFlightRC.Data;
using MakerFlightRC.Runtime.Channels;
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
