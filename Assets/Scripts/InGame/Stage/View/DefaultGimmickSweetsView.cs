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
    public abstract class DefaultGimmickSweetsView:MonoBehaviour,ISweets
    {
        public CancellationToken cancellationToken { get; private set; }
        public SweetsType type { get; } = SweetsType.GimmickSweets;
        public SweetsScoreType scoreType { get; } =SweetsScoreType.Normal;
        public PlayableCharacter Specialist => specialist;
        public FixState fixState { get; protected set; }
        public ReactiveProperty<bool> onFix { get; private set; }
        public bool isActive => gameObject.activeSelf;
        public FixState IsInitState => isInitFixed ? FixState.Fixed : FixState.Broken;
        
        [SerializeField] private PlayableCharacter specialist;
        [SerializeField] private SpriteRenderer fadeSpriteRenderers;
        [SerializeField] private Transform particleInstanceTransform;
        [SerializeField] private Transform scoreInstanceTransform;
        [SerializeField] private GameObject fixGaugeObj;
        [SerializeField] private SpriteRenderer fixGaugeSlider;
        [SerializeField] private bool isInitFixed = false;
        
        private bool Fading =>  transition.IsActiveFadeIn() ||  transition.IsActiveFadeOut();
        
        /// <summary>
        /// お菓子用Transition
        /// </summary>
        protected Transition transition;
        
        /// <summary>
        /// 修復ゲージ用Transition
        /// </summary>
        protected CutOffTransition cutOffTransition;
        

        public virtual void Init()
        {
            cancellationToken = this.GetCancellationTokenOnDestroy();
            transition = new Transition(fadeSpriteRenderers.material, this,1);
            cutOffTransition = new CutOffTransition(fixGaugeSlider.material,1,0);
            fixState = isInitFixed ? FixState.Fixed : FixState.Broken;
            onFix = new ReactiveProperty<bool>(isInitFixed);
        }
        
        public async UniTask FixSweets(float duration,CancellationToken token)
        {
            fixState = FixState.Fixing;
            fixGaugeObj.gameObject.SetActive(true);
            cutOffTransition.SetCutOffY(0);
            float countUp = 0;
            TweenerCore<float, float, FloatOptions> tweenCore = DOTween.To(() => countUp, 
                n => cutOffTransition.SetCutOffY(n), 1, duration);
            
            try
            {
                transition.FadeIn(duration);
                await UniTask.WaitWhile(() => transition.IsActiveFadeIn(),
                    cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"SweetsFixCanceled");
                tweenCore.Kill();
                transition.fadeTween.Kill();
                fixGaugeObj.SetActive(false);
                transition.TransitionFadeOutCondition();
                fixState = FixState.Broken;
                return;
            }

            EachSweetsEvent();
            fixGaugeObj.SetActive(false);
            cutOffTransition.SetCutOffY(0);
            fixState = FixState.Fixed;
            onFix.Value = true;
            Debug.Log($"FixedGimmickSweets!");
        }

        protected abstract void EachSweetsEvent();
        
        public async UniTask BreakSweets(float duration, CancellationToken token)
        {
            //MEMO: ギミックスイーツを壊せるようにするならコメントアウト解除（スイーツ固有効果の削除も忘れずに）
            /*fixState = FixState.Breaking;
            try
            {
                _transition.FadeOut(duration);
                await UniTask.WaitWhile(() => _transition.IsActiveFadeOut(),
                    cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"SweetsBreakCanceled");
                _transition.fadeTween.Kill();
                _transition.TransitionFadeInCondition();
                fixState = FixState.Fixed;
                return;
            }
            fixState = FixState.Broken;
            Debug.Log($"BrokenGimmickSweets!");*/
        }

        
        public virtual bool CanFixSweets(PlayableCharacter editCharacterType)
        {
            if (transition==null)
            {
                Debug.LogWarning($"Had Not Instance _transition", gameObject);
                return false;
            }
            //MEMO: キャラクターごとに直せるスイーツを変えるならコメントアウト解除
            /*if (editableCharacterType!=editCharacterType)
            {
                return false;
            }*/
            if (fixState != FixState.Broken)
            {
                //Debug.Log($"Can`t Edit. CurrentSweetsCondition is:{fixState}");
                return false;
            }
            return true;
        }

        public virtual bool CanBreakSweets()
        {
            if (transition==null)
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
        
        protected virtual void OnDestroy()
        {
            onFix?.Dispose();
            if (cutOffTransition.GetDisposeMaterial()!=null)
            {
                Destroy(cutOffTransition.GetDisposeMaterial());
            }
            if (transition.GetDisposeMaterial() != null)
            {
                Destroy(transition.GetDisposeMaterial());
            }
        }
    }
}