using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InGame.Player.Entity;
using InGame.Player.View;
using MyApplication;
using OutGame.Database;
using UniRx;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class PlayerTalkLogic
    {
        private readonly PlayerTalkEntity _playerTalkEntity;
        private readonly PlayerAnimatorView _playerAnimatorView;
        private readonly BasePlayerView _playerView;
        private readonly PlayerStatusView _playerStatusView;
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;

        //private float princessEnterTime=3;

        public PlayerTalkLogic(PlayerTalkEntity playerTalkEntity,PlayerCommonUpdateableEntity playerCommonUpdateableEntity,
        PlayerAnimatorView playerAnimatorView, BasePlayerView playerView,PlayerStatusView playerStatusView)
        {
            _playerTalkEntity = playerTalkEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerAnimatorView = playerAnimatorView;
            _playerView = playerView;
            _playerStatusView = playerStatusView;
            RegisterObserver();
        }

        public void StartTalk()
        {
            _playerAnimatorView.SetAnimatorSpeed(0);
            _playerStatusView.SetActive(false);
        }
        
        public void FinishTalk()
        {
            _playerAnimatorView.SetAnimatorSpeed(1);
            if (_playerCommonUpdateableEntity.IsUsed)
            {
                _playerStatusView.SetActive(true);
            }
        }
        
        private void RegisterObserver()
        {
            _playerTalkEntity.TalkPartPlayerMoveData
                .Where(data=>data!=null)
                .Subscribe(MovePrincessTask)
                .AddTo(_playerView);
        }

        private async void MovePrincessTask(TalkPartPlayerMoveData data)
        {
            //MEMO: 移動処理
            CancellationToken token = _playerView.thisToken;
            _playerAnimatorView.SetAnimatorSpeed(1);
            OnRunAnimation();
            var moveVec = new Vector2(data.MoveXVec, 0);
            await Move(moveVec, data.MoveTime, token);

            //MEMO: 移動終了処理
            OffRunAnimation();
            _playerAnimatorView.SetAnimatorSpeed(0);
            
            //TODO: 終了通知
            _playerTalkEntity.OnFinishedTalkPartAction(data.ActionType);
        }

        private async UniTask Move(Vector2 moveVec,float exitTime,CancellationToken token)
        {
            Vector2 destination = (Vector2) _playerView.GetTransform().position + moveVec;

            await _playerView.GetTransform().DOMove(destination, exitTime)
                .ToUniTask(cancellationToken: token);
        }

        private void OffRunAnimation()
        {
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.IsHorizontalMove,false);
            _playerAnimatorView.PlayFloatAnimation(PlayerAnimatorParameter.HorizontalMove, 0);
        }

        private void OnRunAnimation()
        {
            _playerAnimatorView.PlayBoolAnimation(PlayerAnimatorParameter.IsHorizontalMove,true);
            _playerAnimatorView.PlayFloatAnimation(PlayerAnimatorParameter.HorizontalMove, 1);
        }
    }
}