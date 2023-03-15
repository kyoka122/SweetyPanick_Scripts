using TalkSystem;
using UnityEngine;

namespace OutGame.PlayerCustom.View
{
    public class ToMessageWindowSenderView:MonoBehaviour
    {
        private Dialogs _playerCountDialog;
        private Dialogs _controllerDialog;
        private Dialogs _characterDialog;
        private Dialogs _cheerDialog;
        private BaseTalkKeyObserver _talkKeyObserver;

        private Dialogs _prevPlayedDialog;

        public void Init(Dialogs playerCountDialog,Dialogs controllerDialog,Dialogs characterDialog,Dialogs cheerDialog)
        {
            _playerCountDialog = playerCountDialog;
            _controllerDialog = controllerDialog;
            _characterDialog = characterDialog;
            _cheerDialog = cheerDialog;
        }
        
        public void SendPlayerCountSettingsEvent()
        {
            ExitPrevDialog();
            _playerCountDialog.PlayFirstDialog();
            _prevPlayedDialog = _playerCountDialog;
        }
        
        public void SendControllerSettingsEvent()
        {
            ExitPrevDialog();
            _controllerDialog.PlayFirstDialog();
            _prevPlayedDialog = _controllerDialog;
        }
        
        public void SendCharacterSettingsEvent()
        {
            ExitPrevDialog();
            _characterDialog.PlayFirstDialog();
            _prevPlayedDialog = _characterDialog;
        }

        private void ExitPrevDialog()
        {
            if (_prevPlayedDialog!=null)
            {
                _prevPlayedDialog.ExitDialog();
            }
        }
        
        public void SetTalkKeyObserver(BaseTalkKeyObserver talkKeyObserver)
        {
            _talkKeyObserver = talkKeyObserver;
        }
        
        public void SendCheerMessageEvent()
        {
            ExitPrevDialog();
            _cheerDialog.SetTalkKeyObserver(_talkKeyObserver);
            _cheerDialog.StartDialogs();
        }
    }
}