using System.Threading;
using Cysharp.Threading.Tasks;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Stage.View
{
    public interface ISweets
    {
        public CancellationToken cancellationToken { get; }
        public SweetsType type { get; }
        public SweetsScoreType scoreType { get; }
        public PlayableCharacter Specialist { get; }
        public FixState fixState { get; }
        public ReactiveProperty<bool> onFix{ get; }
        
        public UniTask FixSweets(float duration,CancellationToken token);
        public UniTask BreakSweets(float duration,CancellationToken token);
        public bool CanFixSweets(PlayableCharacter editCharacterType);
        public bool CanBreakSweets();
        public Vector3 GetPlayParticlePos();
        public Vector3 GetScorePos();
    }
}