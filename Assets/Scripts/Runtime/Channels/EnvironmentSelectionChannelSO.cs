using System;
using MakerFlightRC.Data;
using UnityEngine;

namespace MakerFlightRC.Runtime.Channels
{
    [CreateAssetMenu(menuName = "MakerFlight RC/Channels/Environment Selection")]
    public class EnvironmentSelectionChannelSO : ScriptableObject
    {
        public event Action<EnvironmentData> OnRaised;

        public void Raise(EnvironmentData data)
        {
            OnRaised?.Invoke(data);
        }
    }
}
