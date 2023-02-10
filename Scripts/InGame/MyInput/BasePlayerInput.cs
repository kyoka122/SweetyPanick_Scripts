using System;
using UniRx;

namespace InGame.MyInput
{
    //MEMO: PCキー、Joyconの両方を使えるようにするための基底クラス
    public abstract class BasePlayerInput:IDisposable
    {
        public IReadOnlyReactiveProperty<float> Move=>_move;
        public IReadOnlyReactiveProperty<int> PlayerSelectDirection=>_playerSelectDirection;
        public IReadOnlyReactiveProperty<bool> Jump => _jump;
        public IReadOnlyReactiveProperty<bool> Punch => _punch;
        public IReadOnlyReactiveProperty<bool> Skill => _skill;
        public IReadOnlyReactiveProperty<bool> Fix => _fix;
        public IReadOnlyReactiveProperty<bool> PlayerSelector => _playerSelector;

        protected readonly ReactiveProperty<float> _move;
        protected readonly ReactiveProperty<int> _playerSelectDirection;
        protected readonly ReactiveProperty<bool> _jump;
        protected readonly ReactiveProperty<bool> _punch;
        protected readonly ReactiveProperty<bool> _skill;
        protected readonly ReactiveProperty<bool> _fix;
        protected readonly ReactiveProperty<bool> _playerSelector;

        public abstract void Rumble();
        
        protected BasePlayerInput()
        {
            _move = new ReactiveProperty<float>(0);
            _playerSelectDirection = new ReactiveProperty<int>();
            _jump = new ReactiveProperty<bool>();
            _punch = new ReactiveProperty<bool>();
            _skill = new ReactiveProperty<bool>();
            _fix = new ReactiveProperty<bool>();
            _playerSelector = new ReactiveProperty<bool>(false);
        }

        public void Dispose()
        {
            _move?.Dispose();
            _playerSelectDirection?.Dispose();
            _jump?.Dispose();
            _punch?.Dispose();
            _skill?.Dispose();
            _fix?.Dispose();
            _playerSelector?.Dispose();
        }
    }
}