using System.Threading;
using Cysharp.Threading.Tasks;
using MyApplication;
using UnityEngine;

namespace InGame.Stage.View
{
    public class MarshmallowView:DefaultGimmickSweetsView,IBoundAble
    {
        public float BoundPower => boundPower;
        [SerializeField] private Animator animator;
        [SerializeField] private float boundPower = 1;
        
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
            gameObject.layer = LayerInfo.MarshmallowTrampolineNum;
        }
        
        public async virtual UniTask BreakSweets(float duration, CancellationToken token)
        {
            //MEMO: ギミックスイーツを壊せるようにするならこのvirtualメソッドを削除
        }
    }
}