using InGame.Player.View;
using MyApplication;

namespace InGame.Database
{
    public class FuConstData
    {
        public PlayableCharacter CharacterType { get; }
        public FuView Prefab { get; }
        public BindGumView GumPrefab { get; }

        public FuConstData(FuParameter fuParameter)
        {
            CharacterType = PlayableCharacter.Fu;
            Prefab = fuParameter.Prefab;
            GumPrefab = fuParameter.BindGumView;
        }
    }
}