using System;
using System.Linq;
using InGame.Enemy.View;
using MyApplication;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.Colate.View
{
    [RequireComponent(typeof(Rigidbody2D),typeof(ColateChildComponents),typeof(Animator))]
    public class ColateView:MonoBehaviour,IEnemyDamageAble,IDisposable
    {
        public bool OnDrawRay => onDrawRay;
        public IObservable<Collider2D> OnTriggerEnterEvent=>_onTriggerEnterSubject;
        public IObservable<bool> Attacked=>_attackedSubject;
        
        [SerializeField] private bool onDrawRay;
        [SerializeField] private ColateSprite[] _colateSprites;
        
        private ColateChildComponents _colateChildComponents;
        private Rigidbody2D _rigidbody;
        private Animator _animator;

        private GameObject _currentSpriteObj;
        private Subject<Collider2D> _onTriggerEnterSubject;
        private Subject<bool> _attackedSubject;
        private float _defaultGravityScale;

        private int _prevDirection;
        
        public void Init()
        {
            _onTriggerEnterSubject = new Subject<Collider2D>();
            _attackedSubject = new Subject<bool>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _colateChildComponents = GetComponent<ColateChildComponents>();
            _animator = GetComponent<Animator>();
            
            _defaultGravityScale = _rigidbody.gravityScale;
            _prevDirection = -1;
            _currentSpriteObj = _colateSprites.FirstOrDefault(data => data.Type == ColateSpriteType.Stand)?.SpriteObj;
            if (_currentSpriteObj!=null)
            {
                _currentSpriteObj.SetActive(true);
            }
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            this.OnTriggerEnter2DAsObservable()
                .Subscribe(collider2D => _onTriggerEnterSubject.OnNext(collider2D));
        }
        
        public int GetDirectionX()
        {
            return _prevDirection;
        }
        
        public float AdjustModelDirectionX(float origin)
        {
            return Mathf.Abs(origin) *_prevDirection;
        }
        
        public Vector2 AdjustModelDirectionX(Vector2 origin)
        {
            return new(Mathf.Abs(origin.x)*_prevDirection, origin.y);
        }

        public void OnDamaged(Struct.DamagedInfo info)
        {
            Debug.Log($"Attacked!!!!! Attacker:{info.attacker}");
            if (info.attacker==Attacker.Player)
            {
                _attackedSubject.OnNext(true);
            }
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }

        public Vector2 GetToSideWallRayPos()
        {
            return _colateChildComponents.ToSideWallRayPos;
        }

        public Vector2 GetVelocity()
        {
            return _rigidbody.velocity;
        }

        public void SetVelocity(Vector2 newVelocity)
        {
            _rigidbody.velocity = newVelocity;
        }

        public void AddVelocity(Vector2 addVelocity)
        {
            _rigidbody.velocity += addVelocity;
        }

        public void SetActiveGravity(bool active)
        {
            if (active)
            {
                _rigidbody.gravityScale=_defaultGravityScale;
                return;
            }
            _rigidbody.gravityScale=0;
        }


        public Vector2 GetToGroundRayPos()
        {
            return _colateChildComponents.ToGroundRayPos;
        }
        
        public void TurnAround()
        {
           transform.Rotate(new Vector3(0,180f,0));
           _prevDirection *= -1;
        }


        public void SetSprite(ColateSpriteType type)
        {
            GameObject spriteObj = _colateSprites.FirstOrDefault(data => data.Type == type)?.SpriteObj;
            if (spriteObj==null)
            {
                Debug.LogError($"Couldn`t Find ColateSprite. type:{type}");
                return;
            }
            _currentSpriteObj.SetActive(false);
            spriteObj.SetActive(true);
            _currentSpriteObj = spriteObj;
        }

        public void Dispose()
        {
            _onTriggerEnterSubject?.Dispose();
            _attackedSubject?.Dispose();
        }

        /// <summary>
        /// Y軸と回転のconstrainsを固定する
        /// </summary>
        public void FreezeConstrainsY()
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY|RigidbodyConstraints2D.FreezeRotation;
        }
        
        public void FreePositionConstrain()
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        public void OnExplosionEffect()
        {
            _colateChildComponents.ExplosionAnimator.SetTrigger(ColateAnimatorParameter.OnEffect);
        }


        public void SetBoolAnimation(string animationName,bool on)
        {
            _animator.SetBool(animationName,on);
        }
    }
}