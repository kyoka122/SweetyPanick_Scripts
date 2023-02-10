using System;
using System.Threading;
using MyApplication;
using UniRx;

namespace InGame.Player.View
{
    //MEMO: キャンディちゃん用スクリプト
    public class CandyView:BasePlayerView
    {
        public override PlayableCharacter type { get; } = PlayableCharacter.Candy;
    }
}