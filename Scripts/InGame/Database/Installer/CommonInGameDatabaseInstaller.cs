using System.Collections.Generic;
using System.Linq;
using InGame.Common.Database;
using InGame.Database.ScriptableData;
using InGame.Player.Controller;
using InGame.Player.Installer;
using MyApplication;
using OutGame.Database;
using StageManager;
using UnityEngine;

namespace InGame.Database.Installer
{
    public class CommonInGameDatabaseInstaller
    {
        private readonly InGameDatabase _inGameDatabase;
        private readonly OutGameDatabase _outGameDatabase;
        private readonly CommonDatabase _commonDatabase;

        public CommonInGameDatabaseInstaller(InGameDatabase inGameDatabase, OutGameDatabase outGameDatabase, CommonDatabase commonDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _outGameDatabase = outGameDatabase;
            _commonDatabase = commonDatabase;
        }

        public void SetInGameDatabase(StageUIData stageUIData, StageSettingsScriptableData stageSettingsScriptableData,
            EachStagePlayerInstanceData[] playerInstanceData,CameraInitData[] cameraInitData)
        {
            _inGameDatabase.SetUIData(stageUIData);
            _inGameDatabase.SetStageSettings(stageSettingsScriptableData);

            if (playerInstanceData!=null)
            {
                foreach (var data in playerInstanceData)
                {
                    _inGameDatabase.AddPlayerInstanceData(data);
                }
            }
            else
            {
                Debug.LogWarning($"PlayerInstanceData is null");
            }
            if (cameraInitData!=null)
            {
                foreach (var data in cameraInitData)
                {
                    _commonDatabase.AddCameraInitData(data);
                }
            }
            else
            {
                Debug.LogWarning($"CameraInitData is null");
            }
        }
        
        public void InstallAllPlayer(IManagerInitAble managerInitAble,StageArea stageArea)
        {
            CharacterCommonConstData[] allCharacterCommonData = _inGameDatabase.GetAllCharacterConstData();
            IReadOnlyList<UseCharacterData> characterData = GetCharacterData();

            //MEMO: 全キャラ生成、初期化
            for (int i = 1; i <= characterData.Count; i++)
            {
                BasePlayerController playerController = InstallAllPlayer(i,characterData, allCharacterCommonData,stageArea);
                managerInitAble.AddController(playerController);
                managerInitAble.RegisterPlayerEvent(playerController);
            }
        }

        private IReadOnlyList<UseCharacterData> GetCharacterData()
        {
            var characterData = _commonDatabase.GetUseCharacterData();

            //MEMO: Player設定シーンを飛ばした場合、仮でキーボード、Candyのデータを追加する
            if (characterData == null)
            {
                new DebugInputInstaller().Install(_commonDatabase);
            }
            characterData = _commonDatabase.GetUseCharacterData();

            return characterData;
        }

        private BasePlayerController InstallAllPlayer(int playerNum,IReadOnlyList<UseCharacterData> characterData,
            CharacterCommonConstData[] allCharacterCommonData,StageArea stageArea)
        {
            var oneCharacterData = characterData.FirstOrDefault(data => data.playerNum == playerNum);
            if (oneCharacterData == null)
            {
                Debug.LogError($"Couldn`t Find OneCharacterData");
                return null;
            }

            PlayableCharacter character =
                oneCharacterData.playableCharacter;
            var installer = allCharacterCommonData.FirstOrDefault(data => data.CharacterType == character)
                ?.Installer;
            if (installer == null)
            {
                return null;
            }

            var playerController = installer.Install(playerNum, stageArea, _inGameDatabase, _outGameDatabase, _commonDatabase);
            
            return playerController;
        }
    }
}