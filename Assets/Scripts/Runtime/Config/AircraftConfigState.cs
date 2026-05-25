using System;
using MakerFlightRC.Data;
using UnityEngine;

namespace MakerFlightRC.Runtime.Config
{
    [Serializable]
    public class AircraftConfigState
    {
        public string aircraftId;
        public float thrust;
        public float wingArea;
        public float wingSpan;
        public float mass;
        public Vector3 centerOfMass;

        public AircraftConfigState()
        {
        }

        public AircraftConfigState(AircraftData data)
        {
            aircraftId = data.aircraftId;
            thrust = data.baseThrust;
            wingArea = data.baseWingArea;
            wingSpan = data.baseWingSpan;
            mass = data.baseMass;
            centerOfMass = data.baseCenterOfMass;
        }
    }
}
