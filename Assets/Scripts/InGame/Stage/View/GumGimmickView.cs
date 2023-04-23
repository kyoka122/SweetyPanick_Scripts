using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using MyApplication;
using UniRx;
using UnityEngine;
using Utility;
using Utility.TransitionFade;

namespace InGame.Stage.View
{
    public class GumGimmickView:MonoBehaviour,ISweets
    {
        public CancellationToken cancellationToken { get; private set; }
        public SweetsType type => SweetsType.GimmickSweets;
        public SweetsScoreType scoreType { get; } =SweetsScoreType.Normal;
        public PlayableCharacter Specialist => PlayableCharacter.Fu;
        public FixState fixState { get; private set; }
        public ReactiveProperty<bool> onFix { get; private set; }
        
        private bool Fading => _fadeOutTransitionAtFix.IsActiveFadeIn() || _fadeOutTransitionAtFix.IsActiveFadeOut() ||
                               _fadeInTransitionAtFix.IsActiveFadeIn() || _fadeInTransitionAtFix.IsActiveFadeOut();
        
        [SerializeField] private SpriteRenderer fadeInSpriteRenderersAtFix;
        [SerializeField] private SpriteRenderer fadeOutSpriteRenderersAtFix;
        [SerializeField] private Transform particleInstanceTransform;
        [SerializeField] private Transform scoreInstanceTransform;
        [SerializeField] private GameObject fixGaugeObj;
        [SerializeField] private SpriteRenderer fixGaugeSlider;

        private Transition _fadeInTransitionAtFix;
        private Transition _fadeOutTransitionAtFix;
        private CutOffTransition _cutOffTransition;

        public void Init()
        {
            cancellationToken=cancellationToken = this.GetCancellationTokenOnDestroy();
            _fadeInTransitionAtFix = new Transition(fadeOutSpriteRenderersAtFix.material, this,1);
            _fadeOutTransitionAtFix = new Transition(fadeInSpriteRenderersAtFix.material, this,0);
            _cutOffTransition = new CutOffTransition(fixGaugeSlider.material,1,0);
            fixState = FixState.Broken;
            onFix = new ReactiveProperty<bool>();
        }

        public async UniTask FixSweets(float duration, CancellationToken token)
        {
            fixState = FixState.Fixing;
            fixGaugeObj.gameObject.SetActive(true);
            _cutOffTransition.SetCutOffY(0);
            float countUp = 0;
            TweenerCore<float, float, FloatOptions> tweenCore = DOTween.To(() => countUp, 
                n => _cutOffTransition.SetCutOffY(n), 1, duration);
            
            try
            {
                _fadeOutTransitionAtFix.FadeOut(duration);
                _fadeInTransitionAtFix.FadeIn(duration);
                await WaitFinishFixed(token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"SweetsFixCanceled");
                tweenCore.Kill();
                _fadeOutTransitionAtFix.fadeTween.Kill();
                _fadeInTransitionAtFix.fadeTween.Kill();
                _fadeOutTransitionAtFix.TransitionFadeInCondition();
                _fadeInTransitionAtFix.TransitionFadeOutCondition();
                fixGaugeObj.gameObject.SetActive(false);
                fixState = FixState.Broken;
                return;
            }
            
            fixGaugeObj.gameObject.SetActive(false);
            _cutOffTransition.SetCutOffY(0);
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
                Debug.Log($"SweetsBreakCanceled");
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
                Debug.LogWarning($"Had Not Instance _transition", gameObject);
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
                Debug.LogWarning($"Had Not Instance _transition", gameObject);
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
            var fadeOutTask= UniTask.WaitWhile(() => _fadeOutTransitionAtFix.IsActiveFadeOut(),
                cancellationToken: token);
            var fadeInTask= UniTask.WaitWhile(() => _fadeInTransitionAtFix.IsActiveFadeIn(),
                cancellationToken: token);
            await UniTask.WhenAll(fadeOutTask,fadeInTask);
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
            if (_cutOffTransition.GetDisposeMaterial()!=null)
            {
                Destroy(_cutOffTransition.GetDisposeMaterial());
            }
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