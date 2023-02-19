using System;
using System.Linq;
using InGame.Enemy.View;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Colate.View
{
    [RequireComponent(typeof(Rigidbody2D),typeof(ColateChildComponents))]
    public class ColateView:MonoBehaviour,IEnemyDamageAble,IDisposable
    {
        public bool OnDrawRay => onDrawRay;
        public IObservable<Collision2D> OnCollisionEnterEvent=>_onCollisionEnterSubject;
        public IObservable<bool> Attacked=>_attackedSubject;
        
        [SerializeField] private bool onDrawRay;
        
        private ColateChildComponents _colateChildComponents;
        private ColateSprite[] _colateSprites;
        private Rigidbody2D _rigidbody;

        private GameObject _currentSpriteObj;
        private Subject<Collision2D> _onCollisionEnterSubject;
        private Subject<bool> _attackedSubject;
        private float _defaultGravityScale;
        
        
        public void Init()
        {
            _onCollisionEnterSubject = new Subject<Collision2D>();
            _attackedSubject = new Subject<bool>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _colateChildComponents = GetComponent<ColateChildComponents>();
            _defaultGravityScale = _rigidbody.gravityScale;
        }
        
        public int GetDirectionX()
        {
            float xVelocity=_rigidbody.velocity.x;
            switch (xVelocity)
            {
                case 0:
                    return 0;
                case > 0:
                    return 1;
                case < 0:
                    return -1;
                default:
                    Debug.LogError($"Couldn`t Get xDirection.");
                    return 0;
            }
        }

        public void OnDamaged(Struct.DamagedInfo info)
        {
            if (info.attacker==Attacker.Player)
            {
                
            }
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }

        public Vector2 GetToSweetsRayPos()
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
            _onCollisionEnterSubject?.Dispose();
            _attackedSubject?.Dispose();
        }
    }
}