using System;
using UniRx;
using UnityEngine;

namespace InGame.MyInput
{
    //MEMO: PCキー、Joyconの両方を使えるようにするための基底クラス
    public abstract class BasePlayerInput:IDisposable
    {
        public IReadOnlyReactiveProperty<float> Move=>move;
        public IReadOnlyReactiveProperty<int> PlayerSelectDirection=>playerSelectDirection;
        public IReadOnlyReactiveProperty<bool> Jump => jump;
        public IReadOnlyReactiveProperty<bool> Punch => punch;
        public IReadOnlyReactiveProperty<bool> Skill => skill;
        public IReadOnlyReactiveProperty<bool> Fix => fix;
        public IReadOnlyReactiveProperty<bool> PlayerSelector => playerSelector;

        protected readonly ReactiveProperty<float> move;
        protected readonly ReactiveProperty<int> playerSelectDirection;
        protected readonly ReactiveProperty<bool> jump;
        protected readonly ReactiveProperty<bool> punch;
        protected readonly ReactiveProperty<bool> skill;
        protected readonly ReactiveProperty<bool> fix;
        protected readonly ReactiveProperty<bool> playerSelector;

        protected const float SelectorCanMoveValue = 0.7f;
        protected const float SelectorNotMoveValue = 0.3f;
        
        /// <summary>
        /// 振動に対応しているコントローラーであれば振動の処理を追加
        /// </summary>
        public Action rumbleEvent;

        protected BasePlayerInput()
        {
            move = new ReactiveProperty<float>(0);
            playerSelectDirection = new ReactiveProperty<int>();
            jump = new ReactiveProperty<bool>();
            punch = new ReactiveProperty<bool>();
            skill = new ReactiveProperty<bool>();
            fix = new ReactiveProperty<bool>();
            playerSelector = new ReactiveProperty<bool>(false);
        }
        
        protected int GetStickDigitalDirection(float stickValue,int currentValue)
        {
            int moveDirection = 1;

            //MEMO: スティックが右入力か左入力か
            moveDirection *= (int) Mathf.Sign(stickValue);
            
            //MEMO: スティックの押し込み具合
            if (Mathf.Abs(stickValue) > SelectorCanMoveValue)
            {
                return moveDirection;
            }
            if (Mathf.Abs(stickValue) < SelectorNotMoveValue)
            {
                return 0;
            }

            return currentValue;
        }

        public virtual void Dispose()
        {
            move?.Dispose();
            playerSelectDirection?.Dispose();
            jump?.Dispose();
            punch?.Dispose();
            skill?.Dispose();
            fix?.Dispose();
            playerSelector?.Dispose();
        }
    }
}