using System;
using UnityEngine;

namespace Common.Interface
{
    public interface IAnimationCallbackSender
    {
        /// <summary>
        /// 必ずSubject側でDisposeを呼ぶこと
        /// </summary>
        public IObservable<string> OnAnimationEvent { get; }
    }
}