using System;
using UnityEngine;

namespace InGame.Common.Interface
{
    public interface IAnimationCallbackSender
    {
        /// <summary>
        /// 必ずSubject側でDisposeを呼ぶこと
        /// </summary>
        public IObservable<string> OnAnimationEvent { get; }
    }
}