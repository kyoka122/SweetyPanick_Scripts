using InGame.Player.Entity;
using InGame.Player.View;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class PlayerReShapeLogic
    {
        private readonly Quaternion _rightMoveRot = Quaternion.Euler(0, 180, 0);
        private readonly Quaternion _leftMoveRot = Quaternion.Euler(0, 0, 0);
        private Vector3 InvertedModelScale=>new(-_defaultModelScale.x,_defaultModelScale.y);
        private Vector3 InvertedScale=>new(-_defaultScale.x,_defaultScale.y);

        private readonly BasePlayerView _playerView;
        private readonly PlayerInputEntity _playerInputEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly Vector3 _defaultModelScale;
        private readonly Vector3 _defaultScale;

        private bool _isCurrentTowardsRight;

        public PlayerReShapeLogic(BasePlayerView playerView,PlayerInputEntity playerInputEntity,PlayerCommonInStageEntity playerCommonInStageEntity)
        {
            _playerView = playerView;
            _playerInputEntity = playerInputEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            
            _defaultScale = _playerView.transform.localScale;
            _defaultModelScale =_playerView.GetModelTransform().localScale;
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
    }
}