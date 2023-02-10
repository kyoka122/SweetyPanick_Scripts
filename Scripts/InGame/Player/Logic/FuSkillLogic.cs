using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InGame.Enemy.View;
using InGame.Player.Entity;
using InGame.Player.View;
using KanKikuchi.AudioManager;
using MyApplication;
using UniRx;
using UnityEngine;
using Utility;

namespace InGame.Player.Logic
{
    public class FuSkillLogic:BasePlayerSkillLogic
    {
        private BindGumView _bindGumView;//TODO: Entityにする？
        private readonly FuConstEntity _fuConstEntity;
        private readonly FuView _fuView;
        private IEnemyBindable _bindingEnemy;

        public FuSkillLogic(PlayerInputEntity playerInputEntity, PlayerConstEntity playerConstEntity,FuConstEntity fuConstEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, FuView fuView, PlayerAnimatorView playerAnimatorView,
            WeaponView weaponView)
            : base(playerInputEntity, playerCommonInStageEntity, playerConstEntity, fuView, playerAnimatorView, weaponView)
        {
            _fuConstEntity = fuConstEntity;
            _fuView = fuView;
        }

        private void RegisterGumObserver()
        {
            _bindGumView.OnTriggerEnter.Subscribe(TryCatchEnemy).AddTo(_fuView);
            _bindGumView.OnTriggerStay.Subscribe(TryCatchEnemy).AddTo(_fuView);
            
            _bindGumView.InflatedGum
                .Subscribe(_ => _bindGumView.SetState(GumState.Move))
                .AddTo(_fuView);
        }

        public override void UpdatePlayerSkill()
        {
            if (_bindGumView == null)
            {
                TryOnSkill();
                return;
            }
            UpdateGeneratedGum();
            if (_bindingEnemy == null)
            {
                Debug.Log($"No bindingEnemy");
                return;
            }

            CheckEnemyBindable();
        }

        private void UpdateGeneratedGum()
        {
            switch (_bindGumView.state)
            {
                case GumState.Inflating:
                    break;
                case GumState.Move:
                    CheckOutOfScreen();
                    MoveGum();
                    break;
                case GumState.Catching:
                    break;
                case GumState.Catched:
                    BindEnemy();
                    break;
                case GumState.Destroy:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TryOnSkill()
        {
            if (!playerInputEntity.skillFlag)
            {
                return;
            }
     
            playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.OnSkill);
            _bindGumView = _fuView.GenerateBindGum(_fuConstEntity.bindGumView, (Vector2) _fuView.transform.position +
                new Vector2(_fuConstEntity.toGumInstanceVec.x * playerCommonInStageEntity.playerDirection,
                    _fuConstEntity.toGumInstanceVec.y));
            SEManager.Instance.Play(SEPath.FUSEN);
            _bindGumView.Init();
            RegisterGumObserver();

            _bindGumView.PlayInflateGumAnimation(_fuConstEntity.inflatedGumScale, _fuConstEntity.inflateGumTime)
                .OnComplete(async () =>
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_fuConstEntity.gumInflateToMoveInterval),
                        cancellationToken: _bindGumView.thisToken);
                    _bindGumView.OnCollider();
                    _bindGumView.SetState(GumState.Move);
                });
            
            playerCommonInStageEntity.OnSkillTrigger();
            playerInputEntity.OffSkillFlag();
        }

        private void CheckOutOfScreen()
        {
            Vector2 viewPortPos = playerConstEntity.WorldToViewPortPoint(_bindGumView.GetPosition());
            bool inScreen = SquareRangeCalculator.InSquareRange(viewPortPos, _fuConstEntity.objectInScreenRange);

            if (!inScreen)
            {
                _bindingEnemy?.OffBind();
                DestroyGum(_bindGumView);
            }
        }

        private void MoveGum()
        {
            _bindGumView.UpdateXVelocity(_fuConstEntity.gumMoveSpeed);
        }

        private void TryCatchEnemy(Collider2D hitCollider)
        {
            if (_bindGumView.state!=GumState.Move||_bindingEnemy != null)
            {
                return;
            }

            var newBindEnemy = hitCollider.GetComponent<IEnemyBindable>();

            if (newBindEnemy != null && newBindEnemy.isBindable)
            {
                Debug.Log($"Catch");
                _bindingEnemy = newBindEnemy;
                Debug.Log($"Set bindingEnemy:{_bindingEnemy}");
                Debug.Log($"Set newBindEnemy:{newBindEnemy}");
                _bindingEnemy.OnBind();

                _bindGumView.SetState(GumState.Catching);
                MoveToEnemy();
            }

        }
        
        private void MoveToEnemy()
        {
            _bindGumView.MoveToEnemy(_bindingEnemy.CenterPos, _fuConstEntity.gumMoveToEnemyTime)
                .OnComplete(() =>
                {
                    _bindGumView.SetState(GumState.Catched);
                    TimeBindEnemy(_bindGumView, _bindGumView.thisToken).Forget();
                });
            
        }

        private void BindEnemy()
        {
            _bindGumView.SetPosition(_bindingEnemy.CenterPos);
        }

        private async UniTask TimeBindEnemy(BindGumView bindGumViewCache,CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_fuConstEntity.gumAliveTime), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Gum Destroyed During BindTime");
            }
            
            if (bindGumViewCache!=null)
            {
                _bindingEnemy?.OffBind();
                DestroyGum(bindGumViewCache);
            }
        }

        private void CheckEnemyBindable()
        {
            if (!_bindingEnemy.isBindable)
            {
                Debug.Log($"OffBind");
                _bindingEnemy.OffBind();
                DestroyGum(_bindGumView);
            }
        }

        private void DestroyGum(BindGumView bindGumView)
        {
            //エフェクト出す？
            Debug.Log($"DestroyGum!!!!!!!!!!!!!!!!!!!!!!!!!");
            bindGumView.SetState(GumState.Destroy);
            _bindingEnemy = null;
            bindGumView.Destroy();
        }

    }
}