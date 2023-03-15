using System;
using System.Collections.Generic;
using Common.MyInput.Title;
using MyApplication;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace OutGame.Title
{
    public class AllTitleInputObserver : IDisposable
    {
        public IObservable<bool> OnNext => _onNext;
        public IObservable<bool> SwitchCredit => _switchCredit;
        public IObservable<float> OnChangeScrollCreditValue => _onChangeScrollCreditValue;

        private readonly Subject<bool> _onNext;
        private readonly Subject<bool> _switchCredit;
        private readonly ReactiveProperty<float> _onChangeScrollCreditValue;

        private readonly List<BaseTitleInput> _talkInputs;

        public AllTitleInputObserver()
        {
            _onNext = new Subject<bool>();
            _switchCredit = new Subject<bool>();
            _onChangeScrollCreditValue = new ReactiveProperty<float>();
            
            _talkInputs = new List<BaseTitleInput>();
            _talkInputs.AddRange(InputMakeHelper.GenerateTitleInputsByAllControllers());
            foreach (var input in _talkInputs)
            {
                RegisterObserver(input);
            }

            InputSystem.onDeviceChange += UpdateInputActionButtonInputter;
            JoyconManager.Instance.added += UpdateJoycons;
            Debug.Log($"Init Input.Count:{_talkInputs.Count}");
        }

        private void RegisterObserver(BaseTitleInput input)
        {
            input.OnNext
                .Where(on => on)
                .Subscribe(_ => _onNext.OnNext(true));
            input.OnCredit
                .Subscribe(on => _switchCredit.OnNext(on));
            input.ScrollValue
                .Subscribe(value => _onChangeScrollCreditValue.Value = value);
        }

        private void UpdateInputActionButtonInputter(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            if (inputDeviceChange != InputDeviceChange.Added)
            {
                return;
            }

            MyInputDeviceType type = inputDevice.GetMyInputDeviceType();
            if (type != MyInputDeviceType.None)
            {
                var newInput = new AnyDeviceTitleInput(type, inputDevice.deviceId, inputDevice);
                _talkInputs.Add(newInput);
                RegisterObserver(newInput);
            }
        }

        private void UpdateJoycons(Joycon addedJoycon)
        {
            var newInput = new JoyconTitleInput(JoyconManager.Instance.j.IndexOf(addedJoycon), addedJoycon);
            _talkInputs.Add(newInput);
            RegisterObserver(newInput);
        }

        public void Dispose()
        {
            InputSystem.onDeviceChange -= UpdateInputActionButtonInputter;
            JoyconManager.Instance.added -= UpdateJoycons;
            
            foreach (var talkInput in _talkInputs)
            {
                talkInput.Dispose();
            }
            _onNext?.Dispose();
            _switchCredit?.Dispose();
            _onChangeScrollCreditValue?.Dispose();
        }
    }
}