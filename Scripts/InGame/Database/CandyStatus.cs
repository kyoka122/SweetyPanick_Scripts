using InGame.Player.Installer;
using InGame.Player.View;
using MyApplication;

namespace InGame.Database
{
    public class CandyStatus:BaseCharacterCommonStatus
    {
        public CandyStatus(CharacterBaseParameter characterBaseParameter, CandyParameter candyParameter,
            int playerNum) : base(characterBaseParameter)
        {
        }

        public CandyStatus Clone()
        {
            return (CandyStatus) MemberwiseClone();
        }
    }
}