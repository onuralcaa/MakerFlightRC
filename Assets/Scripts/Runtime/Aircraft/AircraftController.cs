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
        [SerializeField] private InputChannelSO inputChannel;
        [SerializeField] private FlightDataChannelSO flightDataChannel;
        [SerializeField] private KeyboardInputProvider inputProvider;
        [SerializeField] private AircraftData defaultAircraft;

        private Rigidbody rb;
        private AircraftData aircraftData;
        private AircraftConfigState configState;
        private EnvironmentState environmentState;
        private InputState channelInputState;
        private bool hasReceivedInput;

        private const float InputEpsilon = 0.01f;
        private const float DefaultAirDensity = 1.225f;
        private const float ControlSpeedGate = 8f;
        private const float AngularDamping = 1.5f;
        private const float StabilityGain = 2.0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                transform.position = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;
                rb.mass = 2.0f;
                rb.drag = 1.0f;
                rb.angularDrag = 1.5f;
                rb.maxAngularVelocity = 4f;
            }

            // Initialize safe default environment state
            if (environmentState.airDensity <= 0f || float.IsNaN(environmentState.airDensity) || float.IsInfinity(environmentState.airDensity))
            {
                environmentState = new EnvironmentState
                {
                    wind = Vector3.zero,
                    turbulence = Vector3.zero,
                    airDensity = DefaultAirDensity
                };
            }
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
            if (inputChannel != null)
            {
                inputChannel.OnRaised += HandleInputUpdated;
            }

            if (aircraftData == null && defaultAircraft != null)
            {
                HandleAircraftSelected(defaultAircraft);
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
            if (inputChannel != null)
            {
                inputChannel.OnRaised -= HandleInputUpdated;
            }
        }

        private void FixedUpdate()
        {
            // Safety boundary: reset if aircraft strays beyond 1000 units
            if (transform.position.magnitude > 1000f)
            {
                transform.position = Vector3.zero;
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                return;
            }

            if (aircraftData == null || configState == null)
            {
                return;
            }

            var input = inputProvider != null ? inputProvider.CurrentState : channelInputState;
            EnablePhysicsIfNeeded(input);
            if (!hasReceivedInput)
            {
                return;
            }

            // Ensure environment state is valid
            if (environmentState.airDensity <= 0f || float.IsNaN(environmentState.airDensity) || float.IsInfinity(environmentState.airDensity))
            {
                environmentState.airDensity = DefaultAirDensity;
            }

            var airVelocity = rb.velocity - environmentState.AirVelocity;

            // Check for NaN/Infinity in airVelocity
            if (float.IsNaN(airVelocity.x) || float.IsNaN(airVelocity.y) || float.IsNaN(airVelocity.z) ||
                float.IsInfinity(airVelocity.x) || float.IsInfinity(airVelocity.y) || float.IsInfinity(airVelocity.z))
            {
                airVelocity = Vector3.zero;
            }

            var speed = airVelocity.magnitude;

            // Check for NaN/Infinity in speed
            if (float.IsNaN(speed) || float.IsInfinity(speed))
            {
                speed = 0f;
            }

            // Apply thrust with safety checks (further scaled down for gentle acceleration)
            // Reuse currentThrust calculated at FixedUpdate start for safety lock
            var thrust = Mathf.Max(0f, configState.thrust) * Mathf.Clamp01(input.throttle) * 0.4f;
            if (!float.IsNaN(thrust) && !float.IsInfinity(thrust) && thrust > 0f && thrust < 10000f)
            {
                rb.AddForce(transform.forward * thrust * Time.fixedDeltaTime, ForceMode.Force);
            }

            if (speed > Mathf.Epsilon)
            {
                // Clamp parameters to reasonable ranges to prevent overflow
                var airDensity = Mathf.Clamp(environmentState.airDensity, 0.1f, 10f);
                var wingArea = Mathf.Clamp(configState.wingArea, 0.1f, 1000f);
                var liftCoeff = Mathf.Clamp(aircraftData.liftCoefficient, -100f, 100f) * 0.2f;
                var dragCoeff = Mathf.Clamp(aircraftData.dragCoefficient, 0f, 100f);

                var lift = 0.5f * airDensity * speed * speed * wingArea * liftCoeff;
                var drag = 0.5f * airDensity * speed * speed * wingArea * dragCoeff;

                // Check lift validity
                if (!float.IsNaN(lift) && !float.IsInfinity(lift) && lift > -100000f && lift < 100000f)
                {
                    rb.AddForce(transform.up * lift);
                }

                // Check drag validity and apply
                if (!float.IsNaN(drag) && !float.IsInfinity(drag) && drag > -100000f && drag < 100000f)
                {
                    Vector3 airVelocityNormalized = airVelocity.normalized;
                    if (!float.IsNaN(airVelocityNormalized.x) && !float.IsNaN(airVelocityNormalized.y) && !float.IsNaN(airVelocityNormalized.z))
                    {
                        rb.AddForce(-airVelocityNormalized * drag);
                    }
                }
            }

            // Apply torque with safety checks
            var controlScale = Mathf.Clamp01(speed / ControlSpeedGate);
            var torque = new Vector3(
                input.pitch * aircraftData.controlTorque.x,
                input.yaw * aircraftData.controlTorque.y,
                -input.roll * aircraftData.controlTorque.z) * controlScale;

            if (!float.IsNaN(torque.x) && !float.IsNaN(torque.y) && !float.IsNaN(torque.z) &&
                !float.IsInfinity(torque.x) && !float.IsInfinity(torque.y) && !float.IsInfinity(torque.z))
            {
                rb.AddRelativeTorque(torque);
            }

            // Apply angular damping to reduce oscillations
            if (!float.IsNaN(rb.angularVelocity.x) && !float.IsNaN(rb.angularVelocity.y) && !float.IsNaN(rb.angularVelocity.z))
            {
                rb.AddRelativeTorque(-rb.angularVelocity * AngularDamping, ForceMode.Acceleration);
            }

            // Gentle automatic stabilization: push aircraft upright over time
            var uprightError = Vector3.Cross(transform.up, Vector3.up);
            if (!float.IsNaN(uprightError.x) && !float.IsNaN(uprightError.y) && !float.IsNaN(uprightError.z))
            {
                rb.AddTorque(uprightError * StabilityGain, ForceMode.Acceleration);
            }

            PublishFlightData(speed);
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
            // Validate incoming environment state
            if (state.airDensity <= 0f || float.IsNaN(state.airDensity) || float.IsInfinity(state.airDensity))
            {
                state.airDensity = DefaultAirDensity;
            }
            environmentState = state;
        }

        private void HandleInputUpdated(InputState state)
        {
            channelInputState = state;
            EnablePhysicsIfNeeded(state);
        }

        private void ApplyConfigToRigidbody(AircraftConfigState state)
        {
            rb.mass = state.mass;
            rb.centerOfMass = state.centerOfMass;
        }

        private void PublishFlightData(float speed)
        {
            if (flightDataChannel == null)
            {
                return;
            }

            var euler = transform.eulerAngles;
            var data = new FlightData
            {
                speed = speed,
                altitude = transform.position.y,
                pitch = NormalizeAngle(euler.x),
                roll = NormalizeAngle(euler.z)
            };

            flightDataChannel.Raise(data);
        }

        private static float NormalizeAngle(float angle)
        {
            if (angle > 180f)
            {
                angle -= 360f;
            }
            return angle;
        }

        private void EnablePhysicsIfNeeded(InputState input)
        {
            if (hasReceivedInput || rb == null)
            {
                return;
            }

            var hasInput = Mathf.Abs(input.throttle) > InputEpsilon
                           || Mathf.Abs(input.roll) > InputEpsilon
                           || Mathf.Abs(input.pitch) > InputEpsilon
                           || Mathf.Abs(input.yaw) > InputEpsilon;

            if (!hasInput)
            {
                return;
            }

            hasReceivedInput = true;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
