using System;
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
        
        public void InstallAllPlayer(IStageManager stageManager,StageArea stageArea)
        {
            CharacterCommonConstData[] allCharacterCommonData = _inGameDatabase.GetAllCharacterConstData();
            IReadOnlyList<UseCharacterData> characterData = GetCharacterData();
            int playerDummyNum = characterData.Count + 1;
            //全キャラ生成、初期化
            for (int i = 1; i < Enum.GetValues(typeof(PlayableCharacter)).Length; i++)
            {
                PlayableCharacter type = (PlayableCharacter) i;
                PlayerUpdateableData playerUpdateableData = _inGameDatabase.GetPlayerUpdateableData(type);
                var selectCharacterData = characterData.FirstOrDefault(data => data.type == type);
                
                if (playerUpdateableData==null)
                {
                    if (selectCharacterData==null)
                    {
                        SetPlayerUpdateableData(type,playerDummyNum);
                        playerDummyNum++;
                    }
                    else
                    {
                        SetPlayerUpdateableData(type,selectCharacterData);
                    }
                    playerUpdateableData = _inGameDatabase.GetPlayerUpdateableData(type);
                }
                BasePlayerController playerController=InstallPlayer(type,playerUpdateableData, allCharacterCommonData, 
                    stageArea);
                stageManager.AddController(playerController);
                stageManager.RegisterPlayerEvent(playerController);
            }
            stageManager.RegisterGameOverObserver();
        }

        /// <summary>
        /// UseCharacterDataが存在していた場合、使用キャラクターとして登録
        /// </summary>
        /// <param name="type"></param>
        /// <param name="characterData"></param>
        private void SetPlayerUpdateableData(PlayableCharacter type,UseCharacterData characterData)
        {
            PlayerUpdateableData playerUpdateableData=new PlayerUpdateableData(characterData.playerNum,
                _inGameDatabase.GetCharacterCommonStatus(type).MaxHp,true,false);
            _inGameDatabase.SetPlayerUpdateableData(type, playerUpdateableData);
        }
        
        /// <summary>
        /// UseCharacterDataが存在していなかった場合、控えキャラクターとして登録
        /// </summary>
        /// <param name="type"></param>
        /// <param name="playerDummyNum"></param>
        private void SetPlayerUpdateableData(PlayableCharacter type,int playerDummyNum)
        {
            PlayerUpdateableData playerUpdateableData=new PlayerUpdateableData(playerDummyNum,
                _inGameDatabase.GetCharacterCommonStatus(type).MaxHp,false,false);
            _inGameDatabase.SetPlayerUpdateableData(type, playerUpdateableData);
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

        private BasePlayerController InstallPlayer(PlayableCharacter type,PlayerUpdateableData playerUpdateableData,
            CharacterCommonConstData[] allCharacterCommonData,StageArea stageArea)
        {
            var installer = allCharacterCommonData.FirstOrDefault(data => data.CharacterType == type)
                ?.Installer;
            if (installer == null)
            {
                return null;
            }

            var playerController = installer.Install(playerUpdateableData.playerNum, stageArea, _inGameDatabase,
                _outGameDatabase, _commonDatabase);
            
            return playerController;
        }
    }
}