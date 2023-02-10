using MyApplication;
using UnityEngine;

namespace InGame.Player.View
{
    public class FuView:BasePlayerView
    {
        public override PlayableCharacter type { get; } = PlayableCharacter.Fu;   

        public BindGumView GenerateBindGum(BindGumView bindGumView,Vector2 pos)
        {
            return Instantiate(bindGumView, pos, bindGumView.transform.rotation);
        }
    }
}