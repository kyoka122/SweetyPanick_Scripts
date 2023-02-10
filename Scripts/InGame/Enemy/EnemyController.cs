using System.Collections.Generic;
using System.Linq;
using InGame.Enemy.Logic;

namespace InGame.Enemy
{
    public class EnemyController
    {
        private readonly BaseEnemyLogic[] _enemyLogics;
        private readonly DefaultEnemyLogic[] _defaultEnemyLogics;
        private readonly EternitySleepEnemyLogic[] _eternitySleepEnemyLogics;

        public EnemyController(DefaultEnemyLogic[] defaultEnemyLogics,EternitySleepEnemyLogic[] eternitySleepEnemyLogics)
        {
            _defaultEnemyLogics = defaultEnemyLogics;
            _eternitySleepEnemyLogics = eternitySleepEnemyLogics;
            List<BaseEnemyLogic> logics = new List<BaseEnemyLogic>();
            logics.AddRange(defaultEnemyLogics);
            logics.AddRange(eternitySleepEnemyLogics);
            _enemyLogics = logics.ToArray();
        }

        public void FixedUpdate()
        {
            foreach (var enemyLogic in _enemyLogics)
            {
                enemyLogic.UpdateEnemies();
            }
        }
    }
}