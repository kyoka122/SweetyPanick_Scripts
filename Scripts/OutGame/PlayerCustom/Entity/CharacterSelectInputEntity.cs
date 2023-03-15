using System;
using Common.MyInput.PlayerCustom;
using UniRx;

namespace OutGame.PlayerCustom.Entity
{
    /// <summary>
    /// プレイヤーごとに生成。UI選択に使用する。
    /// </summary>
    public class CharacterSelectInputEntity:IDisposable
    {
        public IReadOnlyReactiveProperty<float> horizontalValue=>_playerCustomInput.HorizontalValue;
        public IReadOnlyReactiveProperty<float> verticalValue=>_playerCustomInput.VerticalValue;
        public IReadOnlyReactiveProperty<bool> next=>_playerCustomInput.Next;
        public IReadOnlyReactiveProperty<bool> back => _playerCustomInput.Back;
        
        private readonly BasePlayerCustomInput _playerCustomInput;
        
        public CharacterSelectInputEntity(BasePlayerCustomInput playerCustomInput)
        {
            _playerCustomInput = playerCustomInput;
        }


        public void Dispose()
        {
            _playerCustomInput?.Dispose();
        }
    }
}