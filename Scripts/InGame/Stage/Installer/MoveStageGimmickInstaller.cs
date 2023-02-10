using System;
using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Database;
using InGame.Stage.Logic;
using InGame.Stage.Manager;
using InGame.Stage.View;
using InGame.Player.View;
using InGame.Stage.Entity;
using MyApplication;
using UnityEngine;

namespace InGame.Stage.Installer
{
    public class MoveStageGimmickInstaller:MonoBehaviour
    {
        [SerializeField] private HealAnimationView healAnimationView;
        [SerializeField] private StageObjectsView stageObjectsView;
        [SerializeField] private BackgroundView backgroundView;
        
        public MoveStageGimmickManager Install(Action<StageEvent> stageEvent,InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            var healAnimationLogic = new HealAnimationLogic(healAnimationView);
            var doorLogic = new DoorLogic(stageObjectsView,stageEvent);
            var stageGimmickEntity = new StageGimmickEntity(inGameDatabase);
            var stageBaseEntity = new StageBaseEntity(inGameDatabase,commonDatabase);
            backgroundView.Init();
            
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

            var moveFloorViews = FindObjectsOfType<MoveFloorView>();
            List<MoveFloorLogic> moveFloorLogics = new List<MoveFloorLogic>();
            foreach (var moveFloorView in moveFloorViews)
            {
                moveFloorView.Init();
                moveFloorLogics.Add(new MoveFloorLogic(moveFloorView,stageGimmickEntity));
            }

            BackgroundLogic backgroundLogic = new BackgroundLogic(stageBaseEntity,backgroundView);

            return new MoveStageGimmickManager(healAnimationLogic, doorLogic,moveFloorLogics.ToArray(),backgroundLogic);
        }
    }
}