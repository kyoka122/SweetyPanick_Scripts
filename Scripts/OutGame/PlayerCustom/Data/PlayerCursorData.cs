using OutGame.PlayerCustom.Entity;
using OutGame.PlayerCustom.View;

namespace OutGame.PlayerCustom.Data
{
    public class PlayerCursorData
    {
        public readonly int playerNum; 
        public readonly PlayerInputEntity playerInputEntity; 
        public readonly CharacterSelectCursorView characterSelectCursorView;

        public PlayerCursorData(int playerNum,PlayerInputEntity playerInputEntity, CharacterSelectCursorView characterSelectCursorView)
        {
            this.playerNum = playerNum;
            this.playerInputEntity = playerInputEntity;
            this.characterSelectCursorView = characterSelectCursorView;
        }
    }
}