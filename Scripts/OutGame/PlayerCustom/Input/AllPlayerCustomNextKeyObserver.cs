using System.Collections.Generic;
using Common.MyInput.PlayerCustom;
using MyApplication;
using TalkSystem;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace OutGame.PlayerCustom.Input
{
    /// <summary>
    /// PlayerCustomシーン用InputMapより、全コントローラーのNextを監視する
    /// </summary>
    public class AllPlayerCustomNextKeyObserver : BaseTalkKeyObserver
    {
        private readonly IReadOnlyList<BasePlayerCustomInput> _talkInputs;
        
        public AllPlayerCustomNextKeyObserver(IReadOnlyList<BasePlayerCustomInput> talkInputs)
        {
            _talkInputs = talkInputs;
            foreach (var input in _talkInputs)
            {
                RegisterObserver(input);
            }
            Debug.Log($"Init Input.Count:{_talkInputs.Count}");
        }

        private void RegisterObserver(BasePlayerCustomInput input)
        {
            input.Next.Where(on=>on).Subscribe(_ =>
            {
                onNext.OnNext(true);
            });
        }
        
        public override void Dispose()
        {
            foreach (var talkInput in _talkInputs)
            {
                talkInput?.Dispose();
            }
            base.Dispose();
        }
    }
}