using System;
using UnityEngine;

namespace MakerFlightRC.Runtime.Environment
{
    [Serializable]
    public struct EnvironmentState
    {
        public Vector3 wind;
        public Vector3 turbulence;
        public float airDensity;

        public Vector3 AirVelocity => wind + turbulence;
    }
}
