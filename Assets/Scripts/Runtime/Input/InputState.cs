using System;

namespace MakerFlightRC.Runtime.Input
{
    [Serializable]
    public struct InputState
    {
        public float throttle;
        public float roll;
        public float pitch;
        public float yaw;
    }
}
