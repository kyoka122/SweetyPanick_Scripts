using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Player.Entity;
using InGame.Stage.View;
using InGame.Player.View;
using KanKikuchi.AudioManager;
using MyApplication;
using UnityEngine;
using Utility;
using Debug = UnityEngine.Debug;

namespace InGame.Player.Logic
{
    public class PlayerFixSweetsLogic
    {
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerInputEntity _playerInputEntity;
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;
        private readonly BasePlayerView _playerView;
        private readonly PlayerAnimatorView _playerAnimatorView;
        private readonly ObjectPool<ParticleSystem> _fixSweetsParticlePool;

        //private bool fixFlagCache=false;
        
        public PlayerFixSweetsLogic(PlayerInputEntity playerInputEntity, PlayerConstEntity playerConstEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, PlayerCommonUpdateableEntity playerCommonUpdateableEntity,
            BasePlayerView playerView,PlayerAnimatorView playerAnimatorView, ParticleGeneratorView fixSweetsParticleGeneratorView)
        {
            _playerInputEntity = playerInputEntity;
            _playerConstEntity = playerConstEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerView = playerView;
            _playerAnimatorView = playerAnimatorView;
            _fixSweetsParticlePool = new ObjectPool<ParticleSystem>(fixSweetsParticleGeneratorView);
        }

        //TODO: もう少しきれいにしたい
        public void UpdatePlayerFixSweets()
        {
            if (_playerCommonUpdateableEntity.IsDead)
            {
                return;
            }
            if (!_playerInputEntity.fixFlag)
            {
                Reset();
                return;
            }
            
            if (_playerCommonInStageEntity.currentFixingSweets!=null||_playerCommonInStageEntity.IsFixing)
            {
                if (FacedFixingGimmickSweets())
                {
                    return;
                }
                Reset();
                return;
            }
                
            ISweets[] fixableSweets=GetFacedFixableSweets();
            if (fixableSweets.Length==0)
            {
                return;
            }
            SwitchActionBySweetsType(fixableSweets);
        }

        public void Stop()
        {
            Reset();
        }
        
        public void UpdateStopping()
        {
            _playerInputEntity.OffFixFlag();
        }

        public ISweets[] GetFacedFixableSweets()
        {
            ISweets[] facedSweets = GetFacedSweets();
            ISweets[] fixableSweets = SelectFixableSweets(facedSweets);
            return fixableSweets;
        }

        private void SwitchActionBySweetsType(ISweets[] fixableSweets)
        {
            ISweets fixingSweets = fixableSweets[0];
            switch (fixingSweets.type)
            {
                case SweetsType.Sweets:
                    FixSweets(fixingSweets);
                    break;
                case SweetsType.GimmickSweets:
                    FixGimmickSweets(fixingSweets);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool FacedFixingGimmickSweets()
        {
            ISweets[] fixableSweets=GetFacedFixableSweets();
            if (fixableSweets.Length==0)
            {
                return false;
            }
            if (!fixableSweets.Contains(_playerCommonInStageEntity.currentFixingSweets))
            {
                return false;
            }

            return true;
        }

        private ISweets[] GetFacedSweets()
        {
            var direction = new Vector2(_playerCommonInStageEntity.playerDirection, 0);
            RaycastHit2D[] raycastHit2D = Physics2D.RaycastAll(_playerView.GetToSweetsRayPos(), direction,
                _playerConstEntity.ToSweetsDistance, LayerInfo.SweetsMask);
            var sweets = raycastHit2D
                .Select(hit => hit.collider.gameObject.GetComponent<ISweets>())
                .Where(sweetsComponent => sweetsComponent != null)
                .ToArray();
        
#if UNITY_EDITOR
            if (_playerView.OnDrawRay)
            {
                DrawSweetsRay(direction*_playerConstEntity.ToSweetsDistance);
            }
#endif
            return sweets;
        }
        
        
        private async void FixSweets(ISweets sweets)
        {
            var particle = _fixSweetsParticlePool.GetObject(sweets.GetPlayParticlePos(),
                _playerConstEntity.FixSweetsParticleSize(sweets.type));
            _playerCommonInStageEntity.SetFixingSweets(sweets);
            _playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.OnFix);
            SEManager.Instance.Play(SEPath.FIX_SWEETS);
            particle.Play();
            await sweets.FixSweets(_playerConstEntity.NormalSweetsFixingTime,_playerView.GetCancellationTokenOnDestroy());
            
            _playerCommonInStageEntity.SetFixingSweets(null);
            
            _fixSweetsParticlePool.ReleaseObject(particle);
            _playerInputEntity.OffFixFlag();
        }

        private async void FixGimmickSweets(ISweets sweets)
        {
            var particle = _fixSweetsParticlePool.GetObject(sweets.GetPlayParticlePos(),
                _playerConstEntity.FixSweetsParticleSize(sweets.type));
            OnFixGimmickSweets(sweets,particle);

            var fixSweetsTokenSource = new CancellationTokenSource();
            _playerCommonInStageEntity.SetFixingSweetsTokenSource(fixSweetsTokenSource);
            CancellationTokenSource tokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                fixSweetsTokenSource.Token, sweets.cancellationToken);
            
            Debug.Log($"FixStart!");
            var fixSweetsTask = sweets.FixSweets(_playerConstEntity.GimmickSweetsFixingTime, tokenSource.Token);
            await fixSweetsTask;
            Debug.Log($"TimeOver");

            OffFixGimmickSweets(tokenSource, particle);
        }

        private void OnFixGimmickSweets(ISweets sweets,ParticleSystem particle)
        {
            _playerCommonInStageEntity.SetFixingSweets(sweets);
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.Fixing, true);
            SEManager.Instance.Play(SEPath.FIX_SWEETS,isLoop:true);
            particle.Play();
        }

        private void OffFixGimmickSweets(CancellationTokenSource tokenSource,ParticleSystem particle)
        {
            if (!tokenSource.IsCancellationRequested)
            {
                _playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.Fixed);
            }
            _playerCommonInStageEntity.SetFixingSweets(null);
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.Fixing, false);
            SEManager.Instance.Stop(SEPath.FIX_SWEETS);
            _fixSweetsParticlePool.ReleaseObject(particle);
            _playerInputEntity.OffFixFlag();
        }

        private void Reset()
        {
            if (_playerCommonInStageEntity.fixingSweetsTokenSource!=null)
            {
                _playerCommonInStageEntity.fixingSweetsTokenSource.Cancel();
                _playerCommonInStageEntity.SetFixingSweetsTokenSource(null);
            }
            _playerCommonInStageEntity.SetFixingSweets(null);
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.Fixing,false);
            _playerInputEntity.OffFixFlag();
        }

        private ISweets[] SelectFixableSweets(ISweets[] facedSweets)
        {
            return facedSweets
                .Where(sweetComponent=>sweetComponent.CanFixSweets(_playerView.type))
                .ToArray();
        }
        
        
        
#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        private void DrawSweetsRay(Vector2 vector)
        {
            Debug.DrawRay(_playerView.GetToSweetsRayPos(), vector, Color.green, 0.5f);
        }
#endif
    }
}