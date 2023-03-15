﻿using System;
using InGame.Common.Database;
using InGame.Database;
using InGame.SceneLoader;
using MyApplication;
using OutGame.Database;
using SceneSequencer;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private ScriptableDataInstaller scriptableDataInstaller;

    protected override bool IsDontDestroyOnLoad => true;

    private static InGameDatabase _inGameDatabase;
    private static OutGameDatabase _outGameDatabase;
    private static CommonDatabase _commonDatabase;

    private bool _isLastSceneExecuted;

    protected override void Awake()
    {
        base.Awake();
        if (isDuplication)
        {
            Debug.Log($"isDuplication:{isDuplication}", gameObject);
            return;
        }

        Debug.LogWarning($"InstanceGameManager", gameObject);
        InstallDatabaseAndManager();
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
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
        if (nextSceneName == SceneName.Score)
        {
            _isLastSceneExecuted = true;
            return;
        }

        if (nextSceneName == SceneName.Title && _isLastSceneExecuted)
        {
            DisposeDatabase();
            InstallDatabaseAndManager();
            _isLastSceneExecuted = false;
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