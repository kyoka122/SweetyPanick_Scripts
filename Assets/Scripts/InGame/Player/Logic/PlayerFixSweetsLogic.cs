using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using InGame.Player.Entity;
using InGame.Stage.View;
using InGame.Player.View;
using KanKikuchi.AudioManager;
using MyApplication;
using UnityEngine;
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
        

        //private bool fixFlagCache=false;
        
        public PlayerFixSweetsLogic(PlayerInputEntity playerInputEntity, PlayerConstEntity playerConstEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, PlayerCommonUpdateableEntity playerCommonUpdateableEntity,
            BasePlayerView playerView,PlayerAnimatorView playerAnimatorView)
        {
            _playerInputEntity = playerInputEntity;
            _playerConstEntity = playerConstEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerView = playerView;
            _playerAnimatorView = playerAnimatorView;
        }
        
        //TODO: もう少しきれいにしたい
        public void UpdatePlayerFixSweets()
        {
            if (_playerCommonUpdateableEntity.IsDead)
            {
                ResetAtOffFlag();
                return;
            }
            if (!_playerInputEntity.fixFlag)
            {
                ResetAtOffFlag();
                return;
            }
            
            ISweets[] facedSweets=GetFacedSweets();
            if (_playerCommonInStageEntity.currentFixingSweets!=null)
            {
                SwitchActionUntilFixing(facedSweets);
                return;
            }
            SwitchActionUntilNotFixing(facedSweets);
        }

        private void SwitchActionUntilFixing(ISweets[] facedSweets)
        {
            //MEMO: 修復中のお菓子がギミックお菓子だったら
            if (_playerCommonInStageEntity.currentFixingSweets.type == SweetsType.GimmickSweets)
            {
                if (FacedFixingSweets(facedSweets))
                {
                    return;
                }
                _playerCommonInStageEntity.SetFixingSweets(null);
                ResetAtOffFlag();
                return;
            }
                
            //MEMO: 修復中のお菓子がギミックお菓子じゃなかったら
            ISweets[] fixableSweets = SelectFixableSweets(facedSweets);
            foreach (var fixableSweet in fixableSweets)
            {
                TryFixAdditionalSweets(fixableSweet);
            }

            _playerInputEntity.OffFixFlag();
        }
        
        private void TryFixAdditionalSweets(ISweets sweets)
        {
            if (sweets.CanFixSweets(_playerView.type))
            {
                var particle = _playerCommonInStageEntity.fixSweetsParticlePool.GetObject(sweets.GetPlayParticlePos(),
                    _playerConstEntity.FixSweetsParticleSize(sweets.type));
                particle.Play();
                FixAdditionalSweetsAsync(sweets, particle);
            }
        }

        private async void FixAdditionalSweetsAsync(ISweets sweets,ParticleSystem particleSystem)
        {
            await sweets.FixSweets(_playerConstEntity.NormalSweetsFixingTime,_playerView.thisToken);
            _playerCommonInStageEntity.fixSweetsParticlePool.ReleaseObject(particleSystem);
        }

        private void SwitchActionUntilNotFixing(ISweets[] facedSweets)
        {
            ISweets[] fixableSweets = SelectFixableSweets(facedSweets);
            if (fixableSweets.Length==0)
            {
                _playerInputEntity.OffFixFlag();
                return;
            }
            SwitchActionBySweetsType(fixableSweets[0]);
        }

        public void Stop()
        {
            _playerCommonInStageEntity.SetFixingSweets(null);
            ResetAtOffFlag();
        }
        
        public void UpdateStopping()
        {
            _playerInputEntity.OffFixFlag();
        }
        
        /// <summary>
        /// 一番Player側にあるオブジェクトを修復する
        /// </summary>
        /// <param name="fixableSweets"></param>
        private void SwitchActionBySweetsType(ISweets fixableSweets)
        {
            switch (fixableSweets.type)
            {
                case SweetsType.Sweets:
                    OnFixNormalSweets(fixableSweets);
                    _playerInputEntity.OffFixFlag();
                    break;
                case SweetsType.GimmickSweets:
                    FixGimmickSweets(fixableSweets);
                    break;
                default:
                    Debug.LogError($"Unknown SweetType");
                    return;
            }
        }

        /// <summary>
        /// 直してる最中のギミックお菓子が目の前にあるかどうか
        /// </summary>
        /// <returns></returns>
        private bool FacedFixingSweets(ISweets[] fixableSweets)
        {
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
        
        /// <summary>
        /// まだ直してない通常お菓子が目の前にあるかどうか
        /// </summary>
        /// <returns></returns>
        private bool FacedFixingNormalSweets(ISweets[] fixableSweets)
        {
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
        
        
        private async void OnFixNormalSweets(ISweets sweets)
        {
            var particle = _playerCommonInStageEntity.fixSweetsParticlePool.GetObject(sweets.GetPlayParticlePos(),
                _playerConstEntity.FixSweetsParticleSize(sweets.type));
            _playerCommonInStageEntity.SetFixingSweets(sweets);
            _playerCommonInStageEntity.SetIsNormalSweetsFixing(true);
            _playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.OnFix);
            SEManager.Instance.Play(SEPath.FIX_SWEETS);
            particle.Play();
            
            //MEMO: ↓プレイヤー切り替えが実装されたら解除+パラメータ修正
            /*if (sweets.Specialist==_playerView.type)//MEMO: 担当お菓子の時
            {
                await sweets.FixSweets(_playerConstEntity.NormalSweetsSpecialistFixingTime,_playerView.GetCancellationTokenOnDestroy());
            }
            else
            {
                await sweets.FixSweets(_playerConstEntity.NormalSweetsFixingTime,_playerView.GetCancellationTokenOnDestroy());
            }*/
            sweets.FixSweets(_playerConstEntity.NormalSweetsFixingTime,_playerView.thisToken).Forget();

            if (!_playerAnimatorView.IsFixingSweets())
            {
                await UniTask.WaitUntil(() => _playerAnimatorView.IsFixingSweets(), cancellationToken: _playerView.thisToken);
            }
            await UniTask.WaitWhile(() => _playerAnimatorView.IsFixingSweets(), cancellationToken: _playerView.thisToken);
            _playerCommonInStageEntity.SetFixingSweets(null);
            _playerCommonInStageEntity.fixSweetsParticlePool.ReleaseObject(particle);
            _playerInputEntity.OffFixFlag();
            _playerCommonInStageEntity.SetIsNormalSweetsFixing(false);
        }

        private async void FixGimmickSweets(ISweets sweets)
        {
            var particle = _playerCommonInStageEntity.fixSweetsParticlePool.GetObject(sweets.GetPlayParticlePos(),
                _playerConstEntity.FixSweetsParticleSize(sweets.type));
            int seID = SEManager.Instance.Play(SEPath.FIX_SWEETS, isLoop: true);
            OnFixGimmickSweets(sweets,particle);

            var fixSweetsTokenSource = new CancellationTokenSource();
            _playerCommonInStageEntity.SetFixingSweetsTokenSource(fixSweetsTokenSource);//MEMO: 途中キャンセルをするため
            CancellationTokenSource tokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                fixSweetsTokenSource.Token, sweets.cancellationToken);
            
            Debug.Log($"FixStart!");
            //MEMO: ↓プレイヤー切り替えが実装されたら解除+パラメータ修正
            /*if (sweets.Specialist==_playerView.type)//MEMO: 担当お菓子の時
            {
                await sweets.FixSweets(_playerConstEntity.GimmickSweetsSpecialistFixingTime, tokenSource.Token);
            }
            else
            {
                await sweets.FixSweets(_playerConstEntity.GimmickSweetsFixingTime, tokenSource.Token);
            }*/
            await sweets.FixSweets(_playerConstEntity.GimmickSweetsFixingTime, tokenSource.Token);
            
            Debug.Log($"TimeOver");
            OffFixGimmickSweets(tokenSource, particle, seID);
        }

        private void OnFixGimmickSweets(ISweets sweets,ParticleSystem particle)
        {
            _playerCommonInStageEntity.SetFixingSweets(sweets);
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.Fixing, true);
            particle.Play();
        }

        private void OffFixGimmickSweets(CancellationTokenSource tokenSource,ParticleSystem particle,int seID)
        {
            if (!tokenSource.IsCancellationRequested)
            {
                _playerAnimatorView.PlayTriggerAnimation(PlayerAnimatorParameter.Fixed);
            }
            _playerCommonInStageEntity.SetFixingSweets(null);
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.Fixing, false);
            SEManager.Instance.Stop(seID);
            _playerCommonInStageEntity.fixSweetsParticlePool.ReleaseObject(particle);
            _playerInputEntity.OffFixFlag();
        }

        private void ResetAtOffFlag()
        {
            if (_playerCommonInStageEntity.fixingSweetsTokenSource!=null)
            {
                _playerCommonInStageEntity.fixingSweetsTokenSource.Cancel();
                _playerCommonInStageEntity.SetFixingSweetsTokenSource(null);
            }
            //_playerCommonInStageEntity.SetFixingSweets(null);
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
            Debug.DrawRay(_playerView.GetToSweetsRayPos(), vector, Color.green, 3);
        }
#endif
    }
}