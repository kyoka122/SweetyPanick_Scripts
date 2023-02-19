namespace InGame.Database
{
    /// <summary>
    /// ステージ全体に関わるデータ
    /// </summary>
    public class InStageData
    {
        public bool havingKey;
        public int score;
        
        public InStageData Clone()
        {
            return (InStageData) MemberwiseClone();
        }
    }
}