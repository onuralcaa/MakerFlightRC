using System;
using MakerFlightRC.Data;
using UnityEngine;

namespace MakerFlightRC.Runtime.Channels
{
    [CreateAssetMenu(menuName = "MakerFlight RC/Channels/Aircraft Selection")]
    public class AircraftSelectionChannelSO : ScriptableObject
    {
        public event Action<AircraftData> OnRaised;

        public void Raise(AircraftData data)
        {
            OnRaised?.Invoke(data);
        }
    }
}
