using MakerFlightRC.Data;
using MakerFlightRC.Runtime.Channels;
using MakerFlightRC.Runtime.Config;
using MakerFlightRC.Runtime.Environment;
using MakerFlightRC.Runtime.Input;
using UnityEngine;

namespace MakerFlightRC.Runtime.Aircraft
{
    [RequireComponent(typeof(Rigidbody))]
    public class AircraftController : MonoBehaviour
    {
        [SerializeField] private AircraftSelectionChannelSO aircraftSelectionChannel;
        [SerializeField] private AircraftConfigChannelSO aircraftConfigChannel;
        [SerializeField] private EnvironmentStateChannelSO environmentStateChannel;
        [SerializeField] private KeyboardInputProvider inputProvider;

        private Rigidbody rb;
        private AircraftData aircraftData;
        private AircraftConfigState configState;
        private EnvironmentState environmentState;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            if (aircraftSelectionChannel != null)
            {
                aircraftSelectionChannel.OnRaised += HandleAircraftSelected;
            }
            if (aircraftConfigChannel != null)
            {
                aircraftConfigChannel.OnRaised += HandleConfigUpdated;
            }
            if (environmentStateChannel != null)
            {
                environmentStateChannel.OnRaised += HandleEnvironmentUpdated;
            }
        }

        private void OnDisable()
        {
            if (aircraftSelectionChannel != null)
            {
                aircraftSelectionChannel.OnRaised -= HandleAircraftSelected;
            }
            if (aircraftConfigChannel != null)
            {
                aircraftConfigChannel.OnRaised -= HandleConfigUpdated;
            }
            if (environmentStateChannel != null)
            {
                environmentStateChannel.OnRaised -= HandleEnvironmentUpdated;
            }
        }

        private void FixedUpdate()
        {
            if (aircraftData == null || configState == null || inputProvider == null)
            {
                return;
            }

            var input = inputProvider.CurrentState;
            var airVelocity = rb.velocity - environmentState.AirVelocity;
            var speed = airVelocity.magnitude;
            if (speed <= Mathf.Epsilon)
            {
                return;
            }

            var airDensity = environmentState.airDensity;
            var lift = 0.5f * airDensity * speed * speed * configState.wingArea * aircraftData.liftCoefficient;
            var drag = 0.5f * airDensity * speed * speed * configState.wingArea * aircraftData.dragCoefficient;

            rb.AddForce(transform.up * lift);
            rb.AddForce(-airVelocity.normalized * drag);

            var thrust = Mathf.Max(0f, configState.thrust) * Mathf.Clamp01(input.throttle);
            rb.AddForce(transform.forward * thrust);

            var torque = new Vector3(
                input.pitch * aircraftData.controlTorque.x,
                input.yaw * aircraftData.controlTorque.y,
                -input.roll * aircraftData.controlTorque.z);
            rb.AddRelativeTorque(torque);
        }

        private void HandleAircraftSelected(AircraftData data)
        {
            aircraftData = data;
            if (configState == null && aircraftData != null)
            {
                configState = new AircraftConfigState(aircraftData);
                ApplyConfigToRigidbody(configState);
            }
        }

        private void HandleConfigUpdated(AircraftConfigState state)
        {
            if (state == null)
            {
                return;
            }

            configState = state;
            ApplyConfigToRigidbody(state);
        }

        private void HandleEnvironmentUpdated(EnvironmentState state)
        {
            environmentState = state;
        }

        private void ApplyConfigToRigidbody(AircraftConfigState state)
        {
            rb.mass = state.mass;
            rb.centerOfMass = state.centerOfMass;
        }
    }
}
