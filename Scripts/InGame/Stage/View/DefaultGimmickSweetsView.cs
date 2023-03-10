using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyApplication;
using UnityEngine;
using Utility.TransitionFade;

namespace InGame.Stage.View
{
    public abstract class DefaultGimmickSweetsView:MonoBehaviour,ISweets
    {
        public CancellationToken cancellationToken { get; private set; }
        public SweetsType type { get; } = SweetsType.GimmickSweets;
        public FixState fixState { get; private set; }
        
        [SerializeField]private SpriteRenderer fadeSpriteRenderers;
        [SerializeField] private PlayableCharacter editableCharacterType;
        [SerializeField] private Transform particleInstanceTransform;
        
        
        private bool Fading =>  _transition.IsActiveFadeIn() ||  _transition.IsActiveFadeOut();
        private Transition _transition;
        
        public virtual void Init()
        {
            cancellationToken = this.GetCancellationTokenOnDestroy();
            _transition = new Transition(fadeSpriteRenderers.material, this,1);
            fixState = FixState.Broken;
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

            EachSweetsEvent();
            fixState = FixState.Fixed;
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
            if (fixState != FixState.Broken&&fixState != FixState.Fixing)
            {
                Debug.Log($"Can`t Edit. CurrentSweetsCondition is:{fixState}");
                return false;
            }
            return true;
        }

        public bool CanBreakSweets()
        {
            if (_transition==null)
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
    }
}