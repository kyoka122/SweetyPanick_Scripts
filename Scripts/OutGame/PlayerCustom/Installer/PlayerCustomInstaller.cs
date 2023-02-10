using InGame.Common.Database;
using OutGame.Database;
using OutGame.PlayerCustom.View;
using OutGame.PlayerCustom.Entity;
using OutGame.PlayerCustom.Logic;
using OutGame.PlayerCustom.Manager;
using UniRx;

namespace OutGame.PlayerCustom.Installer
{
    public class PlayerCustomInstaller
    {
        public PlayerCustomManager Install(PlayerCountView playerCountView, ControllersPanelView controllersPanelView, 
            CharacterSelectPanelView characterSelectPanelView,FromMessageWindowRecieverView fromMessageWindowRecieverView,
            ToMessageWindowSenderView toMessageWindowSenderView, OutGameDatabase outGameDatabase,CommonDatabase commonDatabase,
            Subject<bool> moveNextScene)
        {
            InputCaseUnknownControllerEntity inputCaseUnknownControllerEntity=new InputCaseUnknownControllerEntity();
            InSceneDataEntity inSceneDataEntity = new InSceneDataEntity(outGameDatabase,commonDatabase);
            ConstDataEntity constDataEntity = new ConstDataEntity(outGameDatabase,commonDatabase);
            
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
            return new PlayerCustomManager(panelLogic,playerCountButtonLogic,playerControllerLogic,characterSelectLogic,confirmLogic);
        }
    }
}