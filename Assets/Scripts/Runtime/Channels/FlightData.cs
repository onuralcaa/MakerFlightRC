using System;

namespace MakerFlightRC.Runtime.Channels
{
    [Serializable]
    public struct FlightData
    {
        public float speed;
        public float altitude;
        public float pitch;
        public float roll;
    }
}
