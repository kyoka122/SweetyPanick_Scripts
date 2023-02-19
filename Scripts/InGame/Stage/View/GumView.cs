using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyApplication;
using UnityEngine;
using Utility.TransitionFade;

namespace InGame.Stage.View
{
    public class GumView : MonoBehaviour, ISweets
    {
        public CancellationToken cancellationToken { get; private set; }
        public SweetsType type { get; } = SweetsType.Sweets;
        public FixState fixState { get; private set; }
        
        private const PlayableCharacter EditableCharacterType = PlayableCharacter.Fu;
        
        [SerializeField] private SpriteRenderer fadeInSpriteRenderersAtFix;
        [SerializeField] private SpriteRenderer fadeOutSpriteRenderersAtFix;
        [SerializeField] private Transform particleInstanceTransform;
        
        private bool Fading => _fadeInTransitionAtFix.IsActiveFadeIn() || _fadeInTransitionAtFix.IsActiveFadeOut() ||
                               _fadeOutTransitionAtFix.IsActiveFadeIn() || _fadeOutTransitionAtFix.IsActiveFadeOut();
        
        private Transition _fadeInTransitionAtFix;
        private Transition _fadeOutTransitionAtFix;

        public void Init()
        {
            cancellationToken = this.GetCancellationTokenOnDestroy();
            _fadeInTransitionAtFix = new Transition(fadeInSpriteRenderersAtFix.material, this,1);
            _fadeOutTransitionAtFix = new Transition(fadeOutSpriteRenderersAtFix.material, this,0);
            fixState = FixState.Broken;
        }

        public async UniTask FixSweets(float duration,CancellationToken token)
        {
            fixState = FixState.Fixing;
            try
            {
                _fadeOutTransitionAtFix.FadeOut(duration);
                _fadeInTransitionAtFix.FadeIn(duration);
                await CheckFinishFixed(token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"SweetsFixCanceled");
                _fadeOutTransitionAtFix.fadeTween.Kill();
                _fadeInTransitionAtFix.fadeTween.Kill();
                _fadeOutTransitionAtFix.TransitionFadeInCondition();
                _fadeInTransitionAtFix.TransitionFadeOutCondition();
                fixState = FixState.Broken;
                return;
            }
            Debug.Log($"FixedSweets!");
        }

        public async UniTask BreakSweets(float duration,CancellationToken token)
        {
            fixState = FixState.Breaking;
            try
            {
                _fadeOutTransitionAtFix.FadeIn(duration);
                _fadeInTransitionAtFix.FadeOut(duration);
                await CheckFinishBroken(token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"SweetsBreakCanceled. token.state{token.IsCancellationRequested}");
                _fadeInTransitionAtFix.fadeTween.Kill();
                _fadeOutTransitionAtFix.fadeTween.Kill();
                _fadeInTransitionAtFix.TransitionFadeInCondition();
                _fadeOutTransitionAtFix.TransitionFadeOutCondition();
                fixState = FixState.Fixed;
                return;
            }
            Debug.Log($"BrokenSweets!");
        }

        public bool CanFixSweets(PlayableCharacter editCharacterType)
        {
            //MEMO: キャラクターごとに直せるスイーツを変えるならコメントアウト解除
            /*if (EditableCharacterType!=editCharacterType)
            {
                return false;
            }*/
            if (_fadeInTransitionAtFix == null || _fadeOutTransitionAtFix == null)
            {
                Debug.LogWarning($"Had Not Instance _transition", gameObject);
                return false;
            }
            return fixState == FixState.Broken;
        }

        public bool CanBreakSweets()
        {
            if (_fadeInTransitionAtFix == null || _fadeOutTransitionAtFix == null)
            {
                Debug.LogWarning($"Had Not Instance _transition", gameObject);
                return false;
            }
            return fixState == FixState.Fixed;
        }

        public Vector3 GetPlayParticlePos()
        {
            return particleInstanceTransform.position;
        }

        private async UniTask CheckFinishFixed(CancellationToken token)
        {
            var fadeInTask = UniTask.WaitWhile(() => _fadeInTransitionAtFix.IsActiveFadeIn(),
                cancellationToken: token);
            var fadeOutTask = UniTask.WaitWhile(() => _fadeOutTransitionAtFix.IsActiveFadeOut(),
                cancellationToken: token);
            await UniTask.WhenAll(fadeInTask,fadeOutTask);
            fixState = FixState.Fixed;
        }
        
        private async UniTask CheckFinishBroken(CancellationToken token)
        {
            Debug.Log($"UniTask.WhenAll");
            var fadeInTask= UniTask.WaitWhile(() => _fadeInTransitionAtFix.IsActiveFadeOut(),
                cancellationToken: token);
            var fadeOutTask= UniTask.WaitWhile(() => _fadeOutTransitionAtFix.IsActiveFadeIn(),
                cancellationToken: token);
            Debug.Log($"WaitStart!");
            await UniTask.WhenAll(fadeInTask,fadeOutTask);
            Debug.Log($"TaskFinished!");
            fixState = FixState.Broken;
        }
    }
}