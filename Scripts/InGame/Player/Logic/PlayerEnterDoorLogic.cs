using System.Diagnostics;
using InGame.Player.Entity;
using InGame.Stage.View;
using InGame.Player.View;
using MyApplication;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InGame.Player.Logic
{
    public class PlayerEnterDoorLogic
    {
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerInputEntity _playerInputEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly BasePlayerView _playerView;

        public PlayerEnterDoorLogic(PlayerConstEntity playerConstEntity, PlayerInputEntity playerInputEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, BasePlayerView playerView)
        {
            _playerConstEntity = playerConstEntity;
            _playerInputEntity = playerInputEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerView = playerView;
        }

        public void UpdatePlayerEnterDoor()
        {
            if (!_playerInputEntity.enterDoorFlag)
            {
                return;
            }
            if (!CanEnterDoor())
            {
                _playerInputEntity.OffEnterDoorFlag();
                return;
            }
            DoorView door = GetFacedDoor();
            if (door!=null)
            {
#if UNITY_EDITOR
                if (door.IsIgnoreKeyCheck)
                {
                    Debug.Log($"KeyIgnore. Enter");
                    door.TryEnterDoor();
                    _playerInputEntity.OffEnterDoorFlag();
                    return;
                }
#endif
                if (CanEnterDoorByKeyCheck(door.IsKeyDoor))
                {
                    Debug.Log($"TryEnterDoor");
                    door.TryEnterDoor();
                    _playerInputEntity.OffEnterDoorFlag();
                    return;
                }
            }
            _playerInputEntity.OffEnterDoorFlag();
        }

        public void Stop()
        {
            _playerInputEntity.OffEnterDoorFlag();
        }
        
        private DoorView GetFacedDoor()
        {
            //MEMO: とりあえずスイーツの距離と同値に。
            float rayDistance = _playerConstEntity.ToSweetsDistance;
            Vector2 direction = new Vector2(_playerCommonInStageEntity.playerDirection,0);
            
            RaycastHit2D raycastHit2D = Physics2D.Raycast(_playerView.GetToSweetsRayPos(), direction,
                rayDistance, LayerInfo.DoorMask);
            if (raycastHit2D.collider==null)
            {
                return null;
            }

            var door = raycastHit2D.collider.gameObject.GetComponent<DoorView>();
            
#if UNITY_EDITOR
            if (_playerView.OnDrawRay)
            {
                DrawEnterDoorRay(direction,rayDistance);
            }
#endif
            return door;
        }

        private bool CanEnterDoorByKeyCheck(bool isKeyDoor)
        {
            if (!isKeyDoor)
            {
                return true;
            }

            return _playerCommonInStageEntity.HavingKey;
        }

        private bool CanEnterDoor()
        {
            //MEMO: クレーが回復中とかはドアに入れないようにする。  
            return true;
        }
        
#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        private void DrawEnterDoorRay(Vector2 direction,float rayDistance)
        {
            Debug.DrawRay(_playerView.GetToSweetsRayPos(),direction*rayDistance,Color.yellow,0.5f);
        }
#endif
    }
}