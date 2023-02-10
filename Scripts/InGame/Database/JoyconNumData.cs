namespace InGame.Database
{
    public class JoyconNumData
    {
        public readonly int playerNum;
        public readonly int leftJoyconNum;
        public readonly int rightJoyconNum;

        public JoyconNumData(int playerNum,int leftJoyconNum, int rightJoyconNum)
        {
            this.playerNum = playerNum;
            this.leftJoyconNum = leftJoyconNum;
            this.rightJoyconNum = rightJoyconNum;
        }

        public JoyconNumData Clone()
        {
            return MemberwiseClone() as JoyconNumData;
        }
    }
}