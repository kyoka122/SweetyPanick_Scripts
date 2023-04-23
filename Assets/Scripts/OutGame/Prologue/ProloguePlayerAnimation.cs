using System.Threading;
using Cysharp.Threading.Tasks;
using OutGame.PlayerTalks;
using UnityEngine;

namespace OutGame.Prologue
{
    public class ProloguePlayerAnimation:MonoBehaviour
    {
        [SerializeField] private float outOfScreenViewX;
        [SerializeField] private float candyExitTime=3;
        [SerializeField] private float princessExitTime=3;

        [SerializeField] private Camera camera;
        [SerializeField] private PlayerAnimations candy;
        [SerializeField] private PlayerAnimations mash;
        [SerializeField] private PlayerAnimations fu;
        [SerializeField] private PlayerAnimations kure;

        
        private Vector2 _outOfScreenView;
        
        public void Init()
        {
            _outOfScreenView = new Vector2(outOfScreenViewX, 0);
            candy.Init();
            mash.Init();
            fu.Init();
            kure.Init();
            candy.UpdatePlayerDirection(1);
            mash.UpdatePlayerDirection(1);
            kure.UpdatePlayerDirection(1);
            fu.UpdatePlayerDirection(1);
        }
        
        public async UniTask ExitCandy(CancellationToken token)
        {
            candy.UpdatePlayerDirection(-1);
            candy.OnRunAnimation();
            await candy.Move(camera, _outOfScreenView, candyExitTime,token);
        }

        public async UniTask ExitPrincess(CancellationToken token)
        {
            mash.UpdatePlayerDirection(-1);
            fu.UpdatePlayerDirection(-1);
            kure.UpdatePlayerDirection(-1);
            
            mash.OnRunAnimation();
            fu.OnRunAnimation();
            kure.OnRunAnimation();
            
            UniTask mashTask = mash.Move(camera, _outOfScreenView, princessExitTime, token);
            UniTask fuTask = fu.Move(camera,_outOfScreenView, princessExitTime, token);
            UniTask kureTask = kure.Move(camera, _outOfScreenView, princessExitTime, token);
            await UniTask.WhenAll(mashTask, fuTask, kureTask);
        }
    }
}