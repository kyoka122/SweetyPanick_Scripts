using InGame.Player.View;
using MyApplication;

namespace InGame.Database
{
    public class KureConstData
    {
        public PlayableCharacter CharacterType { get; }
        public KureView Prefab { get; }

        public KureConstData(KureParameter kureParameter)
        {
            CharacterType = PlayableCharacter.Kure;
            Prefab = kureParameter.Prefab;
        }
    }
}