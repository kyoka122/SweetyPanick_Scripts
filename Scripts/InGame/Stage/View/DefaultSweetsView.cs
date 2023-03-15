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
    public class DefaultSweetsView:MonoBehaviour,ISweets
    {
        public CancellationToken cancellationToken { get; private set; }
        public SweetsType type => SweetsType.Sweets;
        public FixState fixState { get; private set; }
        public ReactiveProperty<bool> onFix { get; private set; }

        [SerializeField]private SpriteRenderer fadeSpriteRenderers;
        [SerializeField] private PlayableCharacter editableCharacterType;
        [SerializeField] private Transform particleInstanceTransform;
        
        private bool Fading => _transition.IsActiveFadeIn() || _transition.IsActiveFadeOut();
        private Transition _transition;

        public void Init()
        {
            _transition = new Transition(fadeSpriteRenderers.material,this,1);
            cancellationToken = this.GetCancellationTokenOnDestroy();
            fixState = FixState.Broken;
            onFix = new ReactiveProperty<bool>();
        }
        
        public async UniTask FixSweets(float duration,CancellationToken token)
        {
            fixState = FixState.Fixing;
            try
            {
                _transition.FadeIn(duration);
                await UniTask.WaitWhile(() => _transition.IsActiveFadeIn(),
                    cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"SweetsFixCanceled");
                _transition.fadeTween.Kill();
                _transition.TransitionFadeOutCondition();
                fixState = FixState.Broken;
                return;
            }

            fixState = FixState.Fixed;
            onFix.Value = true;
            Debug.Log($"FixedSweets!");
        }
        
        public async UniTask BreakSweets(float duration, CancellationToken token)
        {
            fixState = FixState.Breaking;
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
            onFix.Value = false;
            Debug.Log($"BrokenSweets!");
        }
        
        
        public bool CanFixSweets(PlayableCharacter editCharacterType)
        {
            if (_transition==null)
            {
                Debug.LogWarning($"Had Not Instance _transition", gameObject);
                return false;
            }
            //MEMO: キャラクターごとに直せるスイーツを変えるならコメントアウト解除
            /*if (editableCharacterType!=editCharacterType)
            {
                return false;
            }*/
            return fixState == FixState.Broken;
        }

        public bool CanBreakSweets()
        {
            if (_transition==null)
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

        private void OnDestroy()
        {
            if (_transition.GetDisposeMaterial()!=null)
            {
                Destroy(_transition.GetDisposeMaterial());
            }
        }
    }
}