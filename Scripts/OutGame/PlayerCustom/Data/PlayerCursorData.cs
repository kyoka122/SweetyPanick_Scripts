using OutGame.PlayerCustom.Entity;
using OutGame.PlayerCustom.View;

namespace OutGame.PlayerCustom.Data
{
    public class PlayerCursorData
    {
        public readonly int playerNum; 
        public readonly CharacterSelectInputEntity characterSelectInputEntity; 
        public readonly CharacterSelectCursorView characterSelectCursorView;

        public PlayerCursorData(int playerNum,CharacterSelectInputEntity characterSelectInputEntity, CharacterSelectCursorView characterSelectCursorView)
        {
            this.playerNum = playerNum;
            this.characterSelectInputEntity = characterSelectInputEntity;
            this.characterSelectCursorView = characterSelectCursorView;
        }
    }
}