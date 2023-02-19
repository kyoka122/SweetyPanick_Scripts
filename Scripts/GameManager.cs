using System;
using InGame.Common.Database;
using InGame.Database;
using InGame.SceneLoader;
using MyApplication;
using OutGame.Database;
using SceneSequencer;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace InGame
{
    public class GameManager: SingletonMonoBehaviour<GameManager>
    {
        protected override bool IsDontDestroyOnLoad=>true;
        
        private static InGameDatabase _inGameDatabase;
        private static OutGameDatabase _outGameDatabase;
        private static CommonDatabase _commonDatabase;

        [SerializeField] private ScriptableDataInstaller scriptableDataInstaller;
                
        protected override void Awake()
        {
            base.Awake();
            if (isDuplication)
            {
                return;
            }
            _inGameDatabase = new InGameDatabase();
            _outGameDatabase = new OutGameDatabase();
            _commonDatabase = new CommonDatabase();
            scriptableDataInstaller.SetScriptableData(_inGameDatabase, _outGameDatabase, _commonDatabase);
            LoadManager.Instance.Init(_inGameDatabase,_commonDatabase);
            SceneManager.sceneLoaded += SceneLoaded;
            
            //MEMO: 初回シーンでもコールバックは呼ばれるので↓はいらない？
            //BaseSceneSequencer currentSceneSequencer = GetCurrentSceneSequencer();
            //currentSceneSequencer.Execute(_inGameDatabase,_outGameDatabase,_commonDatabase);
        }

        private void SceneLoaded(Scene nextScene, LoadSceneMode mode)
        {
            BaseSceneSequencer currentSceneSequencer = GetCurrentSceneSequencer();
            currentSceneSequencer.Execute(_inGameDatabase,_outGameDatabase,_commonDatabase);
        }

        private BaseSceneSequencer GetCurrentSceneSequencer()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            return currentSceneName switch
            {
                //MEMO: コメントアウト箇所はSequencer作成後に解除
                SceneName.Title => TitleSequencer.Instance,
                SceneName.PlayerCustom => PlayerCustomSceneSequencer.Instance,
                SceneName.Prologue => PrologueSequencer.Instance,
                SceneName.FirstStage => FirstStageSequencer.Instance,
                SceneName.SecondStage => SecondStageSequencer.Instance,
                //SceneName.ColateStage => ,
                //SceneName.Epilogue => ,
                SceneName.DebugKyokaMoveStage => DebugMoveSceneSequencer.Instance,
                SceneName.DebugMoveStage => DebugMoveSceneSequencer.Instance,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void OnDestroy()
        {
            _inGameDatabase.Dispose();
            _outGameDatabase.Dispose();
        }
    }
}