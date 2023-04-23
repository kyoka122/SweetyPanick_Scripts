using InGame.Enemy.Entity;
using InGame.Enemy.View;
using MyApplication;

namespace InGame.Enemy.Logic
{
    public class StationaryEnemyLogic:BaseEnemyLogic
    {
        public StationaryEnemyLogic(BaseEnemyEntity enemyEntity, BaseEnemyView enemyView) : base(enemyEntity, enemyView)
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

            TryUpdateEatState();
        }

        
    }
}