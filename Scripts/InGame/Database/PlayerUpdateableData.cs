namespace InGame.Database
{
    public class PlayerUpdateableData
    {
        public int playerNum ;
        public int currentHp ;
        //public Input

        public PlayerUpdateableData(int playerNum, int maxHp)
        {
            this.playerNum = playerNum;
            currentHp = maxHp;
        }

        public PlayerUpdateableData Clone()
        {
            return MemberwiseClone() as PlayerUpdateableData;
        }
    }
}