using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
using OutGame.Database.ScriptableData;
using OutGame.Prologue;
using TalkSystem;
using UniRx;
using UnityEngine;
using Utility;

namespace OutGame.ColateStage
{
    public class ColateStageTalkPartBehaviour:MonoBehaviour,IDisposable
    {
        [SerializeField] Dialogs dialogs;
        [SerializeField] private SkipGaugeChargeView skipGaugeChargeView;
        [SerializeField] private Material backGroundMaterial;
        
        private Action _onBattleEvent;

        private OutGameDatabase _outGameDatabase;
        private AllTalkInputObserver _allTalkInputObserver;
        
        public void Init(Action onBattleEvent,OutGameDatabase outGameDatabase)
        {
            _onBattleEvent = onBattleEvent;
            _outGameDatabase = outGameDatabase;
            _allTalkInputObserver = new AllTalkInputObserver();
            dialogs.Init(_allTalkInputObserver,outGameDatabase.GetDialogFaceSpriteScriptableData());
            skipGaugeChargeView.Init(_allTalkInputObserver.OnSkip);
            Color colorCache = backGroundMaterial.color;
            backGroundMaterial.color = new Color(colorCache.r, colorCache.g, colorCache.b, 0);
        }
        
        public void StartTalkScene()
        {
            ColateStageScriptableData data = _outGameDatabase.GetColateStageScriptableData();
            _outGameDatabase.SetTalkObservableData(new TalkPartPlayerMoveData(TalkPartActionType.EnterBossStage,
                data.ColateStagePrincessEnterTime, data.ColateStageMoveXVec));
            RegisterObserver();
        }
        
        private void RegisterObserver()
        {
            _outGameDatabase.TalkPartActionFinished
                .Where(type=>type==TalkPartActionType.EnterBossStage)
                .Subscribe(_ =>
                {
                    Debug.Log($"TalkPartActionType:{TalkPartActionType.EnterBossStage}");
                    StartTalk();
                });
        }
        
        private async void StartTalk()
        {
            await backGroundMaterial.DOFade(1, _outGameDatabase.GetColateStageScriptableData().TalkPartFadeInDuration)
                .ToUniTask(cancellationToken:this.GetCancellationTokenOnDestroy());
            BGMManager.Instance.Play(BGMPath.PROLOGUE);
            dialogs.StartDialogs();
            ActiveSkip();
        }

        private async void ActiveSkip()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_outGameDatabase.GetTalkPartUIScriptableData().ToSkipFadeInTime),
                cancellationToken: this.GetCancellationTokenOnDestroy());
            skipGaugeChargeView.PlayFadeInObjects(_outGameDatabase.GetTalkPartUIScriptableData().SkipFadeInDuration);
            skipGaugeChargeView.RegisterSkipObserver(StartBattle, null,
                _outGameDatabase.GetTalkPartUIScriptableData().SkipGaugeDuration);
        }
        

        #region Callbacks

        public async void StartBattle()
        {
            dialogs.ExitDialog();
            skipGaugeChargeView.PlayFadeOutObjects(_outGameDatabase.GetTalkPartUIScriptableData().SkipFadeOutDuration);
            await backGroundMaterial.DOFade(0, _outGameDatabase.GetColateStageScriptableData().TalkPartFadeOutDuration)
                .ToUniTask(cancellationToken:this.GetCancellationTokenOnDestroy());
            dialogs.DisposeDialog();
            _onBattleEvent.Invoke();
        }

        #endregion

        public void Dispose()
        {
            _allTalkInputObserver?.Dispose();
        }
    }
}