using System;
using System.Threading;
using Common.View;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using InGame.Player.Entity;
using InGame.Player.View;
using MyApplication;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Utility;

namespace InGame.Player.Logic
{
    public class PlayerReviveLogic
    {
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly BasePlayerView _playerView;
        private readonly PlayerStatusView _playerStatusView;
        private readonly ActionKeyView _actionKeyView;

        private CallbackAnimatorView _reviveAnimator;

        public PlayerReviveLogic(PlayerCommonUpdateableEntity playerCommonUpdateableEntity, 
            PlayerCommonInStageEntity playerCommonInStageEntity, BasePlayerView playerView, 
            PlayerStatusView playerStatusView, CallbackAnimatorGeneratorView reviveCharacterCallbackAnimatorView,
            ActionKeyView actionKeyView)
        {
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerView = playerView;
            _playerStatusView = playerStatusView;
            _actionKeyView = actionKeyView;
            RegisterObserver();
            RegisterAnimator(reviveCharacterCallbackAnimatorView);
        }

        private void RegisterObserver()
        {
            _playerCommonInStageEntity.HadHealed
                .Subscribe(_ =>
                {
                    PlayReviveAnimation();
                }).AddTo(_playerView);

        }

        private void RegisterAnimator(CallbackAnimatorGeneratorView reviveCharacterCallbackAnimatorView)
        {
            ObjectPool<CallbackAnimatorView> reviveCharacterAnimatorPool = new ObjectPool<CallbackAnimatorView>(reviveCharacterCallbackAnimatorView);
            _playerCommonInStageEntity.SetReviveCharacterAnimatorPool(reviveCharacterAnimatorPool);
        }

        /// <summary>
        /// ゲームオーバー時、ステージ遷移時など。タスクを止めるのみ
        /// </summary>
        public void TryKillRevive()
        {
            Debug.Log($"TryKillRevive()");
            if (_reviveAnimator==null)
            {
                return;
            }
            _reviveAnimator.Pause();
            //_playerCommonInStageEntity.reviveCharacterAnimatorPool.ReleaseObject(_reviveAnimator);
            //_playerCommonUpdateableEntity.SetIsReviving(false);
        }

        /// <summary>
        /// 復活途中に同一シーン、別ステージ遷移した場合。遷移後、ステージフェードイン前に実行
        /// </summary>
        public void TrySetRevivedParameterStoppedByStageMove()
        {
            Debug.Log($"TrySetRevivedParameterStoppedByStageMove()");
            if (!_playerCommonUpdateableEntity.IsReviving)
            {
                return;
            }

            Debug.Log($"Revive");
            _playerView.gameObject.SetActive(true);
            _actionKeyView.ReInit();
            _playerCommonUpdateableEntity.SetCanDamageFlag(true);
            _playerCommonUpdateableEntity.SetCanTarget(true);
            _playerStatusView.SetSpritesByCharacterState(CharacterHpFaceSpriteType.Normal);
            if (_reviveAnimator!=null)
            {
                _reviveAnimator.ResetAnimator();
                _playerCommonInStageEntity.reviveCharacterAnimatorPool.ReleaseObject(_reviveAnimator);
            }

            _playerCommonUpdateableEntity.SetIsReviving(false);
        }

        /// <summary>
        /// 復活途中に別シーンに遷移した場合。遷移後に実行、ステージフェードイン前に実行
        /// </summary>
        public void TrySetRevivedParameterStoppedBySceneMove()
        {
            Debug.Log($"TrySetRevivedParameterStoppedBySceneMove()");
            if (!_playerCommonUpdateableEntity.IsReviving)
            {
                return;
            }
            _playerCommonUpdateableEntity.SetCanTarget(true);
            _playerCommonUpdateableEntity.SetCanDamageFlag(true);
            _playerCommonUpdateableEntity.SetIsReviving(false);
            Debug.Log($"SetIsReviving");
        }
        
