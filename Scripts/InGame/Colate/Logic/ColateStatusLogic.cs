using System;
using InGame.Colate.Entity;
using InGame.Colate.View;
using UniRx;

namespace InGame.Colate.Logic
{
    public class ColateStatusLogic:IDisposable
    {
        public IObservable<bool> OnFinishColateBattle => _onFinishColateBattle;
        
        private readonly ColateEntity _colateEntity;
        private readonly ColateStatusView _colateStatusView;
        private readonly ColateView _colateView;
        private readonly Subject<bool> _onFinishColateBattle;
        
        public ColateStatusLogic(ColateEntity colateEntity, ColateStatusView colateStatusView,ColateView colateView)
        {
            _colateEntity = colateEntity;
            _colateStatusView = colateStatusView;
            _colateView = colateView;
            _onFinishColateBattle = new Subject<bool>();
            RegisterObserver();
        }

        public void ActiveHpPanel()
        {
            _colateStatusView.SetActive(true);
        }

        private void RegisterObserver()
        {
            _colateEntity.ColateHpChange
                .Subscribe(_colateStatusView.SetHpCalue)
                .AddTo(_colateStatusView);
            
            _colateEntity.DeadMotionFinished
                .Subscribe(_ => _onFinishColateBattle.OnNext(true))
                .AddTo(_colateView);
        }

        public void Dispose()
        {
            _onFinishColateBattle?.Dispose();
        }
    }
}