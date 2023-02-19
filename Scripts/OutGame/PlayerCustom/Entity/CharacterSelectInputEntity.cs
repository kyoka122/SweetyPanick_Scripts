using System;
using OutGame.PlayerCustom.MyInput;
using UniRx;

namespace OutGame.PlayerCustom.Entity
{
    /// <summary>
    /// プレイヤーごとに生成。UI選択に使用する。
    /// </summary>
    public class CharacterSelectInputEntity:IDisposable
    {
        public IReadOnlyReactiveProperty<float> horizontalValue=>_caseUnknownControllerInput.HorizontalValue;
        public IReadOnlyReactiveProperty<float> verticalValue=>_caseUnknownControllerInput.VerticalValue;
        public IReadOnlyReactiveProperty<bool> next=>_caseUnknownControllerInput.Next;
        public IReadOnlyReactiveProperty<bool> back => _caseUnknownControllerInput.Back;
        
        private readonly BaseCaseUnknownControllerInput _caseUnknownControllerInput;
        
        public CharacterSelectInputEntity(BaseCaseUnknownControllerInput caseUnknownControllerInput)
        {
            _caseUnknownControllerInput = caseUnknownControllerInput;
        }


        public void Dispose()
        {
            _caseUnknownControllerInput?.Dispose();
        }
    }
}