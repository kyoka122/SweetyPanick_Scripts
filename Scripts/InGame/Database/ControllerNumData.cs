using InGame.MyInput;

namespace InGame.Database
{
    public class ControllerNumData
    {
        public readonly int playerNum;
        public readonly BasePlayerInput playerInput;

        public ControllerNumData(int playerNum,BasePlayerInput playerInput)
        {
            this.playerNum = playerNum;
            this.playerInput = playerInput;
        }


        public ControllerNumData Clone()
        {
            return MemberwiseClone() as ControllerNumData;
        }
    }
}