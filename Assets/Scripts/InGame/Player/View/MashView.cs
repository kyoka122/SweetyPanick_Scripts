using MyApplication;
using UnityEngine;

namespace InGame.Player.View
{
    public class MashView:BasePlayerView
    {
        public override PlayableCharacter type { get; } = PlayableCharacter.Mash;

        public MashNekoView GenerateMashNeko(MashNekoView mashNekoPrefab,Vector2 instancePos)
        {
            return Instantiate(mashNekoPrefab,instancePos,mashNekoPrefab.transform.rotation);
        }
    }
}