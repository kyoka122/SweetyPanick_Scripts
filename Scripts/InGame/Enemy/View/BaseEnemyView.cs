using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Enemy.Interface;
using InGame.Stage.View;
using InGame.Player.View;
using MyApplication;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Collision2D = UnityEngine.Collision2D;

namespace InGame.Enemy.View
{
    [RequireComponent(typeof(Rigidbody2D),typeof(EnemyChildComponents)),]
    public abstract class BaseEnemyView:MonoBehaviour,IEnemyDamageAble,IEnemyBindable,IEnemyDecoyAble,IEnemyPullAble,
        ICollideAbleToPlayer,IColateOrderAble
    {
        public IObservable<Collision2D> SearchedCollisionObject => _searchedCollisionObject;
        public IObservable<Collision2D> OnHitFlyingCollider => hitFlyingCollider;
        public IObservable<Vector2> OnPlayerPunch => _punchSubject;
        public IObservable<bool> OnRolled => _rolledSubject;
        public IObservable<bool> OnBindByPlayer => _bindByPlayerSubject;
        public IObservable<Transform> OnDecoyByPlayer => _decoyByPlayerSubject;
        public IObservable<Transform> OnPullByPlayer => _pullByPlayerSubject;
        public bool OnDrawRay => onDrawRay;
        public Vector2 ToSweetsRayPos => enemyChildComponents.ToSweetsRayPos;
        public Vector2 CenterPos => enemyChildComponents.CenterPos;
        public CancellationToken ChangeDirectionToken => _changeDirectionTaskSource.Token;
        public CancellationToken ReleaseGumReactionToken => _releaseGumReactionTokenSource.Token;
        public CancellationToken EatSweetsToken => _eatSweetsTokenSource.Token;
        
        public CancellationToken thisToken { get; private set; }
        public EnemyState state { get; private set; } = EnemyState.Walk;
        public bool inScreen { get; private set; } = true;
        public bool inScreenX { get; private set; } = true;
        public int enemyDirectionX { get; private set; }
        public ISweets eatingSweets { get; private set; }
        public float currentEatingTime { get; private set; }
        public Vector2 flyAwayDirection { get; private set; }= Vector2.zero;
        public Transform decoyTransform { get; private set; }
        public Transform pullingWeaponTransform { get; private set; }
        public Collider2D[] takeOffGroundColliders { get; private set; }

        public bool isBindable => state is not (EnemyState.Fly or EnemyState.isPulled or EnemyState.Death);
        public bool isDecoyAble => state is not (EnemyState.Fly or EnemyState.Bind or EnemyState.isPulled or EnemyState.Death);
        public bool isPullAble => state is not (EnemyState.Fly or EnemyState.Bind or EnemyState.Death);


        private readonly Quaternion TowardRightRot = Quaternion.Euler(0, 180, 0);
        private readonly Quaternion TowardLeftRot = Quaternion.Euler(0, 0, 0);
        
        [SerializeField] private float moveLeftLimit;
        [SerializeField] private float moveRightLimit;
        [SerializeField] private bool isRightMoveFirst;
        [SerializeField] private EnemyChildComponents enemyChildComponents;
        [SerializeField] private bool onDrawRay;
        
        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private CancellationTokenSource _changeDirectionTaskSource;
        private CancellationTokenSource _releaseGumReactionTokenSource;
        private CancellationTokenSource _eatSweetsTokenSource;

        private Subject<Collision2D> _searchedCollisionObject;
        private Subject<Vector2> _punchSubject;
        private Subject<bool> _rolledSubject;
        private Subject<bool> _bindByPlayerSubject;
        private Subject<Transform> _decoyByPlayerSubject;
        private Subject<Transform> _pullByPlayerSubject;
        private Subject<Collision2D> hitFlyingCollider;
        private string _currentAnimatorState;
        private List<Collision2D> _onCollisionStays;

        public void Init()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = enemyChildComponents.Animator;
            _changeDirectionTaskSource = new CancellationTokenSource();
            _releaseGumReactionTokenSource = new CancellationTokenSource();
            _eatSweetsTokenSource = new CancellationTokenSource();
            _searchedCollisionObject = new Subject<Collision2D>();
            _punchSubject = new Subject<Vector2>();
            _rolledSubject = new Subject<bool>();
            _bindByPlayerSubject = new Subject<bool>();
            _decoyByPlayerSubject = new Subject<Transform>();
            _pullByPlayerSubject = new Subject<Transform>();
            _onCollisionStays = new List<Collision2D>();
            hitFlyingCollider = new Subject<Collision2D>();
            enemyDirectionX = isRightMoveFirst ? 1 : -1;
            thisToken = this.GetCancellationTokenOnDestroy();
            
            this.OnCollisionEnter2DAsObservable()
                .Subscribe(collision2D =>
                {
                    if (collision2D.otherCollider.gameObject.layer==LayerInfo.EnemyNum)
                    {
                        _onCollisionStays.Add(collision2D);
                        _searchedCollisionObject.OnNext(collision2D);
                    }

                    if (collision2D.otherCollider.gameObject.layer==LayerInfo.EnemyFlyingNum)
                    {
                        hitFlyingCollider.OnNext(collision2D);
                    }
                })
                .AddTo(this);
            
            this.OnCollisionExit2DAsObservable()
                .Subscribe(collision2D =>
                {
                    if (collision2D.otherCollider.gameObject.layer==LayerInfo.EnemyNum)
                    {
                        _onCollisionStays.Remove(collision2D);
                    }
                    
                })
                .AddTo(this);
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }
        
