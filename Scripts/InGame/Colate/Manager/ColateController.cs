using System;
using System.Collections.Generic;
using InGame.Colate.Logic;
using MyApplication;
using UniRx;

namespace InGame.Colate.Manager
{
    public class ColateController:IDisposable
    {
        public IObservable<bool> IsColateDead=>_isColateDead;
        
        private BaseColateStateLogic _colateStateLogic;
        private readonly IDisposable[] _disposables;
        private readonly Subject<bool> _isColateDead;
        
        public ColateController(BaseColateStateLogic colateStateLogic,IDisposable[] disposables)
        {
            _colateStateLogic = colateStateLogic;
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
        }

        public void FixedUpdate()
        {
            _colateStateLogic=_colateStateLogic.Process();
            if (_colateStateLogic.state==ColateState.Dead)
            {
                _isColateDead.OnNext(true);
            }
        }

        public void Dispose()
        {
            _isColateDead.Dispose();
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}