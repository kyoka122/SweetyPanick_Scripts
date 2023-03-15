using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyApplication;
using Unity.VisualScripting;
using UnityEngine;

namespace OutGame.PlayerTalks
{
    public class PlayerAnimations : MonoBehaviour
    {
        [SerializeField] private Transform modelTransform;
        
        private readonly Quaternion _rightMoveRot = Quaternion.Euler(0, 180, 0);
        private readonly Quaternion _leftMoveRot = Quaternion.Euler(0, 0, 0);
        private Vector3 InvertedModelScale => new(-_defaultModelScale.x, _defaultModelScale.y);
        private Vector3 InvertedScale => new(-_defaultScale.x, _defaultScale.y);

        private Vector3 _defaultModelScale;
        private Vector3 _defaultScale;
        private Animator _animator;

        private int _currentDirection;

        public void Init(Transform modelTransform)
        {
            this.modelTransform = modelTransform;
            Init();
        }
        
        public void Init()
        {
            _animator = GetComponent<Animator>();
            _defaultScale = transform.localScale;
        }

        public void UpdatePlayerDirection(int direction)
        {
            _currentDirection = direction;

            if (_currentDirection == 0)
            {
                return;
            }
            
            InvertModel();
        }

        private void InvertModel()
        {
            Quaternion newRot = _currentDirection > 0 ? _rightMoveRot : _leftMoveRot;
            Vector3 newModelScale = _currentDirection > 0 ? _defaultModelScale : InvertedModelScale;
            SetModelRotation(newRot);
        }

        private void SetModelRotation(Quaternion newQuaternion)
        {
            transform.rotation = newQuaternion;
        }

        public void OnRunAnimation()
        {
            _animator.SetBool(PlayerAnimatorParameter.IsHorizontalMove,true);
            _animator.SetFloat(PlayerAnimatorParameter.HorizontalMove, 1);
        }

        public async UniTask Move(Camera camera,Vector2 outOfScreenViewPos,float exitTime,CancellationToken token)
        {
            var exitWorldPoint=camera.ViewportToWorldPoint(outOfScreenViewPos);
            exitWorldPoint = new Vector2(exitWorldPoint.x, transform.position.y);

            await transform.DOMove(exitWorldPoint, exitTime)
                .ToUniTask(cancellationToken: token);
        }
    }
}