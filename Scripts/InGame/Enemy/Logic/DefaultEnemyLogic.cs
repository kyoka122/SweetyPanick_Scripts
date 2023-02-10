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
            if (_enemyView == null || _enemyView.state == EnemyState.Death)
            {
                return;
            }

            CheckHadFallen();
            CheckOutOfScreen();
       
            if (_enemyView.state == EnemyState.Punched)
            {
                Fly();
                _enemyView.SetState(EnemyState.Fly);
            }
            if (TryUpdateFlyState())
            {
                return;
            }
            if (TryUpdateActionByPlayer())
            {
                return;
            }
            if (!_enemyView.inScreenX)
            {
                return;
            }
            if (_enemyView.state==EnemyState.ChangeDirection)
            {
                return;
            }

            TryUpdateEatState();
            if (_enemyView.state==EnemyState.Eat)
            {
                return;
            }

            Move();
            CheckMoveLimit();
        }
    }
}