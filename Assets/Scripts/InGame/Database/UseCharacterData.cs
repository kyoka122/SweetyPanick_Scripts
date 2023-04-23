using MyApplication;

namespace InGame.Database
{
    public class UseCharacterData
    {
        public readonly int playerNum;
        public readonly PlayableCharacter type;

        public UseCharacterData(int playerNum, PlayableCharacter type)
        {
            this.playerNum = playerNum;
            this.type = type;
        }

        public UseCharacterData Clone()
        {
            return MemberwiseClone() as UseCharacterData;
        }
    }
}