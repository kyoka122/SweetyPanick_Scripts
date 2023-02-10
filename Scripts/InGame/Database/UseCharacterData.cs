using MyApplication;

namespace InGame.Database
{
    public class UseCharacterData
    {
        public readonly int playerNum;
        public readonly PlayableCharacter playableCharacter;

        public UseCharacterData(int playerNum, PlayableCharacter playableCharacter)
        {
            this.playerNum = playerNum;
            this.playableCharacter = playableCharacter;
        }

        public UseCharacterData Clone()
        {
            return MemberwiseClone() as UseCharacterData;
        }
    }
}