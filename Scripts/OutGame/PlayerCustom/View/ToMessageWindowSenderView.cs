using Fungus;
using MyApplication;
using UnityEngine;

namespace OutGame.PlayerCustom.View
{
    public class ToMessageWindowSenderView:MonoBehaviour
    {
        [SerializeField] private Flowchart flowchart;
        
        public void SendPlayerCountSettingsEvent()
        {
            flowchart.SendFungusMessage(FungusCallMethodName.PlayerNumSettings);
        }
        
        public void SendControllerSettingsEvent()
        {
            flowchart.SendFungusMessage(FungusCallMethodName.ControllerSettings);
        }
        
        public void SendCharacterSettingsEvent()
        {
            flowchart.SendFungusMessage(FungusCallMethodName.CharacterSettings);
        }
        
        /*public void SendConfirmSettingsEvent()
        {
            flowchart.SendFungusMessage(FungusCallMethodName.ConfirmSettings);
        }*/
        
        public void SendCheerMessageEvent()
        {
            flowchart.SendFungusMessage(FungusCallMethodName.CheerMessage);
        }
    }
}