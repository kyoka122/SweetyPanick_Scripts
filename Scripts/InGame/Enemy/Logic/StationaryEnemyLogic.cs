using InGame.Enemy.Entity;
using InGame.Enemy.View;
using MyApplication;

namespace InGame.Enemy.Logic
{
    public class StationaryEnemyLogic:BaseEnemyLogic
    {
        public StationaryEnemyLogic(BaseEnemyEntity enemyEntity, BaseEnemyView enemyView) : base(enemyEntity, enemyView)
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
       
            if (enemyView.state == EnemyState.Punched)
            {
                TryFly();
                enemyView.SetState(EnemyState.Fly);
            }
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
                return;
            }
            if (enemyView.state==EnemyState.ChangeDirection)
            {
                return;
            }

            TryUpdateEatState();
        }

        
    }
}