        private async void PlayReviveAnimation()
        {
            _playerCommonUpdateableEntity.SetIsReviving(true);
            Vector2 headPlayerPos = GetHeadPlayerPosition();
            Debug.Log($"PlayReviveAnimation()");
            _playerView.SetPosition(headPlayerPos);
            _reviveAnimator = _playerCommonInStageEntity.reviveCharacterAnimatorPool.GetObject(
                _playerView.GetModelAppearEffectPivot(),_playerView.GetModelAppearEffectScale());
            _playerCommonUpdateableEntity.SetCanTarget(true);
            SetAnimationCallback(_reviveAnimator);

            try
            {
                await WaitAnimation(_reviveAnimator);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"Canceled Revive");
                _playerStatusView.SetSpritesByCharacterState(CharacterHpFaceSpriteType.Normal);
                _playerCommonUpdateableEntity.SetCanDamageFlag(true);
                _actionKeyView.ReInit();
                _reviveAnimator.ResetAnimator();
                //Debug.Log($"ResetAnimator");
                _playerCommonInStageEntity.reviveCharacterAnimatorPool.TryReleaseObject(_reviveAnimator);
                _reviveAnimator = null;
                _playerCommonUpdateableEntity.SetIsReviving(false);
                //Debug.Log($"SetIsReviving(false)");
                return;
            }
            Debug.Log($"アニメーション後ぱらめーた変更すたーと");
            _playerStatusView.SetSpritesByCharacterState(CharacterHpFaceSpriteType.Normal);
            _playerCommonUpdateableEntity.SetCanDamageFlag(true);
            _actionKeyView.ReInit();
            _reviveAnimator.ResetAnimator();
            _playerCommonInStageEntity.reviveCharacterAnimatorPool.ReleaseObject(_reviveAnimator);
            _reviveAnimator = null;
            //Debug.Log($"_reviveAnimator=null");
            _playerCommonUpdateableEntity.SetIsReviving(false);
        }
        
        private Vector2 GetHeadPlayerPosition()
        {
            Vector2 headPlayerPos=Vector2.negativeInfinity;
            for (int i = 1; i < Enum.GetValues(typeof(PlayableCharacter)).Length; i++)
            {
                var data = _playerCommonUpdateableEntity.GetPlayerUpdateableData((PlayableCharacter) i);
                if (!data.isInStage || data.isReviving)
                {
                    continue;
                }

                Vector2 pos=_playerCommonUpdateableEntity.GetEachTransform((PlayableCharacter) i).position;
                if (pos.x > headPlayerPos.x)
                {
                    headPlayerPos = pos;
                }
            }
            if (headPlayerPos==Vector2.negativeInfinity)
            {
                Debug.LogError($"headPlayerPos");
                return _playerCommonUpdateableEntity.GetEachTransform(_playerView.type).position;
            }
            
            return headPlayerPos;
        }

        private void SetAnimationCallback(CallbackAnimatorView reviveAnimator)
        {
            reviveAnimator.GetCallbackObserver()
                .Where(name=>name==PlayerEffectAnimationCallbackName.OnPlayer)
                .FirstOrDefault()
                .Subscribe(_ =>
                {
                    _playerView.gameObject.SetActive(true);
                    reviveAnimator.DisposeCallbackObservable();
                }).AddTo(reviveAnimator);
        }

        private async UniTask WaitAnimation(CallbackAnimatorView reviveAnimator)
        {
            reviveAnimator.SetBool(PlayerEffectAnimatorParameter.Appear,true);
            Debug.Log($"SetBool");
            if (!IsPlayingThisAnimation(reviveAnimator,PlayerEffectAnimationName.CloudOpen))
            {
                Debug.Log($"Await Animation");
                await UniTask.WaitUntil(()=>IsPlayingThisAnimation(reviveAnimator,PlayerEffectAnimationName.CloudOpen), 
                    cancellationToken: _playerView.thisToken);
            }

            Debug.Log($"WaitUntil");
            await UniTask.WaitUntil(()=>IsFinishedThisAnimation(reviveAnimator,PlayerEffectAnimationName.CloudOpen), cancellationToken: _playerView.thisToken);
            reviveAnimator.SetBool(PlayerEffectAnimatorParameter.Appear,false);
        }
        
        private bool IsPlayingThisAnimation(CallbackAnimatorView reviveAnimator,string animationName)
        {
            return reviveAnimator.GetCurrentAnimationName() == animationName;
        }
        
        private bool IsFinishedThisAnimation(CallbackAnimatorView reviveAnimator,string animationName)
        {
            AnimatorStateInfo stateInfo = reviveAnimator.GetCurrentAnimationStateInfo();
            return stateInfo.normalizedTime >= 1 && reviveAnimator.GetCurrentAnimationName() == animationName;
        }
    }
}