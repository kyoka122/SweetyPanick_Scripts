using System;

namespace Common.Interface
{
    public interface IAnimationCallbackSender
    {
        /// <summary>
        /// アニメーションイベントが呼ばれた時
        /// （必ずSubject側でDisposeを呼ぶこと）
        /// </summary>
        public IObservable<string> OnAnimationEvent { get; }
    }
}