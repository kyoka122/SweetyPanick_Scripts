using System.Threading;
using Cysharp.Threading.Tasks;
using MyApplication;
using UnityEngine;

namespace InGame.Stage.View
{
    public class MarshmallowView:DefaultGimmickSweetsView,IBoundAble
    {
        [SerializeField] private Animator animator;
        
        public bool BoundAble => fixState==FixState.Fixed;
        
        public void PlayPressAnimation()
        {
            if (fixState==FixState.Fixed)
            {
                animator.SetTrigger(StageAnimatorParameter.OnPress);
            }
        }

        protected override void EachSweetsEvent()
        {
            gameObject.layer = LayerInfo.MarshmallowTrampoline;
        }
        
        public async virtual UniTask BreakSweets(float duration, CancellationToken token)
        {
            //MEMO: ギミックスイーツを壊せるようにするならこのvirtualメソッドを削除
        }
    }
}