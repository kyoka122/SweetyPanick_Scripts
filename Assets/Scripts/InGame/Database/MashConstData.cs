using InGame.Player.View;
using MyApplication;

namespace InGame.Database
{
    public class MashConstData
    {
        public PlayableCharacter CharacterType { get; }
        public MashView Prefab { get; }
        public MashNekoView MashNekoPrefab { get; }

        public MashConstData(MashParameter mashParameter)
        {
            CharacterType = PlayableCharacter.Mash;
            Prefab = mashParameter.Prefab;
            MashNekoPrefab = mashParameter.MashNekoPrefab;
        }
    }
}