using MakerFlightRC.Runtime.Channels;
using UnityEngine;
using UnityEngine.InputSystem;

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
            var keyboard = Keyboard.current;
            var gamepad = Gamepad.current;

            var throttleAxis = 0f;
            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed)
                {
                    throttleAxis += 1f;
                }
                if (keyboard.sKey.isPressed)
                {
                    throttleAxis -= 1f;
                }
            }
            if (gamepad != null)
            {
                throttleAxis = Mathf.Max(throttleAxis, gamepad.rightTrigger.ReadValue() - gamepad.leftTrigger.ReadValue());
            }

            targetState.throttle = Mathf.Clamp01(targetState.throttle + throttleAxis * throttleChangeRate * Time.deltaTime);
            targetState.roll = ReadRollAxis(keyboard, gamepad);
            targetState.pitch = ReadPitchAxis(keyboard, gamepad);
            targetState.yaw = ReadYawAxis(keyboard, gamepad);

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

        private static float ReadRollAxis(Keyboard keyboard, Gamepad gamepad)
        {
            var value = 0f;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed)
                {
                    value -= 1f;
                }
                if (keyboard.dKey.isPressed)
                {
                    value += 1f;
                }
            }

            if (gamepad != null)
            {
                value += gamepad.leftStick.ReadValue().x;
            }

            return value;
        }

        private static float ReadPitchAxis(Keyboard keyboard, Gamepad gamepad)
        {
            var value = 0f;
            if (keyboard != null)
            {
                if (keyboard.downArrowKey.isPressed)
                {
                    value -= 1f;
                }
                if (keyboard.upArrowKey.isPressed)
                {
                    value += 1f;
                }
            }

            if (gamepad != null)
            {
                value += -gamepad.leftStick.ReadValue().y;
            }

            return value;
        }

        private static float ReadYawAxis(Keyboard keyboard, Gamepad gamepad)
        {
            var value = 0f;
            if (keyboard != null)
            {
                if (keyboard.leftArrowKey.isPressed)
                {
                    value -= 1f;
                }
                if (keyboard.rightArrowKey.isPressed)
                {
                    value += 1f;
                }
            }

            if (gamepad != null)
            {
                value += gamepad.rightStick.ReadValue().x;
            }

            return value;
        }
    }
}
