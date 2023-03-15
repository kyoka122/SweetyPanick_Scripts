namespace InGame.Database
{
    /// <summary>
    /// ステージ全体に関わるデータ
    /// </summary>
    public class AllStageData
    {
        public bool havingKey;
        public int score;

        public AllStageData Clone()
        {
            return (AllStageData) MemberwiseClone();
        }
    }
}