using System.Collections.Generic;
using Common.MyInput.Talk;
using MyApplication;
using TalkSystem;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace OutGame.Prologue
{
    /// <summary>
    /// Talk用InputMapより、全コントローラーのNextとSkipを監視する
    /// </summary>
    public class AllTalkInputObserver:BaseTalkKeyObserver
    {
        private readonly List<BaseTalkInput> _talkInputs;
        
        public AllTalkInputObserver()
        {
            _talkInputs = new List<BaseTalkInput>();
            _talkInputs.AddRange(InputMakeHelper.GenerateTalkInputsByAllControllers());
            foreach (var input in _talkInputs)
            {
                RegisterObserver(input);
            }
            InputSystem.onDeviceChange += UpdateInputActionButtonInputter;
            JoyconManager.Instance.added += UpdateJoycons;
            Debug.Log($"Init Input.Count:{_talkInputs.Count}");
        }

        private void RegisterObserver(BaseTalkInput input)
        {
            input.Next.Where(on=>on).Subscribe(_ =>
            {
                onNext.OnNext(true);
            });
            
            input.Skip.Subscribe(on =>
            {
                onSkip.OnNext(on);
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
                var newInput=new AnyDeviceTalkInput(type, inputDevice.deviceId, inputDevice);
                _talkInputs.Add(newInput);
                RegisterObserver(newInput);
            }
        }

        private void UpdateJoycons(Joycon addedJoycon)
        {
            var newInput = new JoyConTalkInput(JoyconManager.Instance.j.IndexOf(addedJoycon), addedJoycon);
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