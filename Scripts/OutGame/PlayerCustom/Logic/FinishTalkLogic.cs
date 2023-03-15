using System.Collections.Generic;
using MyApplication;
using OutGame.PlayerCustom.Entity;
using OutGame.PlayerCustom.Input;
using OutGame.PlayerCustom.View;
using TalkSystem;
using UniRx;

namespace OutGame.PlayerCustom.Logic
{
    public class FinishTalkLogic
    {
        private readonly InSceneDataEntity _inSceneDataEntity;
        private readonly ToMessageWindowSenderView _toMessageWindowSenderView;

        public FinishTalkLogic(InSceneDataEntity inSceneDataEntity,ToMessageWindowSenderView toMessageWindowSenderView)
        {
            _inSceneDataEntity = inSceneDataEntity;
            _toMessageWindowSenderView = toMessageWindowSenderView;
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            _inSceneDataEntity.ChangedSettingsState
                .Where(state => state == PlayerCustomState.Finish)
                .Subscribe(_ =>
                {
                    _toMessageWindowSenderView.SetTalkKeyObserver(
                        new AllPlayerCustomNextKeyObserver(_inSceneDataEntity.RegisteredPlayerSelectController));
                }).AddTo(_toMessageWindowSenderView);
            
            _inSceneDataEntity.HadFinishedPopUpWindow
                .Where(state => state == PlayerCustomState.Finish)
                .Subscribe(_ => _toMessageWindowSenderView.SendCheerMessageEvent())
                .AddTo(_toMessageWindowSenderView);
        }
    }
}