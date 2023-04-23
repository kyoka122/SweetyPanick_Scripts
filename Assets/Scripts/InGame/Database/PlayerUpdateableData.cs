namespace InGame.Database
{
    public class PlayerUpdateableData
    {
        public int playerNum;
        public int currentHp;
        public bool isUsed;
        public bool isDead;

        public PlayerUpdateableData(int playerNum, int maxHp,bool isUsed,bool isDead)
        {
            this.playerNum = playerNum;
            currentHp = maxHp;
            this.isUsed = isUsed;
            this.isDead = isDead;
        }
    }
}