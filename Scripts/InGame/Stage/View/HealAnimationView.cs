using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyApplication;
using UnityEngine;

namespace InGame.Stage.View
{
    public class HealAnimationView:MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        public CancellationToken GetCancellationToken()
        {
            return this.GetCancellationTokenOnDestroy();
        }

        public void PlayHealAnimation()
        {
            animator.SetTrigger(StageAnimatorParameter.OnHeal);
        }

        public bool IsPlayingHealAnimation()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(StageAnimationClipName.Heal);
        }
    }
}