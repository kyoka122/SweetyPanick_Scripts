using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Database;
using InGame.Enemy.Entity;
using InGame.Enemy.Interface;
using InGame.Enemy.Logic;
using InGame.Enemy.View;
using UnityEngine;

namespace InGame.Enemy.Installer
{
    public class EnemyInstaller:MonoBehaviour
    {
        [SerializeField] protected ViewGenerator viewGenerator;
        
        private InGameDatabase _inGameDatabase;
        private CommonDatabase _commonDatabase;

        public void Init(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
        }
        
        public EnemyManager Install()
        {
            var enemyController = new EnemyManager();
            return enemyController;
        }
        
        public EnemyManager InstallWithStageEnemies()
        {
            BaseEnemyView[] enemyViews=FindObjectsOfType<BaseEnemyView>();
            DefaultEnemyView[] defaultEnemyViews = GetDefaultEnemyViews(enemyViews);
            EternitySleepEnemyView[] eternitySleepEnemyViews = GetEternitySleepEnemyViews(enemyViews);
            StationaryEnemyView[] stationaryEnemyViews = GetStationaryEnemyViews(enemyViews);

            var defaultEnemyLogics = new List<DefaultEnemyLogic>();
            var eternitySleepEnemyLogics = new List<EternitySleepEnemyLogic>();
            var stationaryEnemyLogics = new List<StationaryEnemyLogic>();
            
            foreach (var defaultEnemyView in defaultEnemyViews)
            {
                defaultEnemyLogics.Add(new DefaultEnemyLogic(new DefaultEnemyEntity(_inGameDatabase, _commonDatabase),
                    defaultEnemyView));
            }
            foreach (var eternitySleepEnemy in eternitySleepEnemyViews)
            {
                eternitySleepEnemyLogics.Add(new EternitySleepEnemyLogic(
                    new EternitySleepEnemyEntity(_inGameDatabase, _commonDatabase), eternitySleepEnemy));
            }
            foreach (var stationaryEnemyView in stationaryEnemyViews)
            {
                stationaryEnemyLogics.Add(new StationaryEnemyLogic(new StationaryEnemyEntity(
                    _inGameDatabase,_commonDatabase),stationaryEnemyView));
            }
            
            List<BaseEnemyLogic> enemyLogics = new List<BaseEnemyLogic>(defaultEnemyLogics);
            enemyLogics.AddRange(eternitySleepEnemyLogics);
            enemyLogics.AddRange(stationaryEnemyLogics);
            var enemyController = new EnemyManager(enemyLogics);

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
        
        public IColateOrderAble InstallDefaultEnemyByColate(EnemyManager enemyManager,Vector2 spawnPos)
        {
            DefaultEnemyView defaultEnemyView=SpawnRandomDefaultEnemy(spawnPos);
            defaultEnemyView.Init();
            enemyManager.AddEnemy(new NotMoveLimitEnemyLogic(new DefaultEnemyEntity(_inGameDatabase, _commonDatabase),
                defaultEnemyView));
            return defaultEnemyView;
        }
        
        private DefaultEnemyView SpawnRandomDefaultEnemy(Vector2 spawnPos)
        {
            int prefabLength=_inGameDatabase.GetEnemyData().DefaultEnemyPrefab.Length;
            int random = Random.Range(0, prefabLength);
            return viewGenerator.GenerateDefaultEnemyView(_inGameDatabase.GetEnemyData().DefaultEnemyPrefab[random],spawnPos);
        }

        private StationaryEnemyView[] GetStationaryEnemyViews(BaseEnemyView[] enemyViews)
        {
            List<StationaryEnemyView> stationaryEnemyViews = new List<StationaryEnemyView>();
            foreach (var enemyView in enemyViews)
            {
                if (enemyView.TryGetComponent(out StationaryEnemyView stationaryEnemyView))
                {
                    stationaryEnemyViews.Add(stationaryEnemyView);
                    stationaryEnemyView.Init();
                }
            }

            return stationaryEnemyViews.ToArray();
        }
        
    }
}