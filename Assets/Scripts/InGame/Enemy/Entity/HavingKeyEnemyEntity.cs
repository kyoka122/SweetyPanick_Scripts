using InGame.Common.Database;
using InGame.Database;
using MyApplication;

namespace InGame.Enemy.Entity
{
    public class HavingKeyEnemyEntity:BaseEnemyEntity
    {
        public int currentBoundDelayCount { get; private set; }
        public float ToGroundDistance => inGameDatabase.GetEnemyData().ToGroundDistance;
        public int TrampolineBoundDelayCount => inGameDatabase.GetEnemyData().TrampolineBoundDelayCount;
        public float BoundValue => inGameDatabase.GetEnemyData().TrampolineBoundValue;
        public override float moveSpeed => inGameDatabase.GetEnemyData().HavingKeyEnemyMoveSpeed;
        public SquareRange CanMoveRange => inGameDatabase.GetStageSettings().ObjectInScreenRangeOfHavingKeyEnemy;
        public bool hadInScreenX{ get; private set; } = false;
        
        public HavingKeyEnemyEntity(InGameDatabase inGameDatabase, CommonDatabase commonDatabase) : base(inGameDatabase, commonDatabase)
        {
        }

        public void AddCurrentBoundDelayCount()
        {
            currentBoundDelayCount++;
        }
        
        public void ClearCurrentDelayCount()
        {
            currentBoundDelayCount = 0;
        }

        public void SetHadInScreenX(bool on)
        {
            hadInScreenX = on;
        }
    }
}