using System;
using System.Threading;
using MyApplication;
using OutGame.Database;
using OutGame.Database.ScriptableData;
using UniRx;
using UnityEngine;

namespace OutGame.ColateStage
{
    public class ColateStageTalkPartBehaviour:MonoBehaviour
    {
        [SerializeField] Fungus.Flowchart flowchart;

        private CancellationToken _token;
        private Action _onBattleEvent;

        private OutGameDatabase _outGameDatabase;
        
        public void Init(Action onBattleEvent,OutGameDatabase outGameDatabase)
        {
            _onBattleEvent = onBattleEvent;
            _outGameDatabase = outGameDatabase;
            RegisterObserver();
        }

        public void StartTalkScene()
        {
            BossStageScriptableData data = _outGameDatabase.GetBossStageScriptableData();
            _outGameDatabase.SetTalkObservableData(new TalkPartPlayerMoveData(TalkPartActionType.EnterBossStage,
                data.BossStagePrincessEnterTime, data.BossStageMoveXVec));
        }

        #region Callbacks

        public void StartBattle()
        {
            _onBattleEvent.Invoke();
        }

        #endregion
        
        private void StartTalk()
        {
            flowchart.SendFungusMessage(FungusCallMethodName.StartTalk);
        }
        
        private void RegisterObserver()
        {
            _outGameDatabase.TalkPartActionFinished
                .Where(type=>type==TalkPartActionType.EnterBossStage)
                .Subscribe(_ =>
                {
                    StartTalk();
                });
        }
    }
}