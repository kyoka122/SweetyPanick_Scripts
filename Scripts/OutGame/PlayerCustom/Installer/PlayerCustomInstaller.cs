using System;
using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Player.View;
using OutGame.Database;
using OutGame.PlayerCustom.View;
using OutGame.PlayerCustom.Entity;
using OutGame.PlayerCustom.Logic;
using OutGame.PlayerCustom.Manager;
using UniRx;
using UnityEngine;

namespace OutGame.PlayerCustom.Installer
{
    public class PlayerCustomInstaller
    {
        public PlayerCustomManager Install(PlayerCountView playerCountView, ControllersPanelView controllersPanelView, 
            CharacterSelectPanelView characterSelectPanelView,FromMessageWindowRecieverView fromMessageWindowRecieverView,
            ToMessageWindowSenderView toMessageWindowSenderView, OutGameDatabase outGameDatabase,CommonDatabase commonDatabase,
            Subject<bool> moveNextScene)
        {
            playerCountView.Init();
            controllersPanelView.Init();
            var inputCaseUnknownControllerEntity=new InputCaseUnknownControllerEntity();
            var inSceneDataEntity = new InSceneDataEntity(outGameDatabase,commonDatabase);
            var constDataEntity = new ConstDataEntity(outGameDatabase,commonDatabase);
            
            fromMessageWindowRecieverView.Init(moveNextScene);
            
            PlayerCountButtonLogic playerCountButtonLogic = new PlayerCountButtonLogic(inputCaseUnknownControllerEntity,
                inSceneDataEntity, playerCountView,toMessageWindowSenderView);
            PlayerControllerLogic playerControllerLogic = new PlayerControllerLogic(inputCaseUnknownControllerEntity,
                inSceneDataEntity, controllersPanelView,toMessageWindowSenderView);
            CharacterSelectLogic characterSelectLogic = new CharacterSelectLogic(inSceneDataEntity, constDataEntity,
                toMessageWindowSenderView, characterSelectPanelView);
            PanelLogic panelLogic = new PanelLogic(playerCountView, controllersPanelView, characterSelectPanelView,
                inSceneDataEntity, constDataEntity);
            ConfirmLogic confirmLogic = new ConfirmLogic(inSceneDataEntity,toMessageWindowSenderView);

            var disposables=new IDisposable[]{inputCaseUnknownControllerEntity, characterSelectLogic};

            return new PlayerCustomManager(panelLogic,playerCountButtonLogic,playerControllerLogic,characterSelectLogic,confirmLogic,disposables);
        }

    }
}