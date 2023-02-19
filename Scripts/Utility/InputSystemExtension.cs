using UnityEngine.InputSystem;

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
    }
}