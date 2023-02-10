using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InGame.Enemy.View;
using MyApplication;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.Player.View
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BindGumView:MonoBehaviour
    {
        public IObservable<bool> InflatedGum=>_inflatedGumSubject;
        
        public IObservable<Collider2D> OnTriggerEnter => _triggerEnter2DSubject;
        public IObservable<Collider2D> OnTriggerStay => _triggerEnter2DSubject;
        public GumState state { get; private set; }
        public CancellationToken thisToken { get; private set; }

        [SerializeField] private SpriteRenderer gumSpriteRenderer;
        [SerializeField] private Collider2D gumCollider;
        
        
        private IEnemyBindable _bindableObject;
        private Rigidbody2D _rigidbody2D;
        
        private Subject<bool> _inflatedGumSubject;
        private Subject<Collider2D> _triggerEnter2DSubject;
        private Subject<Collider2D> _triggerStay2DSubject;

        public void Init()
        {
            _inflatedGumSubject = new Subject<bool>();
            _triggerEnter2DSubject = new Subject<Collider2D>();
            _triggerStay2DSubject = new Subject<Collider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            thisToken = this.GetCancellationTokenOnDestroy();
            
            this.OnTriggerEnter2DAsObservable()
                .Subscribe(collider2D =>
                {
                    _triggerEnter2DSubject.OnNext(collider2D);
                }).AddTo(this);
            
            this.OnTriggerStay2DAsObservable()
                .Subscribe(collider2D =>
                {
                    _triggerStay2DSubject.OnNext(collider2D);
                }).AddTo(this);
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }
        
        public void SetPosition(Vector3 newPos)
        {
            transform.position = newPos;
        }

        public void SetState(GumState newState)
        {
            state = newState;
        }

        public void OnCollider()
        {
            gumCollider.enabled = true;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (_bindableObject!=null)
            {
                return;
            }
            if (!other.collider.TryGetComponent(out IEnemyBindable bindableObject))
            {
                return;
            }
            _bindableObject = bindableObject;
            bindableObject.OnBind();
        }
        
        public void UpdateXVelocity(float speed)
        {
            _rigidbody2D.velocity = new Vector2(speed,0);
        }
        
        public Sequence MoveToEnemy(Vector2 enemyPos,float moveToEnemyTime)
        {
            return DOTween.Sequence().Append(transform.DOMove(enemyPos, moveToEnemyTime)).SetEase(Ease.InCubic);
        }

        public Sequence PlayInflateGumAnimation(Vector2 inflatedGumScale,float inflateGumTime)
        {
            return DOTween.Sequence()
                .Append(transform.DOScale(inflatedGumScale, inflateGumTime).SetEase(Ease.InCubic))
                .Join(gumSpriteRenderer.DOFade(158/255f,inflateGumTime)).SetEase(Ease.InOutCubic);
        }
        
        public void Destroy()
        {
            //TODO: エフェクト?
            Destroy(gameObject);
        }
    }
}