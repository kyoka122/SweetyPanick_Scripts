using MyApplication;
using OutGame.Database;
using UniRx;

namespace InGame.Player.Entity
{
    public class PlayerTalkEntity
    {
        public IReadOnlyReactiveProperty<TalkPartPlayerMoveData> TalkPartPlayerMoveData => _outGameDatabase.TalkStageData;
        
        private readonly OutGameDatabase _outGameDatabase;
        
        public PlayerTalkEntity(OutGameDatabase outGameDatabase)
        {
            _outGameDatabase = outGameDatabase;
        }

        public void OnFinishedTalkPartAction(TalkPartActionType type)
        {
            _outGameDatabase.SetTalkActionFinished(type);
        }
    }
}