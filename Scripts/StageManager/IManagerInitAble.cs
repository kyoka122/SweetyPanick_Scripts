using InGame.Player.Controller;

namespace StageManager
{
    public interface IManagerInitAble
    {
        public void AddController(BasePlayerController controller);

        public void FixedUpdateEnemy();

        public void RegisterPlayerEvent(BasePlayerController controller);
    }
}