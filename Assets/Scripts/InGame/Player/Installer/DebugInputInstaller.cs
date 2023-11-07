using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Database;
using Common.MyInput.Player;
using MyApplication;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace InGame.Player.Installer
{
    /// <summary>
    /// デバッグ時、入力はキーボード、キャラクターはキャンディを登録する
    /// </summary>
    public class DebugInputInstaller
    {
        public void Install(CommonDatabase commonDatabase,PlayableCharacter debugCharacter)
        {
            var inputDevices = InputSystem.devices.ToArray();
            foreach (var inputDevice in inputDevices)
            {
                if (inputDevice.IsTDevice<Keyboard>())
                {
                    Debug.Log($"{inputDevice.name}Set.");
                    var inputData = new List<ControllerNumData> {new(1,new AnyDevicePlayerInput(inputDevice))};
                    commonDatabase.SetAllControllerData(inputData);
                }
            }
            var characterDataList = new List<UseCharacterData> {new(1, debugCharacter)};
            commonDatabase.SetUseCharacterData(characterDataList);
        }
    }
}