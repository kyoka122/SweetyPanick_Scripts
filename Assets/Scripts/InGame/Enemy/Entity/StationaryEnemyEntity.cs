using InGame.Common.Database;
using InGame.Database;

namespace InGame.Enemy.Entity
{
    public class StationaryEnemyEntity:BaseEnemyEntity
    {
        public StationaryEnemyEntity(InGameDatabase inGameDatabase, CommonDatabase commonDatabase) : base(inGameDatabase, commonDatabase)
        {
        }
    }
}