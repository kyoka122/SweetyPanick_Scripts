using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InGame.SceneLoader;
using UnityEngine;

namespace OutGame.Prologue
{
    public class PrologueBehaviour:MonoBehaviour
    {
        [SerializeField] Fungus.Flowchart flowchart;
        [SerializeField] private PlayerAnimations playerAnimations;
        [SerializeField] private GameObject castleObj;
        [SerializeField] private GameObject worldMapObj;
        [SerializeField] private SpriteRenderer backGroundFilter;

        [SerializeField] private float fadeInDuration=1f;
        [SerializeField] private float fadeOutDuration=1f;
        [SerializeField] private float fadingDuration=1f;
        
        private ScreenFader _screenFader;
        private CancellationToken _token;
        private Action _toNextSceneEvent;
        
        public void Init(Action toNextSceneEvent)
        {
            _toNextSceneEvent = toNextSceneEvent;
            playerAnimations.Init();
            worldMapObj.SetActive(true);
            castleObj.SetActive(false);
            backGroundFilter.enabled = true;
        }

        #region Callback
        
        public async void MoveCastle()
        {
            await _screenFader.FadeInOut(fadeInDuration, fadeOutDuration,fadingDuration,Ease.InQuad, _token,ChangeToCastleScene);
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(fadingDuration), cancellationToken:_token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"CanceledDelayAfterFade");
            }

            backGroundFilter.enabled = true;
            CallStartTalkInCastle();
        }

        public async void ExitCandy()
        {
            backGroundFilter.enabled = false;
            Debug.Log($"Start");
            await playerAnimations.ExitCandy(_token);
            Debug.Log($"Finish");
            backGroundFilter.enabled = true;
            CallAfterExitCandy();
        }

        public async void ExitAllPrincess()
        {
            backGroundFilter.enabled = false;
            await playerAnimations.ExitPrincess(_token);
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
            flowchart.SendFungusMessage("Narration");
        }

        private void CallStartTalkInCastle()
        {
            flowchart.SendFungusMessage("StartTalk");
        }

        private void CallAfterExitCandy()
        {
            flowchart.SendFungusMessage("AfterExitCandy");
        }

        private void CallAfterExitPrincess()
        {
            flowchart.SendFungusMessage("AfterExitPrincess");
        }

        private void ChangeToCastleScene()
        {
            worldMapObj.SetActive(false);
            castleObj.SetActive(true);
            backGroundFilter.enabled = false;
        }

    }
}