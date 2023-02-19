using System.Collections.Generic;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;
using InGame.Common.Database;
using InGame.Database;
using InGame.SceneLoader;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
using OutGame.Prologue;
using OutGame.Prologue.MyInput;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Utility;

namespace SceneSequencer
{
    public class PrologueSequencer:BaseSceneSequencer
    {
        [SerializeField] private PrologueBehaviour prologueBehaviour;

        private List<IButtonInput> _buttonInputs;//MEMO: コールバックでListの中身が変動する恐れあり
        
        protected override void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            _buttonInputs = new List<IButtonInput>();
            InitControllers();
            InputSystem.onDeviceChange += UpdateInputActionButtonInputter;
            JoyconManager.Instance.added += UpdateJoycons;
            Debug.Log($"Init Input.Count:{_buttonInputs.Count}");
            prologueBehaviour.Init(OnToNextSceneFlag,_buttonInputs);
        }

        //TODO: 別クラスにまとめる //////////////////////////////////////////////////////////////////////////////////
        
        private void UpdateInputActionButtonInputter(InputDevice inputDevice,InputDeviceChange inputDeviceChange)
        {
            if (inputDeviceChange==InputDeviceChange.Added)
            {
                _buttonInputs.Add(new InputActionButtonInputter(inputDevice));
            }
        }

        private void UpdateJoycons(Joycon addedJoycon)
        {
            _buttonInputs.Add(new JoyConButtonInput(addedJoycon));
        }

        /// <summary>
        /// 現在複数コントローラーに対応していない && Joycon以外のゲームパッドに対応していない
        /// </summary>
        private void InitControllers()
        {
            var devices = InputSystem.devices;
            foreach (var device in devices.Where(device => !device.IsTDevice<Joystick>()))
            {
                Debug.Log($"Set {device.name}.");
                _buttonInputs.Add(new InputActionButtonInputter(device));
            }

            List<Joycon> joycons = new List<Joycon>(JoyconManager.Instance.j);
            foreach (var joycon in joycons)
            {
                _buttonInputs.Add(new JoyConButtonInput(joycon));
                Debug.Log($"Set Joycon. isLeft{joycon.isLeft}");
            }
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override async void ProcessInOrder()
        {
            BGMManager.Instance.Stop();
            try
            {
                await LoadManager.Instance.TryPlayFadeOut();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            BGMManager.Instance.Play(BGMPath.PROLOGUE);
            prologueBehaviour.CallStartNarration();
        }
        

        private void OnToNextSceneFlag()
        {
            toNextSceneFlag.OnNext(SceneName.PlayerCustom);
        }

        protected override async void Finish(string nextSceneName)
        {
            try
            {
                await LoadManager.Instance.TryPlayBlackFadeIn();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            await UniTask.Delay(TimeSpan.FromSeconds(2f),cancellationToken:this.GetCancellationTokenOnDestroy());
            BGMManager.Instance.FadeOut(BGMPath.PROLOGUE, 3f, () => {
                Debug.Log("BGMフェードアウト終了");
            });
            SceneManager.LoadScene(nextSceneName);
        }

        private void OnDestroy()
        {
            InputSystem.onDeviceChange -= UpdateInputActionButtonInputter;
            JoyconManager.Instance.added -= UpdateJoycons;
            prologueBehaviour.Dispose();
        }
    }
}