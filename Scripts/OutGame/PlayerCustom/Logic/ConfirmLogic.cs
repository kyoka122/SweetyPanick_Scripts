using MyApplication;
using OutGame.PlayerCustom.Entity;
using OutGame.PlayerCustom.View;
using UniRx;

namespace OutGame.PlayerCustom.Logic
{
    public class ConfirmLogic
    {
        private readonly InSceneDataEntity _inSceneDataEntity;
        private readonly ToMessageWindowSenderView _toMessageWindowSenderView;

        public ConfirmLogic(InSceneDataEntity inSceneDataEntity,ToMessageWindowSenderView toMessageWindowSenderView)
        {
            _inSceneDataEntity = inSceneDataEntity;
            _toMessageWindowSenderView = toMessageWindowSenderView;
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            _inSceneDataEntity.hadFinishedPopUpWindow
                .Where(state => state == PlayerCustomState.Finish)
                .Subscribe(_ => _toMessageWindowSenderView.SendCheerMessageEvent())
                .AddTo(_toMessageWindowSenderView);
        }
    }
}