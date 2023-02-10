using System.Threading;
using Cysharp.Threading.Tasks;
using MyApplication;
using UnityEngine;

namespace InGame.Stage.View
{
    public interface ISweets
    {
        public CancellationToken cancellationToken { get; }
        public SweetsType type { get; }
        public FixState fixState { get; }
        
        public UniTask FixSweets(float duration,CancellationToken token);

        public UniTask BreakSweets(float duration,CancellationToken token);

        public bool CanFixSweets(PlayableCharacter editCharacterType);
        public bool CanBreakSweets();
        public Vector3 GetPlayParticlePos();
    }
}