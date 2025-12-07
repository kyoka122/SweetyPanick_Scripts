using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Loader;
using OutGame.Database;
using TalkSystem;
using UnityEngine;
using Utility;

namespace OutGame.Prologue
{
    public class PrologueBehaviour:MonoBehaviour,IDisposable
    {
        [SerializeField] private Dialogs dialogs;
        [SerializeField] private ProloguePlayerAnimation prologuePlayerAnimation;

        [SerializeField] private GameObject castleObj;
        [SerializeField] private GameObject worldMapObj;
        [SerializeField] private SpriteRenderer backGroundFilter;

        [SerializeField] private SkipGaugeChargeView skipGaugeChargeView;
        
        private CancellationToken _token;
        private Action _toNextSceneEvent;
        private AllTalkInputObserver _allTalkInputObserver;
        private OutGameDatabase _outGameDatabase;

        private bool _isPlayingSkipGaugeAnimation;
        
        public void Init(Action toNextSceneEvent,OutGameDatabase outGameDatabase)
        {
            _toNextSceneEvent = toNextSceneEvent;
            _outGameDatabase = outGameDatabase;
            _allTalkInputObserver = new AllTalkInputObserver();
            dialogs.Init(_allTalkInputObserver,outGameDatabase.GetDialogFaceSpriteScriptableData());
            prologuePlayerAnimation.Init();
            skipGaugeChargeView.Init(_allTalkInputObserver.OnSkip);
            
            worldMapObj.SetActive(true);
            castleObj.SetActive(false);
            backGroundFilter.enabled = true;
        }

        public void CallStartNarration()
        {
            Debug.Log($"StartNarration");
            dialogs.StartDialogs();
            ActiveSkip();
        }
        
        private async void ActiveSkip()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_outGameDatabase.GetTalkPartUIScriptableData().ToSkipFadeInTime),
                cancellationToken: this.GetCancellationTokenOnDestroy());
            skipGaugeChargeView.PlayFadeInObjects(_outGameDatabase.GetTalkPartUIScriptableData().SkipFadeInDuration);
            skipGaugeChargeView.RegisterSkipObserver(MoveNextScene, null,
                _outGameDatabase.GetTalkPartUIScriptableData().SkipGaugeDuration);
        }
        
       
        private void ChangeToCastleScene()
        {
            worldMapObj.SetActive(false);
            castleObj.SetActive(true);
            backGroundFilter.enabled = false;
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
            dialogs.SetFinishAction();
        }

        public async void ExitCandy()
        {
            backGroundFilter.enabled = false;
            Debug.Log($"Start");
            await prologuePlayerAnimation.ExitCandy(_token);
            Debug.Log($"Finish");
            backGroundFilter.enabled = true;
            dialogs.SetFinishAction();
        }

        public async void ExitAllPrincess()
        {
            backGroundFilter.enabled = false;
            await prologuePlayerAnimation.ExitPrincess(_token);
            backGroundFilter.enabled = true;
            dialogs.SetFinishAction();
        }

        public void MoveNextScene()
        {
            dialogs.ExitDialog();
            dialogs.DisposeDialog();
            dialogs.UnloadBigResource();//MEMO: Textureのサイズが大きく負荷になるため、使用後にUnloadしておく。
            _toNextSceneEvent.Invoke();
        }

        #endregion

        public void Dispose()
        {
            _allTalkInputObserver.Dispose();
        }
    }
}