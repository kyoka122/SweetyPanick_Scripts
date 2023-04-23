using System.Collections.Generic;
using Common.MyInput.Score;
using MyApplication;
using TalkSystem;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace OutGame.Prologue
{
    /// <summary>
    /// Scoreシーン用InputMapより、全コントローラーのNextを監視する
    /// </summary>
    public class AllNextKeyObserver : BaseTalkKeyObserver
    {
        private readonly List<BaseScoreInput> _talkInputs;
        
        public AllNextKeyObserver()
        {
            _talkInputs = new List<BaseScoreInput>();
            _talkInputs.AddRange(InputMakeHelper.GenerateScoreInputsByAllControllers());
            foreach (var input in _talkInputs)
            {
                RegisterObserver(input);
            }
            InputSystem.onDeviceChange += UpdateInputActionButtonInputter;
            JoyconManager.Instance.added += UpdateJoycons;
            Debug.Log($"Init Input.Count:{_talkInputs.Count}");
        }

        private void RegisterObserver(BaseScoreInput input)
        {
            input.Next.Where(on=>on).Subscribe(_ =>
            {
                onNext.OnNext(true);
            });
        }

        private void UpdateInputActionButtonInputter(InputDevice inputDevice,InputDeviceChange inputDeviceChange)
        {
            if (inputDeviceChange!=InputDeviceChange.Added)
            {
                return;
            }
            MyInputDeviceType type = inputDevice.GetMyInputDeviceType();
            if (type != MyInputDeviceType.None)
            {
                var newInput=new AnyDeviceScoreInput(type, inputDevice.deviceId, inputDevice);
                _talkInputs.Add(newInput);
                RegisterObserver(newInput);
            }
        }

        private void UpdateJoycons(Joycon addedJoycon)
        {
            var newInput = new JoyconScoreInput(JoyconManager.Instance.j.IndexOf(addedJoycon), addedJoycon);
            _talkInputs.Add(newInput);
            RegisterObserver(newInput);
        }

        public override void Dispose()
        {
            InputSystem.onDeviceChange -= UpdateInputActionButtonInputter;
            JoyconManager.Instance.added -= UpdateJoycons;
            foreach (var talkInput in _talkInputs)
            {
                talkInput?.Dispose();
            }
            base.Dispose();
        }
    }
}