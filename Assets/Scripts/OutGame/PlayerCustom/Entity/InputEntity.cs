using System;
using System.Collections.Generic;
using MyApplication;
using Common.MyInput.PlayerCustom;

namespace OutGame.PlayerCustom.Entity
{
    /// <summary>
    /// コントローラー登録前
    /// </summary>
    public class InputEntity:IDisposable
    {
        public IReadOnlyList<BasePlayerCustomInput> CustomInputs => _customInputs;
        private readonly List<BasePlayerCustomInput> _customInputs;
        
        public InputEntity()
        {
            _customInputs = InputMakeHelper.GeneratePlayerCustomInputsByAllControllers();
        }
        
        public void Dispose()
        {
            foreach (var controllerUnKnownInput in _customInputs)
            {
                controllerUnKnownInput.Dispose();
            }
        }
    }
}