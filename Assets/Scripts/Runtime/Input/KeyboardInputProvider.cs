using UnityEngine;
using MakerFlightRC.Runtime.Channels;

namespace MakerFlightRC.Runtime.Input
{
    public class KeyboardInputProvider : MonoBehaviour, IInputProvider
    {
        [SerializeField] private float throttleChangeRate = 0.6f;
        [SerializeField] private float inputSmoothing = 8f;
        [SerializeField] private InputChannelSO inputChannel;

        private InputState currentState;
        private InputState targetState;

        public InputState CurrentState => currentState;

        private void Update()
        {
            var throttleDelta = 0f;
            if (UnityEngine.Input.GetKey(KeyCode.W))
            {
                throttleDelta += 1f;
            }
            if (UnityEngine.Input.GetKey(KeyCode.S))
            {
                throttleDelta -= 1f;
            }

            targetState.throttle = Mathf.Clamp01(targetState.throttle + throttleDelta * throttleChangeRate * Time.deltaTime);
            targetState.roll = GetAxis(KeyCode.A, KeyCode.D);
            targetState.pitch = GetAxis(KeyCode.DownArrow, KeyCode.UpArrow);
            targetState.yaw = GetAxis(KeyCode.LeftArrow, KeyCode.RightArrow);

            var t = 1f - Mathf.Exp(-inputSmoothing * Time.deltaTime);
            currentState.throttle = Mathf.Lerp(currentState.throttle, targetState.throttle, t);
            currentState.roll = Mathf.Lerp(currentState.roll, targetState.roll, t);
            currentState.pitch = Mathf.Lerp(currentState.pitch, targetState.pitch, t);
            currentState.yaw = Mathf.Lerp(currentState.yaw, targetState.yaw, t);

            if (inputChannel != null)
            {
                inputChannel.Raise(currentState);
            }
        }

        private static float GetAxis(KeyCode negative, KeyCode positive)
        {
            var value = 0f;
            if (UnityEngine.Input.GetKey(negative))
            {
                value -= 1f;
            }
            if (UnityEngine.Input.GetKey(positive))
            {
                value += 1f;
            }
            return value;
        }
    }
}
