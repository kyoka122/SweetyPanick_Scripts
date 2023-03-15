using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyApplication;
using OutGame.PlayerCustom.View;
using OutGame.PlayerCustom.Entity;
using UniRx;
using UnityEngine;
using Utility.PanelAnimation;

namespace OutGame.PlayerCustom.Logic
{
    public class PanelLogic
    {
        //private readonly PanelsView _panelsView;
        private readonly PlayerCountView _playerCountView;
        private readonly ControllersPanelView _controllersPanelView;
        private readonly CharacterSelectPanelView _characterSelectPanelView;
        private readonly InSceneDataEntity _inSceneDataEntity;
        private readonly ConstDataEntity _constDataEntity;
        private const float NextPanelEnterDelay=1f;

        public PanelLogic(PlayerCountView playerCountView,ControllersPanelView controllersPanelView, 
            CharacterSelectPanelView characterSelectPanelView, InSceneDataEntity inSceneDataEntity,ConstDataEntity constDataEntity)
        {
            _playerCountView = playerCountView;
            _controllersPanelView = controllersPanelView;
            _characterSelectPanelView = characterSelectPanelView;
            _inSceneDataEntity = inSceneDataEntity;
            _constDataEntity = constDataEntity;
            RegisterObserver();
        }
        
        private void RegisterObserver()
        {
            _inSceneDataEntity.ChangedSettingsState
                .Subscribe(ChangePanel)
                .AddTo(_controllersPanelView);
        }

        private async void ChangePanel(PlayerCustomState state)
        {
            _inSceneDataEntity.SetIsPlayingPopAnimation(true);
            IPanelAnimation panelExitAnimation=GetPopAnimationByState(_inSceneDataEntity.PrevPlayerCustomState);
            if (panelExitAnimation!=null)
            {
                await panelExitAnimation.Exit(panelExitAnimation.Token);
                await UniTask.Delay(TimeSpan.FromSeconds(NextPanelEnterDelay),cancellationToken:panelExitAnimation.Token);
            }
            
            IPanelAnimation panelEnterAnimation=GetPopAnimationByState(_inSceneDataEntity.PlayerCustomState);
            if (panelEnterAnimation!=null)
            {
                await panelEnterAnimation.Enter(panelEnterAnimation.Token);
            }
          
            _inSceneDataEntity.SetIsPlayingPopAnimation(false);
            _inSceneDataEntity.SetHadFinishedPopUpWindow(state);
        }



        private IPanelAnimation GetPopAnimationByState(PlayerCustomState state)
        {
            switch (state)
            {
                case PlayerCustomState.None:
                    return null;
                case PlayerCustomState.PlayerCount:
                    return new PopAnimation(_playerCountView.PanelObj,_constDataEntity.PopUpDuration,_constDataEntity.PopDownDuration);
                case PlayerCustomState.Controller:
                    return new PopAnimation(_controllersPanelView.PanelObj,_constDataEntity.PopUpDuration,_constDataEntity.PopDownDuration);
                case PlayerCustomState.Character:
                    return new UIMoveAnimation(_characterSelectPanelView.UpPanelRectTransform,
                        _characterSelectPanelView.DownPanelRectTransform,
                        _constDataEntity.PopUpDuration, _constDataEntity.PopDownDuration,
                        _characterSelectPanelView.UpTargetExitEndPos, _characterSelectPanelView.DownTargetExitEndPos,
                        _characterSelectPanelView.UpTargetEnterEndPos, _characterSelectPanelView.DownTargetEnterEndPos);
                case PlayerCustomState.Finish:
                    return null;
                default:
                    Debug.LogError($"state:{state}");
                    return null;
            }
        }
    }
}