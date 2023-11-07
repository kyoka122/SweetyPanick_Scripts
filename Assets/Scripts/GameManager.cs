using System;
using InGame.Common.Database;
using InGame.Database;
using InGame.SceneLoader;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
using SceneSequencer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Utility.SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private ScriptableDataInstaller scriptableDataInstaller;

    protected override bool IsDontDestroyOnLoad => true;

    private static InGameDatabase _inGameDatabase;
    private static OutGameDatabase _outGameDatabase;
    private static CommonDatabase _commonDatabase;

    protected override void Awake()
    {
        base.Awake();
        if (isDuplication)
        {
            Debug.Log($"isDuplication:{isDuplication}", gameObject);
            return;
        }

        Debug.LogWarning($"InstanceGameManager", gameObject);
        if (SceneManager.GetActiveScene().name!=SceneName.Title)
        {
            InstallDatabaseAndManager();
        }
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
        Debug.Log($"Loaded!");
        CheckDatabaseLife(nextScene.name);
        BaseSceneSequencer currentSceneSequencer = GetCurrentSceneSequencer();
        currentSceneSequencer.Execute(_inGameDatabase, _outGameDatabase, _commonDatabase);
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
            SceneName.ColateStage => ColateStageSequencer.Instance,
            SceneName.Epilogue => EpilogueSequencer.Instance,
            SceneName.Score => ScoreSceneSequencer.Instance,
            SceneName.DebugKyokaMoveStage => DebugMoveSceneSequencer.Instance,
            SceneName.DebugMoveStage => DebugMoveSceneSequencer.Instance,
            SceneName.DebugPrologue => PrologueSequencer.Instance,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void CheckDatabaseLife(string nextSceneName)
    {
        if (nextSceneName == SceneName.Title)
        {
            DisposeDatabase();
            InstallDatabaseAndManager();
        }
    }

    /// <summary>
    /// データベース、DontDestroyOnLoadのManagerクラスを初期化する
    /// </summary>
    private void InstallDatabaseAndManager()
    {
        _inGameDatabase = new InGameDatabase();
        _outGameDatabase = new OutGameDatabase();
        _commonDatabase = new CommonDatabase();
        scriptableDataInstaller.SetScriptableData(_inGameDatabase, _outGameDatabase, _commonDatabase);
        LoadManager.Instance.Init(_inGameDatabase, _commonDatabase);
    }

    /// <summary>
    /// データベースを完全に削除する
    /// </summary>
    private void DisposeDatabase()
    {
        Debug.LogWarning($"DisposeDatabase!", gameObject);
        _inGameDatabase?.Dispose();
        _outGameDatabase?.Dispose();
        _commonDatabase?.Dispose();
        SEManager.Instance.Stop();
        //BGMManager.Instance.Stop();
    }


    private void OnDestroy()
    {
        if (isDuplication)
        {
            return;
        }

        DisposeDatabase();
    }
}