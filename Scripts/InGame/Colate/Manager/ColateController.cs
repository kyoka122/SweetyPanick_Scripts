using System;
using InGame.Colate.Logic;
using UniRx;

namespace InGame.Colate.Manager
{
    public class ColateController:IDisposable
    {
        public IObservable<bool> IsColateDead=>_isColateDead;
        
        private BaseColateStateLogic _colateStateLogic;
        private readonly ColateStatusLogic _colateStatusLogic;
        private readonly IDisposable[] _disposables;
        private readonly Subject<bool> _isColateDead;
        
        public ColateController(BaseColateStateLogic colateStateLogic,ColateStatusLogic colateStatusLogic,
            IDisposable[] disposables)
        {
            _colateStateLogic = colateStateLogic;
            _colateStatusLogic = colateStatusLogic;
            _disposables = disposables;
            _isColateDead = new Subject<bool>();
        }

        public void StartTalk()
        {
            _colateStateLogic.SetIsTalking(true);
        }

        public void StartBattle()
        {
            _colateStateLogic.SetIsTalking(false);
            _colateStatusLogic.ActiveHpPanel();
            _colateStatusLogic.OnFinishColateBattle
                .Subscribe(_ => _isColateDead.OnNext(true));
        }

        public void FixedUpdate()
        {
            _colateStateLogic=_colateStateLogic.Process();
        }

        public void Dispose()
        {
            _isColateDead?.Dispose();
            _colateStatusLogic.Dispose();
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }
        }
    }
}