        public Vector2 GetLocalPosition()
        {
            return transform.localPosition;
        }
        
        public void SetPosition(Vector2 pos)
        {
            transform.position = pos;
        }
        
        public Quaternion GetRotation()
        {
            return transform.rotation;
        }
        
        public void SetRotation(Quaternion rot)
        {
            transform.rotation = rot;
        }
        
        public void SetState(EnemyState newState)
        {
            //Debug.Log($"newState; {newState}");
            state = newState;
        }
        
        public void SwitchDirection(CancellationToken token)
        {
            enemyDirectionX *= -1;
            SetSpriteRotation(token);
        }

        public void SetDirection(int direction)
        {
            enemyDirectionX = direction > 0 ? 1 : -1;
            SetSpriteRotation(CancellationToken.None);
        }

        public bool OutOfMoveLimit(Vector2 pos,int direction)
        {
            return direction switch
            {
                > 0 => pos.x > moveRightLimit,
                < 0 => pos.x < moveLeftLimit,
                _ => false
            };
        }
        
        public void SetFlyAwayDirection(Vector2 direction)
        {
            flyAwayDirection = direction;
        }
        
        public Vector2 GetVelocity()
        {
            return _rigidbody2D.velocity;
        }

        public void AddVelocity(Vector2 velocity)
        {
            _rigidbody2D.velocity += velocity;
        }

        public void SetVelocity(Vector2 velocity)
        {
            _rigidbody2D.velocity = velocity;
        }

        public void SetAngularVelocity(float rotVelocity)
        {
            _rigidbody2D.angularVelocity = rotVelocity;
        }

        public void SetEatingSweets(ISweets sweets)
        {
            eatingSweets = sweets;
        }

        public void SetFreezeRotation(bool on)
        {
            _rigidbody2D.constraints = on ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.None;
        }

        public void SetCurrentEatingTime(float newTime)
        {
            currentEatingTime = newTime;
        }
        
        public void OnDamaged(Struct.DamagedInfo info)
        {
            switch (info.attacker)
            {
                case Attacker.Player:
                    _punchSubject.OnNext(info.attackerPos);
                    break;
                case Attacker.Crepe:
                    _rolledSubject.OnNext(true);
                    break;
                default:
                    Debug.Log($"Not Found Type:{info.attacker}");
                    break;
            }
        }
        public void SetInScreen(bool on)
        {
            inScreen = on;
        } 
        
