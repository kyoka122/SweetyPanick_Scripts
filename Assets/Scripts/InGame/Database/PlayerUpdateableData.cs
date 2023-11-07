namespace InGame.Database
{
    public class PlayerUpdateableData
    {
        public int playerNum;
        public float currentHp;
        public bool isUsed;
        public bool isInStage;
        public bool isDead;
        public bool isReviving;

        public PlayerUpdateableData(int playerNum, int maxHp,bool isUsed,bool isInStage,bool isDead,bool isReviving)
        {
            this.playerNum = playerNum;
            currentHp = maxHp;
            this.isInStage = isInStage;
            this.isUsed = isUsed;
            this.isDead = isDead;
            this.isReviving = isReviving;
        }
    }
}