using InGame.Database;

namespace InGame.Stage.Entity
{
    public class StageGimmickEntity
    {
        public float MoveFloorSpeed => _inGameDatabase.GetStageGimmickData().MoveFloorSpeed;
        
        private readonly InGameDatabase _inGameDatabase;

        public StageGimmickEntity(InGameDatabase inGameDatabase)
        {
            _inGameDatabase = inGameDatabase;
        }
    }
}