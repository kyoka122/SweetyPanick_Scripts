using InGame.Enemy.Entity;
using InGame.Enemy.View;
using MyApplication;
using UnityEngine;

namespace InGame.Enemy.Logic
{
    public class DefaultEnemyLogic:BaseEnemyLogic
    {
        //private readonly DefaultEnemyEntity _defaultEnemyEntity;

        public DefaultEnemyLogic(DefaultEnemyEntity enemyEntity,DefaultEnemyView enemyView)
            : base(enemyEntity,enemyView)
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
            CheckOutOfScreen();
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
                enemyView.SetVelocity(Vector2.zero);
                return;
            }
            TryUpdateEatState();
            if (enemyView.state==EnemyState.Eat)
            {
                enemyView.SetVelocity(Vector2.zero);
                return;
            }

            Move();
            CheckMoveLimit();
        }
    }
}