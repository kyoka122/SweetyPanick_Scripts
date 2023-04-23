using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyApplication;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.Player.View
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class BasePlayerView : MonoBehaviour
    {
        public bool OnDrawRay => onDrawRay;
        public bool IgnoreCheckGround => ignoreCheckGround;
        public abstract PlayableCharacter type { get; }
        public CancellationToken thisToken { get; private set; }
        public IObservable<Collision2D> OnCollisionEnterObj=>_onCollisionEnterObjSubject;

        [SerializeField] private bool onDrawRay;
        [SerializeField, Tooltip("チェックを入れると何回でもジャンプできるようになる")]
        private bool ignoreCheckGround;

        private PlayerChildComponents _playerChildComponents;
        private Rigidbody2D _rigidbody;
        private Subject<Collision2D> _onCollisionEnterObjSubject;
        private Collider2D[] _activeColliderCache;
        private SpriteRenderer[] _activeSpriteRenderers;

        public void Init()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerChildComponents = GetComponent<PlayerChildComponents>();
            _onCollisionEnterObjSubject = new Subject<Collision2D>();
            thisToken = this.GetCancellationTokenOnDestroy();
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            this.OnCollisionEnter2DAsObservable()
                .Subscribe(collision2D =>
                {
                    _onCollisionEnterObjSubject.OnNext(collision2D);
                })
                .AddTo(this);

        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }
        
        public void SetPosition(Vector2 pos)
        {
            transform.position = pos;
        }
        
        public Transform GetTransform()
        {
            return transform;
        }

        public Vector2 GetVelocity()
        {
            return _rigidbody.velocity;
        }

        public GameObject GetWeaponObject()
        {
            return _playerChildComponents.WeaponColliderObject;
        }

        public GameObject GetAnimatorObject()
        {
            return _playerChildComponents.Animator;
        }
        
        public Transform GetModelTransform()
        {
            return _playerChildComponents.ModelTransform;
        }

        public Vector2 GetDownToGroundRayPos()
        {
            return _playerChildComponents.DownGroundRayPos;
        }
        
        public Vector2 GetUpGroundRayPos()
        {
            return _playerChildComponents.UpGroundRayPos;
        }

        public Vector2 GetToSweetsRayPos()
        {
            return _playerChildComponents.ToSweetsRayPos;
        }
        
        public Vector2 GetToSlopeRayPos()
        {
            return _playerChildComponents.ToSlopeRayPos;
        }
        
        public void SetScale(Vector3 newScale)
        {
            transform.localScale = newScale;
        }

        public void SetChild(Transform child)
        {
            child.parent = transform;
        }

        public void SetModelScale(Vector3 newScale)
        {
            _playerChildComponents.ModelTransform.localScale = newScale;
        }

        public void SetModelRotation(Quaternion newQuaternion)
        {
            _playerChildComponents.ModelTransform.rotation = newQuaternion;
        }

        public void SetXVelocity(float newXVelocity)
        {
            var velocityCache = _rigidbody.velocity;
            velocityCache.x = newXVelocity;
            _rigidbody.velocity = velocityCache;
        }
        
        public void SetYVelocity(float newYVelocity)
        {
            var velocityCache = _rigidbody.velocity;
            velocityCache.y = newYVelocity;
            _rigidbody.velocity = velocityCache;
        }

        public void AddXVelocity(float newXVelocity,ForceMode2D mode)
        {
            _rigidbody.AddForce(new Vector2(newXVelocity,0),mode);
        }
        
        public void AddYVelocity(float newYVelocity)
        {
            var velocityCache = _rigidbody.velocity;
            velocityCache.y += newYVelocity;
            _rigidbody.velocity = velocityCache;
        }

        public void OffCollider()
        {
            _activeColliderCache = GetComponents<Collider2D>().Where(collider => collider.enabled).ToArray();
            foreach (var collider in _activeColliderCache)
            {
                collider.enabled = false;
            }
        }

        public void OnCollider()
        {
            foreach (var collider in _activeColliderCache)
            {
                collider.enabled = true;
            }
            _activeColliderCache = null;
        }
        
        public void OffSprite()
        {
            _activeSpriteRenderers =
                GetComponentsInChildren<SpriteRenderer>().Where(renderer => renderer.enabled).ToArray();
            foreach (var renderer in _activeSpriteRenderers)
            {
                renderer.enabled = false;
            }
        }

        public void OnSprite()
        {
            foreach (var renderer in _activeSpriteRenderers)
            {
                renderer.enabled = true;
            }

            _activeSpriteRenderers = null;
        }
        public ParticleSystem InstanceParticle(ParticleSystem particle)
        {
            return Instantiate(particle, transform.position, particle.transform.rotation);
        }
        
        public void SetPlayerIcon(bool on)
        {
            _playerChildComponents.PlayerIcon.enabled = on;
        }

        public void PlayParticle(ParticleSystem particle)
        {
            particle.Play();
        }

        public void PlayParticleAtPlayerPos(ParticleSystem particle)
        {
            particle.transform.position = transform.position;
            particle.Play();
        }
        
        public void PlayParticleAtPlayerWeaponPos(ParticleSystem particle)
        {
            particle.transform.position = _playerChildComponents.AttackParticlePos;
            particle.Play();
        }

        public void StopParticle(ParticleSystem particle)
        {
            particle.Stop();
        }

        public async UniTask Warp(Vector2 endPos,float duration,CancellationToken token)
        {
            await transform.DOMove(endPos, duration)
                .SetEase(Ease.InOutQuart)
                .ToUniTask(cancellationToken: token);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void ChangeWeaponColliderSize(Vector2 newSize)
        {
            _playerChildComponents.WeaponColliderObject.transform.localScale = newSize;
        }

        public void SetLayer(int layerNum)
        {
            gameObject.layer = layerNum;
        }

        private void OnDestroy()
        {
            _onCollisionEnterObjSubject.Dispose();
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}