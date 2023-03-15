using System;
using InGame.Common.Database;
using InGame.Database;
using Common.MyInput.Player;
using OutGame.Database;
using UniRx;
using UnityEngine;

namespace InGame.Player.Entity
{
    public class PlayerInputEntity:IDisposable
    {
        public float xMoveValue { get; private set; }
        public bool jumpFlag { get; private set; }
        public bool punchFlag{ get; private set; }
        public bool skillFlag{ get; private set; }
        public bool fixFlag{ get; private set; }
        public bool enterDoorFlag{ get; private set; }

        public Action rumble => _playerInput.rumbleEvent;

        public bool IsOnPlayerSelector => OnPlayerSelector.Value;
        
        //MEMO: ↓UI系
        public IReadOnlyReactiveProperty<bool> OnPlayerSelector => _playerInput.PlayerSelector;
        public IReadOnlyReactiveProperty<int> PlayerSelectorMoveDirection => _playerInput.PlayerSelectDirection;


        private readonly BasePlayerInput _playerInput;
        
        public PlayerInputEntity(int playerNum,InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            var data=commonDatabase.GetControllerData(playerNum);
            if (data==null)
            {
                Debug.LogError($"Not Found PlayerData (playerNum:{playerNum})");
            }
            _playerInput = data;
            
            RegisterReactiveProperty();
        }

        private void RegisterReactiveProperty()
        {
            _playerInput.Move
                .Subscribe(value =>
                {
                    xMoveValue = value;
                });
            
            _playerInput.Jump
                .Where(jump => jump)
                .Subscribe(_ =>
                {
                    jumpFlag = true;
                });
            
            _playerInput.Punch
                .Where(punch => punch)
                .Subscribe(_ =>
                {
                    punchFlag = true;
                    enterDoorFlag = true;
                });
            
            _playerInput.Skill
                .Where(skill => skill)
                .Subscribe(_ =>
                {
                    skillFlag = true;
                });
            
            _playerInput.Fix
                .Subscribe(onFix =>
                {
                    fixFlag = onFix;
                });
        }

        public void OffJumpFlag()
        {
            jumpFlag = false;
        }
        
        public void OffPunchFlag()
        {
            punchFlag = false;
        }
        
        public void OffSkillFlag()
        {
            skillFlag=false;
        }
        
        public void OffFixFlag()
        {
            fixFlag=false;
        }
        
        public void OffEnterDoorFlag()
        {
            enterDoorFlag=false;
        }

        public void Dispose()
        {
            _playerInput?.Dispose();
        }
    }
}