using System;
using System.Collections.Generic;
using MyApplication;
using OutGame.PlayerCustom.MyInput;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
using Utility;

namespace OutGame.PlayerCustom.Entity
{
    /// <summary>
    /// コントローラー登録前
    /// </summary>
    public class InputCaseUnknownControllerEntity:IDisposable
    {
        public IReadOnlyList<BaseCaseUnknownControllerInput> CustomInputs => _customInputs;
        private readonly List<BaseCaseUnknownControllerInput> _customInputs;
        
        public InputCaseUnknownControllerEntity()
        {
            _customInputs = new List<BaseCaseUnknownControllerInput>();
            var joycons = new List<Joycon>(JoyconManager.Instance.j);
            foreach (var joycon in joycons)
            {
                Debug.Log($"JoyconSet. isleft:{joycon.isLeft}");
                _customInputs.Add(new JoyConCaseUnknownControllerInput(joycon));
            }

            var inputDevices = InputSystem.devices.ToArray();
            foreach (var inputDevice in inputDevices)
            {
                MyInputDeviceType type = GetAnyInputDeviceType(inputDevice);
                if (type != MyInputDeviceType.None)
                {
                    Debug.Log($"{inputDevice.name}Set.");
                    _customInputs.Add(new AnyDeviseCaseUnknownControllerInput(type,inputDevice));
                }
            }
        }
        
        /// <summary>
        /// deviceの型判定を行うJoyconを含まない、
        /// </summary>
        /// <param name="newDevice"></param>
        /// <returns></returns>
        private MyInputDeviceType GetAnyInputDeviceType(InputDevice newDevice)
        {
            if (newDevice.IsTDevice<Keyboard>())
            {
                return MyInputDeviceType.Keyboard;
            }
            if (newDevice.IsTDevice<SwitchProControllerHID>())
            {
                return MyInputDeviceType.Procon;
            }
            if (newDevice.IsTDevice<XInputController>())
            {
                return MyInputDeviceType.GamePad;
            }
            return MyInputDeviceType.None;
        }

        public void Dispose()
        {
            foreach (var controllerUnKnownInput in _customInputs)
            {
                controllerUnKnownInput.Dispose();
            }
        }
    }
}