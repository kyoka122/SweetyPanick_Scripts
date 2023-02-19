using System;
using OutGame.PlayerCustom.Logic;

namespace OutGame.PlayerCustom.Manager
{
    public class PlayerCustomManager:IDisposable
    {
        private PanelLogic _panelLogic;
        private PlayerCountButtonLogic _playerCountButtonLogic;
        private PlayerControllerLogic _playerControllerLogic;
        private CharacterSelectLogic _characterSelectLogic;
        private ConfirmLogic _confirmLogic;
        private IDisposable[] _disposables;

        public PlayerCustomManager(PanelLogic panelLogic, PlayerCountButtonLogic playerCountButtonLogic,
            PlayerControllerLogic playerControllerLogic, CharacterSelectLogic characterSelectLogic,
            ConfirmLogic confirmLogic,IDisposable[] disposables)
        {
            _panelLogic = panelLogic;
            _playerCountButtonLogic = playerCountButtonLogic;
            _playerControllerLogic = playerControllerLogic;
            _characterSelectLogic = characterSelectLogic;
            _confirmLogic = confirmLogic;
            _disposables = disposables;
        }

        public void Start()
        {
            _playerCountButtonLogic.SetPlayerCountState();
        }


        public void Dispose()
        {
            _characterSelectLogic?.Dispose();
        }
    }
}