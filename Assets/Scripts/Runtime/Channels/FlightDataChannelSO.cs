using System;
using UnityEngine;

namespace MakerFlightRC.Runtime.Channels
{
    [CreateAssetMenu(menuName = "MakerFlight RC/Channels/Flight Data")]
    public class FlightDataChannelSO : ScriptableObject
    {
        public event Action<FlightData> OnRaised;

        public void Raise(FlightData data)
        {
            OnRaised?.Invoke(data);
        }
    }
}
