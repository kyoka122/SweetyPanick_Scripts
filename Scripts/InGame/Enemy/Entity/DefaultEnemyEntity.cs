using InGame.Common.Database;
using InGame.Database;

namespace InGame.Enemy.Entity
{
    public class DefaultEnemyEntity:BaseEnemyEntity
    {
        public DefaultEnemyEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase) : base(inGameDatabase,commonDatabase)
        {
        }
    }
}