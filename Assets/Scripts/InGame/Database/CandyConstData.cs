using InGame.Player.View;
using MyApplication;

namespace InGame.Database
{
    public class CandyConstData
    {
        public PlayableCharacter CharacterType { get; }
        public CandyView Prefab { get; }

        public CandyConstData(CandyParameter candyParameter)
        {
            CharacterType = PlayableCharacter.Candy;
            Prefab = candyParameter.Prefab;
        }
    }
}