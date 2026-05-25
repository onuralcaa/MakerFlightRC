using System.IO;
using MakerFlightRC.Data;
using MakerFlightRC.Runtime.Channels;
using UnityEngine;

namespace MakerFlightRC.Runtime.Config
{
    public class ConfigurationManager : MonoBehaviour
    {
        [SerializeField] private AircraftSelectionChannelSO aircraftSelectionChannel;
        [SerializeField] private AircraftConfigChannelSO configChannel;
        [SerializeField] private string saveFileName = "aircraft_config.json";

        private ConfigSaveData saveData = new ConfigSaveData();
        private AircraftData currentAircraft;
        private AircraftConfigState currentConfig;

        public AircraftConfigState CurrentConfig => currentConfig;

        private void Awake()
        {
            Load();
        }

        private void OnEnable()
        {
            if (aircraftSelectionChannel != null)
            {
                aircraftSelectionChannel.OnRaised += HandleAircraftSelected;
            }
        }

        private void OnDisable()
        {
            if (aircraftSelectionChannel != null)
            {
                aircraftSelectionChannel.OnRaised -= HandleAircraftSelected;
            }

            Save();
        }

        public void SetThrust(float value)
        {
            if (currentConfig == null)
            {
                return;
            }

            currentConfig.thrust = Mathf.Max(0f, value);
            PublishCurrentConfig();
        }

        public void SetWingArea(float value)
        {
            if (currentConfig == null)
            {
                return;
            }

            currentConfig.wingArea = Mathf.Max(0.01f, value);
            PublishCurrentConfig();
        }

        public void SetWingSpan(float value)
        {
            if (currentConfig == null)
            {
                return;
            }

            currentConfig.wingSpan = Mathf.Max(0.01f, value);
            PublishCurrentConfig();
        }

        public void SetMass(float value)
        {
            if (currentConfig == null)
            {
                return;
            }

            currentConfig.mass = Mathf.Max(0.1f, value);
            PublishCurrentConfig();
        }

        public void SetCenterOfMass(Vector3 value)
        {
            if (currentConfig == null)
            {
                return;
            }

            currentConfig.centerOfMass = value;
            PublishCurrentConfig();
        }

        public void Save()
        {
            saveData.lastAircraftId = currentAircraft != null ? currentAircraft.aircraftId : saveData.lastAircraftId;
            var json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(GetSavePath(), json);
        }

        public void Load()
        {
            var path = GetSavePath();
            if (!File.Exists(path))
            {
                saveData = new ConfigSaveData();
                return;
            }

            var json = File.ReadAllText(path);
            saveData = JsonUtility.FromJson<ConfigSaveData>(json) ?? new ConfigSaveData();
        }

        private void HandleAircraftSelected(AircraftData data)
        {
            currentAircraft = data;
            currentConfig = GetOrCreateConfig(data);
            PublishCurrentConfig();
        }

        private AircraftConfigState GetOrCreateConfig(AircraftData data)
        {
            foreach (var config in saveData.aircraftConfigs)
            {
                if (config.aircraftId == data.aircraftId)
                {
                    return config;
                }
            }

            var created = new AircraftConfigState(data);
            saveData.aircraftConfigs.Add(created);
            return created;
        }

        private void PublishCurrentConfig()
        {
            if (configChannel != null && currentConfig != null)
            {
                configChannel.Raise(currentConfig);
            }
        }

        private string GetSavePath()
        {
            return Path.Combine(Application.persistentDataPath, saveFileName);
        }
    }
}
