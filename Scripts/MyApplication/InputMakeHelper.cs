using System.Collections.Generic;
using System.Linq;
using Common.MyInput.Score;
using Common.MyInput.Title;
using Common.MyInput.Player;
using Common.MyInput.PlayerCustom;
using Common.MyInput.Talk;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace MyApplication
{
    public static class InputMakeHelper
    {
        public static BaseTalkInput GenerateTalkInput(MyInputDeviceType deviceType,int deviceId)
        {
            switch (GetDeviceSystemType(deviceType))
            {
                case DeviceSystemType.Joycon:
                    return new JoyConTalkInput(deviceId, JoyconManager.Instance.j[deviceId]);
                case DeviceSystemType.AnyDevice:
                    InputDevice inputDevice=InputSystem.devices.FirstOrDefault(device => device.deviceId == deviceId);
                    if (inputDevice!=null)
                    {
                        return new AnyDeviceTalkInput(deviceType, deviceId, inputDevice);
                    }
                    Debug.LogError($"Not Found InputDevice. id:{deviceId}");
                    break;
                case DeviceSystemType.None:
                    Debug.LogError($"DeviceType Is Not Registered.");
                    break;
            }
            return null;
        }
        
        public static List<BaseTalkInput> GenerateTalkInputsByAllControllers()
        {
            List<BaseTalkInput> talkInputs = new ();
            var joycons = new List<Joycon>(JoyconManager.Instance.j);
            for (int i = 0; i < joycons.Count; i++)
            {
                Debug.Log($"JoyconSet. IsLeft:{joycons[i].isLeft}");
                talkInputs.Add(new JoyConTalkInput(i, joycons[i]));
            }

            var inputDevices = InputSystem.devices.ToArray();
            foreach (var inputDevice in inputDevices)
            {
                MyInputDeviceType type = inputDevice.GetMyInputDeviceType();
                if (type != MyInputDeviceType.None)
                {
                    Debug.Log($"{inputDevice.name} Set.");
                    talkInputs.Add(new AnyDeviceTalkInput(type, inputDevice.deviceId, inputDevice));
                }
            }

            return talkInputs;
        }

        public static BaseTitleInput GenerateTitleInput(MyInputDeviceType deviceType,int deviceId)
        {
            switch (GetDeviceSystemType(deviceType))
            {
                case DeviceSystemType.Joycon:
                    return new JoyconTitleInput(deviceId, JoyconManager.Instance.j[deviceId]);
                case DeviceSystemType.AnyDevice:
                    InputDevice inputDevice=InputSystem.devices.FirstOrDefault(device => device.deviceId == deviceId);
                    if (inputDevice!=null)
                    {
                        return new AnyDeviceTitleInput(deviceType, deviceId, inputDevice);
                    }
                    Debug.LogError($"Not Found InputDevice. id:{deviceId}");
                    break;
                case DeviceSystemType.None:
                    Debug.LogError($"DeviceType Is Not Registered.");
                    break;
            }
            return null;
        }
        
        
        public static List<BaseTitleInput> GenerateTitleInputsByAllControllers()
        {
            List<BaseTitleInput> titleInputs = new ();
            var joycons = new List<Joycon>(JoyconManager.Instance.j);
            for (int i = 0; i < joycons.Count; i++)
            {
                Debug.Log($"JoyconSet. IsLeft:{joycons[i].isLeft}");
                titleInputs.Add(new JoyconTitleInput(i, joycons[i]));
            }

            var inputDevices = InputSystem.devices.ToArray();
            foreach (var inputDevice in inputDevices)
            {
                MyInputDeviceType type = inputDevice.GetMyInputDeviceType();
                if (type != MyInputDeviceType.None)
                {
                    Debug.Log($"{inputDevice.name} Set.");
                    titleInputs.Add(new AnyDeviceTitleInput(type, inputDevice.deviceId, inputDevice));
                }
            }

            return titleInputs;
        }
        
        public static BasePlayerInput GeneratePlayerCustomInput(MyInputDeviceType deviceType,int deviceId)
        {
            switch (GetDeviceSystemType(deviceType))
            {
                case DeviceSystemType.Joycon:
                    return new JoyConPlayerInput(deviceId, JoyconManager.Instance.j[deviceId]);
                case DeviceSystemType.AnyDevice:
                    InputDevice inputDevice=InputSystem.devices.FirstOrDefault(device => device.deviceId == deviceId);
                    if (inputDevice!=null)
                    {
                        return new AnyDevicePlayerInput(deviceType, deviceId, inputDevice);
                    }
                    Debug.LogError($"Not Found InputDevice. id:{deviceId}");
                    break;
                case DeviceSystemType.None:
                    Debug.LogError($"DeviceType Is Not Registered.");
                    break;
            }
            return null;
        }
        
        public static List<BasePlayerCustomInput> GeneratePlayerCustomInputsByAllControllers()
        {
            List<BasePlayerCustomInput> customInputs = new ();
            var joycons = new List<Joycon>(JoyconManager.Instance.j);
            for (int i = 0; i < joycons.Count; i++)
            {
                Debug.Log($"JoyconSet. IsLeft:{joycons[i].isLeft}");
                customInputs.Add(new JoyConPlayerCustomInput(i, joycons[i]));
            }

            var inputDevices = InputSystem.devices.ToArray();
            foreach (var inputDevice in inputDevices)
            {
                MyInputDeviceType type = inputDevice.GetMyInputDeviceType();
                if (type != MyInputDeviceType.None)
                {
                    Debug.Log($"{inputDevice.name} Set.");
                    customInputs.Add(new AnyDevicePlayerCustomInput(type, inputDevice.deviceId, inputDevice));
                }
            }

            return customInputs;
        }

        public static BaseScoreInput GenerateScoreInput(MyInputDeviceType deviceType,int deviceId)
        {
            switch (GetDeviceSystemType(deviceType))
            {
                case DeviceSystemType.Joycon:
                    return new JoyconScoreInput(deviceId, JoyconManager.Instance.j[deviceId]);
                case DeviceSystemType.AnyDevice:
                    InputDevice inputDevice=InputSystem.devices.FirstOrDefault(device => device.deviceId == deviceId);
                    if (inputDevice!=null)
                    {
                        return new AnyDeviceScoreInput(deviceType, deviceId, inputDevice);
                    }
                    Debug.LogError($"Not Found InputDevice. id:{deviceId}");
                    break;
                case DeviceSystemType.None:
                    Debug.LogError($"DeviceType Is Not Registered.");
                    break;
            }
            return null;
        }
        
        public static List<BaseScoreInput> GenerateScoreInputsByAllControllers()
        {
            List<BaseScoreInput> customInputs = new ();
            var joycons = new List<Joycon>(JoyconManager.Instance.j);
            for (int i = 0; i < joycons.Count; i++)
            {
                Debug.Log($"JoyconSet. IsLeft:{joycons[i].isLeft}");
                customInputs.Add(new JoyconScoreInput(i, joycons[i]));
            }

            var inputDevices = InputSystem.devices.ToArray();
            foreach (var inputDevice in inputDevices)
            {
                MyInputDeviceType type = inputDevice.GetMyInputDeviceType();
                if (type != MyInputDeviceType.None)
                {
                    Debug.Log($"{inputDevice.name} Set.");
                    customInputs.Add(new AnyDeviceScoreInput(type, inputDevice.deviceId, inputDevice));
                }
            }

            return customInputs;
        }

        private static DeviceSystemType GetDeviceSystemType(MyInputDeviceType deviceType)
        {
            if(deviceType is MyInputDeviceType.JoyconLeft or MyInputDeviceType.JoyconRight)
            {
                return DeviceSystemType.Joycon;
            }
            if (deviceType is MyInputDeviceType.Keyboard or MyInputDeviceType.Procon or MyInputDeviceType.GamePad)
            {
                return DeviceSystemType.AnyDevice;
            }
            return DeviceSystemType.None;
        }

        private enum DeviceSystemType
        {
            Joycon,
            AnyDevice,
            None
        }
    }
}