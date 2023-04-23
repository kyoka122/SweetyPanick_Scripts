using System;
using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Common.Interface;
using InGame.Database;
using InGame.Stage.Logic;
using InGame.Stage.Manager;
using InGame.Stage.View;
using InGame.Player.View;
using InGame.Stage.Entity;
using MyApplication;
using UnityEngine;
using Utility;

namespace InGame.Stage.Installer
{
    public class MoveStageGimmickInstaller:MonoBehaviour
    {
        [SerializeField] private HealAnimationView healAnimationView;
        [SerializeField] private StageObjectsView stageObjectsView;
        [SerializeField] private PopTextGeneratorView popTextGeneratorView;
        

        public MoveStageGimmickManager Install(Action<StageEvent> stageEvent,StageArea stageArea,
            InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            var healAnimationLogic = new HealAnimationLogic(healAnimationView);
            var doorLogic = new DoorLogic(stageObjectsView,stageEvent);
            var stageGimmickEntity = new StageGimmickEntity(inGameDatabase,commonDatabase);
            var stageBaseEntity = new StageBaseEntity(inGameDatabase,commonDatabase);
            var scoreEntity = new ScoreEntity(inGameDatabase,stageArea);

            var scoreView = FindObjectOfType<ScoreView>();
            scoreView.Init(inGameDatabase.GetAllStageData().score);
            foreach (var backgroundView in FindObjectsOfType<BackgroundView>())
            {
                backgroundView.Init();
            }
            
            var defaultSweetsView = FindObjectsOfType<DefaultSweetsView>();
            foreach (var sweetsView in defaultSweetsView)
            {
                sweetsView.Init();
            }
            
            var gumViews = FindObjectsOfType<GumView>();
            foreach (var gumView in gumViews)
            {
                gumView.Init();
            }
            
            var defaultGimmickSweetsViews = FindObjectsOfType<DefaultGimmickSweetsView>();
            foreach (var defaultGimmickSweetsView in defaultGimmickSweetsViews)
            {
                defaultGimmickSweetsView.Init();
            }
            
            var gumGimmickViews = FindObjectsOfType<GumGimmickView>();
            foreach (var gumGimmickView in gumGimmickViews)
            {
                gumGimmickView.Init();
            }
            
            var candyLightsGimmickViews = FindObjectsOfType<CandyLightsGimmickView>();

            var moveFloorViews = FindObjectsOfType<MoveFloorView>();
            var moveFloorLogics = new List<MoveFloorLogic>();
            foreach (var moveFloorView in moveFloorViews)
            {
                moveFloorView.Init();
                moveFloorLogics.Add(new MoveFloorLogic(moveFloorView,stageGimmickEntity));
            }
            var candyLightsLogic = new CandyLightsLogic(stageGimmickEntity,candyLightsGimmickViews);

            var backgroundLogic = new BackgroundLogic(stageBaseEntity);
            var animationEventLogic = new AnimationEventLogic(GameObjectExtensions
                .FindObjectsOfInterface<IAnimationCallbackSender>(),stageGimmickEntity);

            List<ISweets> sweets = new List<ISweets>();
            sweets.AddRange(defaultSweetsView);
            sweets.AddRange(gumViews);
            sweets.AddRange(defaultGimmickSweetsViews);
            sweets.AddRange(gumGimmickViews);
            SweetsScoreLogic scoreLogic = new SweetsScoreLogic(scoreEntity,sweets.ToArray(),scoreView,popTextGeneratorView);
            
            List<IDisposable> disposables = new List<IDisposable>{scoreLogic};
            
            return new MoveStageGimmickManager(healAnimationLogic, doorLogic,moveFloorLogics.ToArray(),backgroundLogic,
                animationEventLogic,candyLightsLogic,disposables);
        }
    }
}