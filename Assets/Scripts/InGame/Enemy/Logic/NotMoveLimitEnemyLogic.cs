using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Enemy.Entity;
using InGame.Enemy.View;
using MyApplication;
using UnityEngine;

namespace InGame.Enemy.Logic
{
    public class NotMoveLimitEnemyLogic:BaseEnemyLogic
    {
        public NotMoveLimitEnemyLogic(BaseEnemyEntity enemyEntity, BaseEnemyView enemyView) : base(enemyEntity, enemyView)
        {
            Init();
        }
        
        private void Init()
        {
            enemyView.SetActiveAnimator(false);
        }

        public override void UpdateEnemies()
        {
            if (enemyView == null || enemyView.state == EnemyState.Death)
            {
                return;
            }

            CheckHadFallen();
            TryFly();
            if (TryUpdateFlyState())
            {
                return;
            }
            if (TryUpdateActionByPlayer())
            {
                return;
            }
            if (!(enemyEntity.hadMoved||enemyView.inScreenX))//MEMO: 一度動き出したら画面外でも止まらない
            {
                return;
            }
            if (!enemyEntity.hadMoved)//MEMO: 初めて画面内に入ったら
            {
                enemyEntity.SetHadMoved();//MEMO: 一度でも画面内に入って動き出したかどうかを保持するフラグを設定
                enemyView.SetActiveAnimator(true);
            }
            if (enemyView.state==EnemyState.ChangeDirection)
            {
                return;
            }
            TryUpdateEatState();
            if (enemyView.state==EnemyState.Eat)
            {
                enemyView.SetVelocity(Vector2.zero);
                return;
            }

            Move();
            CheckIsCollideSideWall();
        }

        private void CheckIsCollideSideWall()
        {
            bool isFacedWall = IsFacedWall();
    
            if(isFacedWall)
            {
                ChangeState(EnemyState.ChangeDirection);
                enemyView.SetVelocity(Vector2.zero);

                enemyView.SetChangeDirectionTokenSource(new CancellationTokenSource());
        
                CancellationToken changeDirectionToken =
                    CancellationTokenSource.CreateLinkedTokenSource(enemyView.thisToken,
                        enemyView.ChangeDirectionToken).Token;
                ChangeDirectionTaskAsync(changeDirectionToken).Forget();
            }
        }
    }
}