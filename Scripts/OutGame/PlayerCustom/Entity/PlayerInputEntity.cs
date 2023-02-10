using OutGame.MyInput;
using UniRx;
using UnityEngine;

namespace OutGame.PlayerCustom.Entity
{
    /// <summary>
    /// プレイヤーごとに生成。UI選択に使用する。
    /// </summary>
    public class PlayerInputEntity
    {
        public IReadOnlyReactiveProperty<float> horizontalValue=>_playerInput.HorizontalValue;
        public IReadOnlyReactiveProperty<float> verticalValue=>_playerInput.VerticalValue;
        public IReadOnlyReactiveProperty<bool> next=>_playerInput.Next;
        public IReadOnlyReactiveProperty<bool> back => _playerInput.Back;
        public IReadOnlyReactiveProperty<bool> backPrevMenu => _playerInput.BackPrevMenu;
        
        private readonly BasePlayerInput _playerInput;
        
        public PlayerInputEntity(int joyconRightIndex,int joyconLeftIndex)
        {
            //MEMO: ↓はデバッグ（PC）用//////////////////////////////////////////////////
            //_playerInput = new DebugPlayerInput();
            //////////////////////////////////////////////////////////////////////
            
            //Debug.Log($"playerNum - 1) * 2:{(joyconLeftIndex - 1) * 2}");
            //Debug.Log($"JoyconManager.Instance.j:{JoyconManager.Instance.j}");
            _playerInput = new JoyConPlayerInput(JoyconManager.Instance.j[joyconRightIndex],
                JoyconManager.Instance.j[joyconLeftIndex]);
        }
        
        
    }
}