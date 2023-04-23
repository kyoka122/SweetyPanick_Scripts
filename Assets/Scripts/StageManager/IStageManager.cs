using InGame.Player.Controller;

namespace StageManager
{
    public interface IStageManager
    {
        public void AddController(BasePlayerController controller);
        public void RegisterGameOverObserver();

        public void FixedUpdateEnemy();

        public void RegisterPlayerEvent(BasePlayerController controller);
    }
}