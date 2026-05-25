using System;
using MakerFlightRC.Runtime.Config;
using UnityEngine;

namespace MakerFlightRC.Runtime.Channels
{
    [CreateAssetMenu(menuName = "MakerFlight RC/Channels/Aircraft Config")]
    public class AircraftConfigChannelSO : ScriptableObject
    {
        public event Action<AircraftConfigState> OnRaised;

        public void Raise(AircraftConfigState config)
        {
            OnRaised?.Invoke(config);
        }
    }
}
