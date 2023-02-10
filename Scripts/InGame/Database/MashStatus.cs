using InGame.Player.Installer;
using InGame.Player.View;
using MyApplication;
using UnityEngine;

namespace InGame.Database
{
    public class MashStatus:BaseCharacterCommonStatus
    {
        public float mashNekoAliveTime;
        public Vector2 toMashNekoInstanceVec;
        
        public MashStatus(CharacterBaseParameter characterBaseParameter,MashParameter mashParameter) 
            : base(characterBaseParameter)
        {
            mashNekoAliveTime = mashParameter.MashNekoAliveTime;
            toMashNekoInstanceVec = mashParameter.ToMashNekoInstanceVec;
        }

        public MashStatus Clone()
        {
            return (MashStatus) MemberwiseClone();
        }
    }
}