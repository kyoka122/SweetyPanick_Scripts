using OutGame.PlayerCustom.Logic;

namespace OutGame.PlayerCustom.Manager
{
    public class PlayerCustomManager
    {
        private PanelLogic _panelLogic;
        private PlayerCountButtonLogic _playerCountButtonLogic;
        private PlayerControllerLogic _playerControllerLogic;
        private CharacterSelectLogic _characterSelectLogic;
        private ConfirmLogic _confirmLogic;

        public PlayerCustomManager(PanelLogic panelLogic, PlayerCountButtonLogic playerCountButtonLogic,
            PlayerControllerLogic playerControllerLogic, CharacterSelectLogic characterSelectLogic,
            ConfirmLogic confirmLogic)
        {
            _panelLogic = panelLogic;
            _playerCountButtonLogic = playerCountButtonLogic;
            _playerControllerLogic = playerControllerLogic;
            _characterSelectLogic = characterSelectLogic;
            _confirmLogic = confirmLogic;
        }

        public void Start()
        {
            _playerCountButtonLogic.SetPlayerCountState();
        }
        
    }
}