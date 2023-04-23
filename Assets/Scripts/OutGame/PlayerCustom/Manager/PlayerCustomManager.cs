using System;
using OutGame.PlayerCustom.Logic;

namespace OutGame.PlayerCustom.Manager
{
    public class PlayerCustomManager:IDisposable
    {
        private readonly PanelLogic _panelLogic;
        private readonly PlayerCountButtonLogic _playerCountButtonLogic;
        private readonly ControllerLogic _controllerLogic;
        private readonly CharacterSelectLogic _characterSelectLogic;
        private readonly FinishTalkLogic _finishTalkLogic;
        private readonly IDisposable[] _disposables;

        public PlayerCustomManager(PanelLogic panelLogic, PlayerCountButtonLogic playerCountButtonLogic,
            ControllerLogic controllerLogic, CharacterSelectLogic characterSelectLogic,
            FinishTalkLogic finishTalkLogic,IDisposable[] disposables)
        {
            _panelLogic = panelLogic;
            _playerCountButtonLogic = playerCountButtonLogic;
            _controllerLogic = controllerLogic;
            _characterSelectLogic = characterSelectLogic;
            _finishTalkLogic = finishTalkLogic;
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