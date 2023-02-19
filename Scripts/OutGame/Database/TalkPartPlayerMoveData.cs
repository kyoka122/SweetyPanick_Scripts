using MyApplication;

namespace OutGame.Database
{
    public class TalkPartPlayerMoveData
    {
        public TalkPartActionType ActionType { get; }
        public float MoveTime { get; }
        public float MoveXVec{ get; }
            
        public TalkPartPlayerMoveData(TalkPartActionType actionType,float moveTime, float moveXVec)
        {
            ActionType = actionType;
            MoveTime = moveTime;
            MoveXVec = moveXVec;
        }
    }
}