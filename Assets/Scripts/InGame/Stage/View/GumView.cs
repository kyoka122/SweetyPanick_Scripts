using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyApplication;
using UniRx;
using UnityEngine;
using Utility.TransitionFade;

namespace InGame.Stage.View
{
    public class GumView : MonoBehaviour, ISweets
    {
        public CancellationToken cancellationToken { get; private set; }
        public SweetsType type => SweetsType.Sweets;
        public SweetsScoreType scoreType => sweetsScoreType;
        public PlayableCharacter Specialist => PlayableCharacter.Fu;
        public FixState fixState { get; private set; }
        public ReactiveProperty<bool> onFix { get; private set; }
        public bool isActive => gameObject.activeSelf;
        public FixState IsInitState => isInitFixed ? FixState.Fixed : FixState.Broken;
        
        [SerializeField] private SweetsScoreType sweetsScoreType = SweetsScoreType.Normal;
        [SerializeField] private bool isInitFixed;
        [SerializeField] private SpriteRenderer fadeInSpriteRenderersAtFix;
        [SerializeField] private SpriteRenderer fadeOutSpriteRenderersAtFix;
        [SerializeField] private Transform particleInstanceTransform;
        [SerializeField] private Transform scoreInstanceTransform;

        private bool Fading => _fadeInTransitionAtFix.IsActiveFadeIn() || _fadeInTransitionAtFix.IsActiveFadeOut() ||
                               _fadeOutTransitionAtFix.IsActiveFadeIn() || _fadeOutTransitionAtFix.IsActiveFadeOut();
        
        private Transition _fadeInTransitionAtFix;
        private Transition _fadeOutTransitionAtFix;

        public void Init()
        {
            cancellationToken = this.GetCancellationTokenOnDestroy();
            fixState = isInitFixed ? FixState.Fixed : FixState.Broken;
            _fadeInTransitionAtFix = new Transition(fadeInSpriteRenderersAtFix.material, this,Convert.ToInt32(!isInitFixed));
            _fadeOutTransitionAtFix = new Transition(fadeOutSpriteRenderersAtFix.material, this,Convert.ToInt32(isInitFixed));
            onFix = new ReactiveProperty<bool>(isInitFixed);
        }

        public async UniTask FixSweets(float duration,CancellationToken token)
        {
            fixState = FixState.Fixing;
            try
            {
                _fadeOutTransitionAtFix.FadeOut(duration);
                _fadeInTransitionAtFix.FadeIn(duration);
                await WaitFinishFixed(token);
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
            fixState = FixState.Fixed;
            onFix.Value = true;
            Debug.Log($"FixedSweets!");
        }

        public async UniTask BreakSweets(float duration,CancellationToken token)
        {
            fixState = FixState.Breaking;
            try
            {
                _fadeOutTransitionAtFix.FadeIn(duration);
                _fadeInTransitionAtFix.FadeOut(duration);
                await WaitFinishBroken(token);
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
            fixState = FixState.Broken;
            onFix.Value = false;
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
                //Debug.LogWarning($"Had Not Instance _transition", gameObject);
                return false;
            }
            if (fixState != FixState.Broken)
            {
                //Debug.Log($"Can`t Edit. CurrentSweetsCondition is:{fixState}");
                return false;
            }
            return true;
        }

        public bool CanBreakSweets()
        {
            if (_fadeInTransitionAtFix == null || _fadeOutTransitionAtFix == null)
            {
                //Debug.LogWarning($"Had Not Instance _transition", gameObject);
                return false;
            }
            if (fixState !=FixState.Fixed)
            {
                //Debug.Log($"Can`t Edit. CurrentSweetsCondition is:{fixState}");
                return false;
            }
            return true;
        }

        public Vector3 GetPlayParticlePos()
        {
            return particleInstanceTransform.position;
        }
        
        public Vector3 GetScorePos()
        {
            return scoreInstanceTransform.position;
        }

        private async UniTask WaitFinishFixed(CancellationToken token)
        {
            var fadeInTask = UniTask.WaitWhile(() => _fadeInTransitionAtFix.IsActiveFadeIn(),
                cancellationToken: token);
            var fadeOutTask = UniTask.WaitWhile(() => _fadeOutTransitionAtFix.IsActiveFadeOut(),
                cancellationToken: token);
            await UniTask.WhenAll(fadeInTask,fadeOutTask);
        }
        
        private async UniTask WaitFinishBroken(CancellationToken token)
        {
            var fadeInTask= UniTask.WaitWhile(() => _fadeInTransitionAtFix.IsActiveFadeOut(),
                cancellationToken: token);
            var fadeOutTask= UniTask.WaitWhile(() => _fadeOutTransitionAtFix.IsActiveFadeIn(),
                cancellationToken: token);
            await UniTask.WhenAll(fadeInTask,fadeOutTask);
        }

        public void OnDestroy()
        {
            if (_fadeInTransitionAtFix.GetDisposeMaterial()!=null)
            {
                Destroy(_fadeInTransitionAtFix.GetDisposeMaterial());
            }
            if (_fadeOutTransitionAtFix.GetDisposeMaterial()!=null)
            {
                Destroy(_fadeOutTransitionAtFix.GetDisposeMaterial());
            }
        }
    }
}