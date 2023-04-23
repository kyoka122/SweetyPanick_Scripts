using InGame.Database.ScriptableData;
using MyApplication;

namespace InGame.Database
{
    public class EachStagePlayerInstanceData
    {
        public StageArea StageArea { get; }
        public PlayerInstanceData PlayerInstanceData { get; }

        public EachStagePlayerInstanceData(StageArea stageArea, PlayerInstanceData playerInstanceData)
        {
            StageArea = stageArea;
            PlayerInstanceData = playerInstanceData;
        }
    }
}