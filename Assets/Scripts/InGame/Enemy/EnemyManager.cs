using System.Collections.Generic;
using System.Linq;
using InGame.Enemy.Logic;

namespace InGame.Enemy
{
    public class EnemyManager
    {
        private readonly List<BaseEnemyLogic> _enemyLogics;

        public EnemyManager(List<BaseEnemyLogic> enemyLogics)
        {
            _enemyLogics = enemyLogics;
        }
        
        public EnemyManager()
        {
            _enemyLogics = new List<BaseEnemyLogic>();
        }

        public void FixedUpdate()
        {
            var logics = new List<BaseEnemyLogic>(_enemyLogics);
            foreach (var enemyLogic in logics)
            {
                enemyLogic.UpdateEnemies();
                if (enemyLogic.IsDeath())
                {
                    _enemyLogics.Remove(enemyLogic);
                }
            }
        }

        public void AddEnemy(BaseEnemyLogic enemyLogic)
        {
            _enemyLogics.Add(enemyLogic);
        }
    }
}