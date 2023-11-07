using System;
using Common.MyInput.Player;
using MyApplication;
using UniRx;
using UnityEngine;

namespace Common.MyInput.PlayerCustom
{
    //MEMO: PlayerCustomシーンでPCキー、Joyconの両方を使えるようにするための基底クラス
    public abstract class BasePlayerCustomInput:IDisposable
    {
        public MyInputDeviceType DeviceType { get; }
        public int DeviceId { get; }
        public IReadOnlyReactiveProperty<float> HorizontalValue=>horizontalValue;
        public IReadOnlyReactiveProperty<float> VerticalValue => verticalValue;
        
        public IReadOnlyReactiveProperty<int> HorizontalDigitalMoveValue=>horizontalDigitalMoveValue;
        public IReadOnlyReactiveProperty<int> VerticalDigitalMoveValue => verticalDigitalMoveValue;
        public IReadOnlyReactiveProperty<bool> Next => next;
        public IReadOnlyReactiveProperty<bool> Back => back;
        
        protected readonly ReactiveProperty<float> horizontalValue;
        protected readonly ReactiveProperty<float> verticalValue;
        protected readonly ReactiveProperty<int> horizontalDigitalMoveValue;
        protected readonly ReactiveProperty<int> verticalDigitalMoveValue;
        protected readonly ReactiveProperty<bool> next;
        protected readonly ReactiveProperty<bool> back;
        
        protected const float SelectorCanMoveStickValue = 0.8f;
        protected const float SelectorNotMoveStickValue = 0.6f;
        
        protected BasePlayerCustomInput(MyInputDeviceType type,int deviceId)
        {
            DeviceType = type;
            DeviceId = deviceId;
            horizontalValue = new ReactiveProperty<float>();
            verticalValue = new ReactiveProperty<float>();
            verticalDigitalMoveValue = new ReactiveProperty<int>();
            horizontalDigitalMoveValue = new ReactiveProperty<int>();
            next = new ReactiveProperty<bool>();
            back = new ReactiveProperty<bool>();
        }

        /// <summary>
        /// しきい値1を超えたら1か-1 (スティックの方向による)を返し、しきい値2を下回ったら0を返す。しきい値1からしきい値2までの間だった場合、"currentValue"を返す
        /// </summary>
        /// <param name="stickValue"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        protected int GetStickDigitalDirection(float stickValue,int currentValue)
        {
            int moveDirection = 1;

            //MEMO: スティックが右入力か左入力か
            moveDirection *= (int) Mathf.Sign(stickValue);
            
            //MEMO: スティックの押し込み具合
            if (Mathf.Abs(stickValue) > SelectorCanMoveStickValue)
            {
                return moveDirection;
            }
            if (Mathf.Abs(stickValue) < SelectorNotMoveStickValue)
            {
                return 0;
            }

            return currentValue;
        }

        public virtual BasePlayerCustomInput Clone()
        {
            return MemberwiseClone() as BasePlayerCustomInput;
        }

        public virtual void Dispose()
        {
            horizontalValue?.Dispose();
            verticalValue?.Dispose();
            verticalDigitalMoveValue?.Dispose();
            horizontalDigitalMoveValue?.Dispose();
            next?.Dispose();
            back?.Dispose();
        }
    }
}