        public void SetInScreenX(bool on)
        {
            inScreenX = on;
        }

        public void SetGravity(float scale)
        {
            _rigidbody2D.gravityScale = scale;
        }

        public void SetChangeDirectionTokenSource(CancellationTokenSource newTokenSource)
        {
            _changeDirectionTaskSource = newTokenSource;
        }

        public void CancelChangeDirectionTokenSource()
        {
            if (!_changeDirectionTaskSource.IsCancellationRequested)
            {
                _changeDirectionTaskSource.Cancel();
            }

            if (!_releaseGumReactionTokenSource.IsCancellationRequested)
            {
                _releaseGumReactionTokenSource.Cancel();
            }
            
        }

        public void SetEatSweetsTokenSource(CancellationTokenSource newTokenSource)
        {
            _eatSweetsTokenSource = newTokenSource;
        }
        
        public void CancelEatSweetsTokenSource()
        {
            _eatSweetsTokenSource.Cancel();
        }

        public void SetTakeOffCollider()
        {
            var colliders = new List<Collider2D>();
            foreach (var stay in _onCollisionStays)
            {
                var contacts=new List<ContactPoint2D>();
                stay.GetContacts(contacts);
                if (contacts.Any(contact => contact.point.y <= gameObject.transform.position.y))
                {
                   colliders.Add(stay.collider); 
                }
                
            }
            takeOffGroundColliders = colliders.ToArray();
        }
        
        public Collider2D GetUseDuringFlyingCollider()
        {
            return enemyChildComponents.UseDuringFlyingCollider;
        }

        public void SetCollidersCaseFlying()
        {
            enemyChildComponents.UseDuringFlyingCollider.enabled = true;
        }

        public void SetDecoyTransform(Transform newDecoyTransform)
        {
            decoyTransform = newDecoyTransform;
        }
        
        public void SetPullingWeaponTransform(Transform newWeaponTransform)
        {
            pullingWeaponTransform = newWeaponTransform;
        }

        public void PlayBoolAnimation(string stateName,bool on)
        {
            _animator.SetBool(stateName,on);
        }
        
        public void OffAllAnimationParameter()
        {
            _animator.SetBool(EnemyAnimatorStateName.Idle,false);
            _animator.SetBool(EnemyAnimatorStateName.Eat,false);
            _animator.SetBool(EnemyAnimatorStateName.Jump,false);
            _animator.SetBool(EnemyAnimatorStateName.Cry,false);
            _animator.SetBool(EnemyAnimatorStateName.Sleep,false);
        }

        public void OnBind()
        {
            _bindByPlayerSubject.OnNext(true);
        }

        public void OffBind()
        {
            _bindByPlayerSubject.OnNext(false);
        }

        public void OnDecoy(Transform newDecoyTransform)
        {
            _decoyByPlayerSubject.OnNext(newDecoyTransform);
        }

        public void OffDecoy()
        {
            _decoyByPlayerSubject.OnNext(null);
        }

        public void OnPull(Transform weaponTransform)
        {
            _pullByPlayerSubject.OnNext(weaponTransform);
        }
        
        public void OffPull()
        {
            _pullByPlayerSubject.OnNext(null);
        }

        public SpriteEffectView InstanceDestroyEffect(SpriteEffectView shockSpriteView,Vector2 pos)
        {
            return Instantiate(shockSpriteView, pos, shockSpriteView.transform.rotation);
        }
        
        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void SetSpriteRotation(CancellationToken token)
        {
            if (enemyDirectionX>0)
            {
                transform.rotation = TowardRightRot;
            }

            if (enemyDirectionX<0)
            {
                transform.rotation = TowardLeftRot;
            }
        }
    }
}