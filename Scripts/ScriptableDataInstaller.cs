using Audio;
using Audio.ScriptableData;
using InGame.Common.Database;
using InGame.Database;
using InGame.Database.ScriptableData;
using MyApplication;
using OutGame.Database;
using OutGame.Database.ScriptableData;
using UnityEngine;

namespace InGame
{
    [CreateAssetMenu(fileName = "ScriptableDataInstaller", menuName = "ScriptableObjects/ScriptableDataInstaller")]
    public class ScriptableDataInstaller:ScriptableObject
    {
        [SerializeField] private AudioScriptableData audioScriptableData;
        [SerializeField] private PlayerCustomSceneScriptableData playerCustomSceneScriptableData;
        [SerializeField] private PlayerScriptableData playerScriptableData;
        [SerializeField] private EnemyScriptableData enemyScriptableData;
        [SerializeField] private StageGimmickScriptableData stageGimmickScriptableData;
        [SerializeField] private StageSettingsScriptableData stageSettingsScriptableData;
        [SerializeField] private SceneLoadData sceneLoadData;
        [SerializeField] private ColateScriptableData colateScriptableData;
        [SerializeField] private BossStageScriptableData bossStageScriptableData;
        
        public void SetScriptableData(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase)
        {
            AudioVolumeManager.Init(audioScriptableData);
            inGameDatabase.SetStageSettings(stageSettingsScriptableData);
            inGameDatabase.SetEnemyData(enemyScriptableData);
            enemyScriptableData.EnemyInstaller.Init(inGameDatabase, commonDatabase);
            inGameDatabase.SetStageGimmickData(stageGimmickScriptableData);
            inGameDatabase.SetSceneLoadData(sceneLoadData);
            inGameDatabase.SetColateData(colateScriptableData);
            SetCharacterDatabase(inGameDatabase);
            outGameDatabase.SetPlayerCustomSceneData(playerCustomSceneScriptableData);
            outGameDatabase.SetBossStageScriptableData(bossStageScriptableData);
        }
        
        private void SetCharacterDatabase(InGameDatabase inGameDatabase)
        {
            var candyCommonParameter = playerScriptableData.GetCommonParameter(PlayableCharacter.Candy);
            var mashCommonParameter = playerScriptableData.GetCommonParameter(PlayableCharacter.Mash);
            var fuCommonParameter = playerScriptableData.GetCommonParameter(PlayableCharacter.Fu);
            var kureCommonParameter = playerScriptableData.GetCommonParameter(PlayableCharacter.Kure);

            var candyParameter = playerScriptableData.GetCandyParameter();
            var mashParameter = playerScriptableData.GetMashParameter();
            var fuParameter = playerScriptableData.GetFuParameter();
            var kureParameter = playerScriptableData.GetKureParameter();

            if (candyCommonParameter != null)
            {
                inGameDatabase.SetCandyStatus(new CandyStatus(candyCommonParameter,
                    playerScriptableData.GetCandyParameter(),
                    0));
                inGameDatabase.SetCommonCandyConstData(new CharacterCommonConstData(candyCommonParameter));
                inGameDatabase.SetCandyConstData(new CandyConstData(candyParameter));
            }

            if (mashCommonParameter != null)
            {
                inGameDatabase.SetMashStatus(new MashStatus(mashCommonParameter,
                    playerScriptableData.GetMashParameter()));
                inGameDatabase.SetCommonMashConstData(new CharacterCommonConstData(mashCommonParameter));
                inGameDatabase.SetMashConstData(new MashConstData(mashParameter));
            }

            if (fuCommonParameter != null)
            {
                inGameDatabase.SetFuStatus(new FuStatus(fuCommonParameter, playerScriptableData.GetFuParameter()));
                inGameDatabase.SetCommonFuConstData(new CharacterCommonConstData(fuCommonParameter));
                inGameDatabase.SetFuConstData(new FuConstData(fuParameter));
            }

            if (kureCommonParameter != null)
            {
                inGameDatabase.SetKureStatus(new KureStatus(kureCommonParameter,
                    playerScriptableData.GetKureParameter()));
                inGameDatabase.SetCommonKureConstData(new CharacterCommonConstData(kureCommonParameter));
                inGameDatabase.SetKureConstData(new KureConstData(kureParameter));
            }
        }
        
    }
}