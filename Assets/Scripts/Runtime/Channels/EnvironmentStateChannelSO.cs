using System;
using MakerFlightRC.Runtime.Environment;
using UnityEngine;

namespace MakerFlightRC.Runtime.Channels
{
    [CreateAssetMenu(menuName = "MakerFlight RC/Channels/Environment State")]
    public class EnvironmentStateChannelSO : ScriptableObject
    {
        public event Action<EnvironmentState> OnRaised;

        public void Raise(EnvironmentState state)
        {
            OnRaised?.Invoke(state);
        }
    }
}
