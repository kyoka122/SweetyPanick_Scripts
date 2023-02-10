using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyApplication;
using UnityEngine;
using Utility.TransitionFade;

namespace InGame.Stage.View
{
    public class GumGimmickView:MonoBehaviour,ISweets
    {
        public CancellationToken cancellationToken { get; private set; }
        public SweetsType type { get; } = SweetsType.GimmickSweets;
        public FixState fixState { get; private set; }

        private const PlayableCharacter EditableCharacterType = PlayableCharacter.Fu;

        private bool Fading => _fadeOutTransitionAtFix.IsActiveFadeIn() || _fadeOutTransitionAtFix.IsActiveFadeOut() ||
                               _fadeInTransitionAtFix.IsActiveFadeIn() || _fadeInTransitionAtFix.IsActiveFadeOut();
        
        [SerializeField] private SpriteRenderer fadeInSpriteRenderersAtFix;
        [SerializeField] private SpriteRenderer fadeOutSpriteRenderersAtFix;
        [SerializeField] private Transform particleInstanceTransform;
        
        private Transition _fadeInTransitionAtFix;
        private Transition _fadeOutTransitionAtFix;

        public void Init()
        {
            cancellationToken=cancellationToken = this.GetCancellationTokenOnDestroy();
            _fadeInTransitionAtFix = new Transition(fadeOutSpriteRenderersAtFix.material, this,1);
            _fadeOutTransitionAtFix = new Transition(fadeInSpriteRenderersAtFix.material, this,0);
            fixState = FixState.Broken;
        }

        public async UniTask FixSweets(float duration, CancellationToken token)
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
                Debug.Log($"SweetsBreakCanceled");
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
            if (fixState != FixState.Broken&&fixState != FixState.Fixing)
            {
                Debug.Log($"Can`t Edit. CurrentSweetsCondition is:{fixState}");
                return false;
            }
            return true;
        }

        public bool CanBreakSweets()
        {
            if (_fadeInTransitionAtFix == null || _fadeOutTransitionAtFix == null)
            {
                Debug.LogWarning($"Had Not Instance _transition", gameObject);
                return false;
            }
            if (fixState != FixState.Fixed&&fixState != FixState.Breaking)
            {
                return false;
            }
            return true;
        }

        public Vector3 GetPlayParticlePos()
        {
            return particleInstanceTransform.position;
        }

        private async UniTask CheckFinishFixed(CancellationToken token)
        {
            var fadeOutTask= UniTask.WaitWhile(() => _fadeOutTransitionAtFix.IsActiveFadeOut(),
                cancellationToken: token);
            var fadeInTask= UniTask.WaitWhile(() => _fadeInTransitionAtFix.IsActiveFadeIn(),
                cancellationToken: token);
            await UniTask.WhenAll(fadeOutTask,fadeInTask);
            fixState = FixState.Fixed;
        }
        
        private async UniTask CheckFinishBroken(CancellationToken token)
        {
            var fadeInTask= UniTask.WaitWhile(() => _fadeInTransitionAtFix.IsActiveFadeOut(),
                cancellationToken: token);
            var fadeOutTask= UniTask.WaitWhile(() => _fadeOutTransitionAtFix.IsActiveFadeIn(),
                cancellationToken: token);
            await UniTask.WhenAll(fadeInTask,fadeOutTask);
            fixState = FixState.Broken;
        }
        
    }
}