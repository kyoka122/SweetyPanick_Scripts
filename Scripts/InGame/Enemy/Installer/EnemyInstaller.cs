using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Database;
using InGame.Enemy.Entity;
using InGame.Enemy.Logic;
using InGame.Enemy.View;
using UnityEngine;

namespace InGame.Enemy.Installer
{
    public class EnemyInstaller:MonoBehaviour
    {
        public EnemyController Install(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            BaseEnemyView[] enemyViews=FindObjectsOfType<BaseEnemyView>();
            DefaultEnemyView[] defaultEnemyViews = GetDefaultEnemyViews(enemyViews);
            EternitySleepEnemyView[] eternitySleepEnemyViews = GetEternitySleepEnemyViews(enemyViews);

            var defaultEnemyLogics = new List<DefaultEnemyLogic>();
            var eternitySleepEnemyLogics = new List<EternitySleepEnemyLogic>();
            
            foreach (var defaultEnemyView in defaultEnemyViews)
            {
                defaultEnemyLogics.Add(new DefaultEnemyLogic(new DefaultEnemyEntity(inGameDatabase,commonDatabase),defaultEnemyView));
            }

            foreach (var eternitySleepEnemy in eternitySleepEnemyViews)
            {
                eternitySleepEnemyLogics.Add(new EternitySleepEnemyLogic(new EternitySleepEnemyEntity(inGameDatabase,commonDatabase),eternitySleepEnemy));
            }
            var enemyController = new EnemyController(defaultEnemyLogics.ToArray(),eternitySleepEnemyLogics.ToArray());

            return enemyController;
        }

        private DefaultEnemyView[] GetDefaultEnemyViews(BaseEnemyView[] enemyViews)
        {
            List<DefaultEnemyView> defaultEnemyViewEntities = new List<DefaultEnemyView>();
            foreach (var enemyView in enemyViews)
            {
                if (enemyView.TryGetComponent(out DefaultEnemyView defaultEnemyView))
                {
                    defaultEnemyViewEntities.Add(defaultEnemyView);
                    defaultEnemyView.Init();
                }
            }

            return defaultEnemyViewEntities.ToArray();
        }
        
        private EternitySleepEnemyView[] GetEternitySleepEnemyViews(BaseEnemyView[] enemyViews)
        {
            List<EternitySleepEnemyView> eternitySleepEnemyViewEntities = new List<EternitySleepEnemyView>();
            foreach (var enemyView in enemyViews)
            {
                if (enemyView.TryGetComponent(out EternitySleepEnemyView eternitySleepEnemyView))
                {
                    eternitySleepEnemyViewEntities.Add(eternitySleepEnemyView);
                    eternitySleepEnemyView.Init();
                }
            }

            return eternitySleepEnemyViewEntities.ToArray();
        }
        
    }
}