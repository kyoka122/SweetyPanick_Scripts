using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OutGame.Database;
using OutGame.Prologue;
using TalkSystem;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace OutGame.Epilogue
{
    public class EpilogueBehaviour:MonoBehaviour,IDisposable
    {
        [SerializeField] Dialogs dialogs;
        [SerializeField] private SkipGaugeChargeView skipGaugeChargeView;
        
        private AllTalkInputObserver _allTalkInputObserver;
        private OutGameDatabase _outGameDatabase;
        private CancellationToken _token;
        private Action _toNextSceneEvent;
        
        
        public void Init(Action toNextSceneEvent,OutGameDatabase outGameDatabase)
        {
            _toNextSceneEvent = toNextSceneEvent;
            _outGameDatabase = outGameDatabase;
            _allTalkInputObserver = new AllTalkInputObserver();
            dialogs.Init(_allTalkInputObserver,outGameDatabase.GetDialogFaceSpriteScriptableData());
            skipGaugeChargeView.Init(_allTalkInputObserver.OnSkip);
        }

        public void CallStartTalk()
        {
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

        private void MoveNextScene()
        {
            dialogs.ExitDialog();
            dialogs.DisposeDialog();
            _toNextSceneEvent.Invoke();
        }

        #region Callbacks
        

        #endregion

        public void Dispose()
        {
            _allTalkInputObserver?.Dispose();
        }
    }
}