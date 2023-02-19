namespace InGame.Database
{
    public class PlayerUpdateableData
    {
        public int playerNum ;
        public int currentHp ;
        public bool isDead ;
        //public Input

        public PlayerUpdateableData(int playerNum, int maxHp,bool isDead)
        {
            this.playerNum = playerNum;
            currentHp = maxHp;
            this.isDead = isDead;
        }

        public PlayerUpdateableData Clone()
        {
            return MemberwiseClone() as PlayerUpdateableData;
        }
    }
}