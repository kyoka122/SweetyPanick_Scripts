﻿using InGame.Player.Entity;
using InGame.Player.View;
using UnityEngine;

namespace InGame.Player.Logic
{
    /// <summary>
    /// プレイヤーの移動方向によってオブジェクトの向きを変更する。
    /// 単純にrotationを反転するだけではアニメーターで不具合が起こるため、scaleを反転する。
    /// scaleを反転するとrigidbodyも反転するため、Rigidbodyも反転する。
    /// </summary>
    public class PlayerReShapeLogic
    {
        private readonly Quaternion _rightMoveRot = Quaternion.Euler(0, 180, 0);
        private readonly Quaternion _leftMoveRot = Quaternion.Euler(0, 0, 0);
        private Vector3 InvertedModelScale=>new(-_defaultModelScale.x,_defaultModelScale.y);
        private Vector3 InvertedScale=>new(-_defaultScale.x,_defaultScale.y);

        private readonly BasePlayerView _playerView;
        private readonly ActionKeyView _actionKeyView;
        private readonly PlayerInputEntity _playerInputEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly Vector3 _defaultModelScale;
        private readonly Vector3 _defaultScale;

        private bool _isCurrentTowardsRight;

        public PlayerReShapeLogic(BasePlayerView playerView,ActionKeyView actionKeyView,PlayerInputEntity playerInputEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity)
        {
            _playerView = playerView;
            _actionKeyView = actionKeyView;
            _playerInputEntity = playerInputEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _defaultScale = _playerView.transform.localScale;
            _defaultModelScale =_playerView.GetModelTransform().localScale;
            _isCurrentTowardsRight = false;
        }

        public void LateInit()
        {
            UpdatePlayerDirection();
        }

        public void UpdatePlayerDirection()
        {
            float xMoveValue=_playerCommonInStageEntity.playerDirection;
            if (xMoveValue==0)
            {
                return;
            }
            bool isTowardsRight = xMoveValue > 0;
            if (isTowardsRight!=_isCurrentTowardsRight)
            {
                _isCurrentTowardsRight = isTowardsRight;
                InvertModel(xMoveValue);
                InvertRigidbody(xMoveValue);
                InvertActionKey(xMoveValue);
            }

        }
        
        private void InvertModel(float inputValue)
        {
            Quaternion newRot = inputValue > 0 ? _rightMoveRot : _leftMoveRot;
            Vector3 newModelScale = inputValue > 0 ? _defaultModelScale : InvertedModelScale;
            _playerView.SetModelRotation(newRot);
            _playerView.SetModelScale(newModelScale);
        }
        
        private void InvertRigidbody(float  inputDirection)
        {
            Vector3 newScale = inputDirection > 0 ? _defaultScale : InvertedScale;
            _playerView.SetScale(newScale);
        }
        
        private void InvertActionKey(float inputValue)
        {
            Quaternion newRot = inputValue > 0 ? _rightMoveRot : _leftMoveRot;
            _actionKeyView.SetRotation(newRot);
        }
    }
}