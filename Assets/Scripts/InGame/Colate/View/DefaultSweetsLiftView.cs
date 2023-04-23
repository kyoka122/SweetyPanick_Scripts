using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using InGame.Stage.View;
using MyApplication;
using UniRx;
using UnityEngine;
using Utility;
using Utility.TransitionFade;

namespace InGame.Colate.View
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class DefaultSweetsLiftView:MonoBehaviour,ISweets
    {
        public SweetsType type { get; } = SweetsType.GimmickSweets;
        public SweetsScoreType scoreType { get; } = SweetsScoreType.None;
        public PlayableCharacter Specialist => specialist;
        public Transform AppearParticleInstanceTransform=>appearParticleInstanceTransform;
        public CancellationToken cancellationToken { get; private set; }
        public FixState fixState { get; protected set; }
        public ReactiveProperty<bool> onFix { get; private set; }
        public LiftState liftState { get; private set; } = LiftState.Up;

        /// <summary>
        /// お菓子用Transition
        /// </summary>
        protected Transition transition;
        
        /// <summary>
        /// 修復ゲージ用Transition
        /// </summary>
        protected CutOffTransition cutOffTransition;
        protected virtual Material FadeMaterial => fadeSpriteRenderers.material;
        
        [SerializeField] private PlayableCharacter specialist;
        [SerializeField] private SpriteRenderer fadeSpriteRenderers;
        [SerializeField] private Transform particleInstanceTransform;
        [SerializeField] private GameObject fixGaugeObj;
        [SerializeField] private SpriteRenderer fixGaugeSlider;
        [SerializeField] private GameObject colliderObj;
        [SerializeField] private Transform appearParticleInstanceTransform;
        [SerializeField] private Transform scoreInstanceTransform;


        private Rigidbody2D _rigidbody2D;
        private int _moveDirection;
        public float currentStayDuration { get; private set; }
        
        public void Init()
        {
            cancellationToken = this.GetCancellationTokenOnDestroy();
            transition = new Transition(FadeMaterial, this,1);
            cutOffTransition = new CutOffTransition(fixGaugeSlider.material,1,0);
            fixState = FixState.Broken;
            onFix = new ReactiveProperty<bool>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            gameObject.SetActive(false);
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

        private void EachSweetsEvent()
        {
            colliderObj.SetActive(true);
        }
        
        public async UniTask BreakSweets(float duration, CancellationToken token)
        {
            return;
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
            if (fixState != FixState.Broken&&fixState != FixState.Fixing)
            {
                Debug.Log($"Can`t Edit. CurrentSweetsCondition is:{fixState}");
                return false;
            }
            return true;
        }

        public virtual bool CanBreakSweets()
        {
            return false;
        }

        public Vector3 GetPlayParticlePos()
        {
            return particleInstanceTransform.position;
        }
        
        public Vector3 GetScorePos()
        {
            return scoreInstanceTransform.position;
        }

        public void SetYVelocity(float velocityY)
        {
            _rigidbody2D.velocity = new Vector2(0, velocityY);
        }

        public void InitToBreakState()
        {
            transition.TransitionFadeOutCondition();
            colliderObj.SetActive(false);
            fixState = FixState.Broken;
            Debug.Log($"BrokenGimmickSweets!");
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
        public void SetLiftState(LiftState state)
        {
            liftState = state;
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }

        public void SetCurrentDuration(float duration)
        {
            currentStayDuration = duration;
        }

        public void SetXPosition(float xPos)
        {
            transform.position = new Vector2(xPos,transform.position.y);
        }

        public void SetYPosition(float yPos)
        {
            transform.position = new Vector2(transform.position.x,yPos);
        }
        
        public virtual void OnDestroy()
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