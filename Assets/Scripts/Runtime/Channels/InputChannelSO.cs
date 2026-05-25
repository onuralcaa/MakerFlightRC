using System;
using MakerFlightRC.Runtime.Input;
using UnityEngine;

namespace MakerFlightRC.Runtime.Channels
{
    [CreateAssetMenu(menuName = "MakerFlight RC/Channels/Input")]
    public class InputChannelSO : ScriptableObject
    {
        public event Action<InputState> OnRaised;

        public void Raise(InputState state)
        {
            OnRaised?.Invoke(state);
        }
    }
}
