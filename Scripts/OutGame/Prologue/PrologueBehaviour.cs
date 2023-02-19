using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using InGame.SceneLoader;
using OutGame.Prologue.MyInput;
using UniRx;
using MyApplication;
using UnityEngine;

namespace OutGame.Prologue
{
    public class PrologueBehaviour:MonoBehaviour
    {
        [SerializeField] Fungus.Flowchart flowchart;
        [SerializeField] private ProloguePlayerAnimation prologuePlayerAnimation;

        [SerializeField] private GameObject castleObj;
        [SerializeField] private GameObject worldMapObj;
        [SerializeField] private SpriteRenderer backGroundFilter;

        private IReadOnlyList<IButtonInput> _buttonInputs;
        private CancellationToken _token;
        private Action _toNextSceneEvent;
        
        public void Init(Action toNextSceneEvent,List<IButtonInput> buttonInputs)
        {
            _toNextSceneEvent = toNextSceneEvent;
            _buttonInputs = buttonInputs;//MEMO: コールバックでListの中身が変動する恐れあり
            prologuePlayerAnimation.Init();
            worldMapObj.SetActive(true);
            castleObj.SetActive(false);
            backGroundFilter.enabled = true;
            RegisterInputObserver();
        }

        /// <summary>
        /// このメソッドを呼び出すと会話シーン用のキー入力受け付けを開始します
        /// </summary>
        private void RegisterInputObserver()
        {
            List<IButtonInput> buttonInputs = new List<IButtonInput>(_buttonInputs);
            foreach (var buttonInput in buttonInputs)
            {
                buttonInput.OnButton.Subscribe(_ =>
                {
                    OnClickMethod();
                }).AddTo(_token);
            }
        }

        private void OnClickMethod()
        {
            //MEMO: ここにクリック後の処理を書く。メソッド名は変えてOK。文字表記処理の最中にも呼ばれ続けるので、文字表記処理中かどうかのフラグを用いた分岐処理を行うといい
        }

        #region Callbacks
        
        public async void MoveCastle()
        {
            try
            {
                await LoadManager.Instance.TryPlayBlackFadeIn();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            ChangeToCastleScene();
            try
            {
                await LoadManager.Instance.TryPlayFadeOut();
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Cancel Loading");
            }
            backGroundFilter.enabled = true;
            CallStartTalkInCastle();
        }

        public async void ExitCandy()
        {
            backGroundFilter.enabled = false;
            Debug.Log($"Start");
            await prologuePlayerAnimation.ExitCandy(_token);
            Debug.Log($"Finish");
            backGroundFilter.enabled = true;
            CallAfterExitCandy();
        }

        public async void ExitAllPrincess()
        {
            backGroundFilter.enabled = false;
            await prologuePlayerAnimation.ExitPrincess(_token);
            backGroundFilter.enabled = true;
            CallAfterExitPrincess();
        }

        public void MoveNextScene()
        {
            _toNextSceneEvent.Invoke();
        }

        #endregion
        
        
        
        public void CallStartNarration()
        {
            flowchart.SendFungusMessage(FungusCallMethodName.Narration);
        }

        private void CallStartTalkInCastle()
        {
            flowchart.SendFungusMessage(FungusCallMethodName.StartTalk);
        }

        private void CallAfterExitCandy()
        {
            flowchart.SendFungusMessage(FungusCallMethodName.AfterExitCandy);
        }

        private void CallAfterExitPrincess()
        {
            flowchart.SendFungusMessage(FungusCallMethodName.AfterExitPrincess);
        }

        private void ChangeToCastleScene()
        {
            worldMapObj.SetActive(false);
            castleObj.SetActive(true);
            backGroundFilter.enabled = false;
        }
    }
}