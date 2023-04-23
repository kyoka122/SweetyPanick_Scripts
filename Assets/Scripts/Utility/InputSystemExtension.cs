using MyApplication;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

namespace Utility
{
    public static class InputSystemExtension
    {
        public static TDevice GetTDevice<TDevice>(this InputDevice inputDevice)
            where TDevice : InputDevice
        {
            if (inputDevice is TDevice deviceOfType)
            {
                return deviceOfType;
            }
            return null;
        }
        
        public static bool IsTDevice<TDevice>(this InputDevice inputDevice)
            where TDevice : InputDevice
        {
            return inputDevice is TDevice;
        }
        
        /// <summary>
        /// Joyconを含まない、deviceのタイプ判定を行う
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static MyInputDeviceType GetMyInputDeviceType(this InputDevice device)
        {
            if (device.IsTDevice<Keyboard>())
            {
                return MyInputDeviceType.Keyboard;
            }
            if (device.IsTDevice<SwitchProControllerHID>())
            {
                return MyInputDeviceType.Procon;
            }
            if (device.IsTDevice<XInputController>())
            {
                return MyInputDeviceType.GamePad;
            }
            return MyInputDeviceType.None;
        }
        
        /// <summary>
        /// Joyconのタイプ判定を行う
        /// </summary>
        /// <param name="newDevice"></param>
        /// <returns></returns>
        public static MyInputDeviceType GetMyInputDeviceType(this Joycon joycon)
        {
            return joycon.isLeft ? MyInputDeviceType.JoyconLeft : MyInputDeviceType.JoyconRight;
        }
    }
}