using System;
using System.Collections.Generic;

namespace MakerFlightRC.Runtime.Config
{
    [Serializable]
    public class ConfigSaveData
    {
        public int version = 1;
        public string lastAircraftId;
        public List<AircraftConfigState> aircraftConfigs = new List<AircraftConfigState>();
    }
}
