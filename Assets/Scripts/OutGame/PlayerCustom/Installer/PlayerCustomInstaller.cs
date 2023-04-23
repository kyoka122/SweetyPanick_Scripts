using System;
using System.Collections.Generic;
using InGame.Common.Database;
using InGame.Player.View;
using OutGame.Database;
using OutGame.PlayerCustom.View;
using OutGame.PlayerCustom.Entity;
using OutGame.PlayerCustom.Logic;
using OutGame.PlayerCustom.Manager;
using TalkSystem;
using UniRx;
using UnityEngine;

namespace OutGame.PlayerCustom.Installer
{
    public class PlayerCustomInstaller
    {
        public PlayerCustomManager Install(PlayerCountView playerCountView, ControllersPanelView controllersPanelView,
            CharacterSelectPanelView characterSelectPanelView,
            FromMessageWindowRecieverView fromMessageWindowRecieverView,
            ToMessageWindowSenderView toMessageWindowSenderView, Dialogs playerCountDialog, Dialogs controllerDialog,
            Dialogs characterDialog, Dialogs cheerDialog, OutGameDatabase outGameDatabase,
            CommonDatabase commonDatabase,
            Subject<bool> moveNextScene)
        {
            playerCountView.Init();
            controllersPanelView.Init();
            characterSelectPanelView.Init();
            var inputCaseUnknownControllerEntity = new InputEntity();
            var inSceneDataEntity = new InSceneDataEntity(outGameDatabase, commonDatabase);
            var constDataEntity = new ConstDataEntity(outGameDatabase, commonDatabase);

            playerCountDialog.Init(outGameDatabase.GetDialogFaceSpriteScriptableData());
            controllerDialog.Init(outGameDatabase.GetDialogFaceSpriteScriptableData());
            characterDialog.Init(outGameDatabase.GetDialogFaceSpriteScriptableData());
            cheerDialog.Init(outGameDatabase.GetDialogFaceSpriteScriptableData());
            
            fromMessageWindowRecieverView.Init(moveNextScene);
            toMessageWindowSenderView.Init(playerCountDialog,controllerDialog,characterDialog,cheerDialog);
            PlayerCountButtonLogic playerCountButtonLogic = new PlayerCountButtonLogic(inputCaseUnknownControllerEntity,
                inSceneDataEntity, playerCountView, toMessageWindowSenderView);
            ControllerLogic controllerLogic = new ControllerLogic(inputCaseUnknownControllerEntity,
                inSceneDataEntity, controllersPanelView, toMessageWindowSenderView);
            CharacterSelectLogic characterSelectLogic = new CharacterSelectLogic(inSceneDataEntity, constDataEntity,
                toMessageWindowSenderView, characterSelectPanelView);
            PanelLogic panelLogic = new PanelLogic(playerCountView, controllersPanelView, characterSelectPanelView,
                inSceneDataEntity, constDataEntity);
            FinishTalkLogic finishTalkLogic = new FinishTalkLogic(inSceneDataEntity, toMessageWindowSenderView);

            var disposables = new IDisposable[] {inputCaseUnknownControllerEntity, characterSelectLogic};

            return new PlayerCustomManager(panelLogic, playerCountButtonLogic, controllerLogic, characterSelectLogic,
                finishTalkLogic, disposables);
        }

    }
}