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
            if (!enemyView.inScreenX)
            {
                //TODO: アニメーション停止処理追加
                return;
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
            CheckMoveLimit();
        }
    }
}