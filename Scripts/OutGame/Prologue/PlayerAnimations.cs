using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OutGame.Prologue
{
    public class PlayerAnimations:MonoBehaviour
    {
        [SerializeField] private float outOfScreenViewX;
        [SerializeField] private float candyExitTime=3;
        [SerializeField] private float princessExitTime=3;

        [SerializeField] private Camera camera;
        [SerializeField] private ProloguePlayerAnimation candy;
        [SerializeField] private ProloguePlayerAnimation mash;
        [SerializeField] private ProloguePlayerAnimation fu;
        [SerializeField] private ProloguePlayerAnimation kure;

        
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
            await candy.Exit(camera, _outOfScreenView, candyExitTime,token);
        }

        public async UniTask ExitPrincess(CancellationToken token)
        {
            mash.UpdatePlayerDirection(-1);
            fu.UpdatePlayerDirection(-1);
            kure.UpdatePlayerDirection(-1);
            
            mash.OnRunAnimation();
            fu.OnRunAnimation();
            kure.OnRunAnimation();
            
            UniTask mashTask = mash.Exit(camera, _outOfScreenView, princessExitTime, token);
            UniTask fuTask = fu.Exit(camera,_outOfScreenView, princessExitTime, token);
            UniTask kureTask = kure.Exit(camera, _outOfScreenView, princessExitTime, token);
            await UniTask.WhenAll(mashTask, fuTask, kureTask);
        }
    }
}