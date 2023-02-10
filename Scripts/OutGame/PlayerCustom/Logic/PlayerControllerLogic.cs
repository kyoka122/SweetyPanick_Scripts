using System.Collections.Generic;
using System.Linq;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.PlayerCustom.View;
using OutGame.PlayerCustom.Entity;
using UniRx;

namespace OutGame.PlayerCustom.Logic
{
    public class PlayerControllerLogic
    {
        private readonly InputCaseUnknownControllerEntity _inputCaseUnknownControllerEntity;
        private readonly InSceneDataEntity _inSceneDataEntity;
        private readonly ControllersPanelView _controllersPanelView;
        private readonly ToMessageWindowSenderView _toMessageWindowSenderView;

        public PlayerControllerLogic(InputCaseUnknownControllerEntity inputCaseUnknownControllerEntity,
            InSceneDataEntity inSceneDataEntity, ControllersPanelView controllersPanelView,
            ToMessageWindowSenderView toMessageWindowSenderView)
        {
            _inputCaseUnknownControllerEntity = inputCaseUnknownControllerEntity;
            _inSceneDataEntity = inSceneDataEntity;
            _controllersPanelView = controllersPanelView;
            _toMessageWindowSenderView = toMessageWindowSenderView;
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            _inSceneDataEntity.changedSettingsState
                .Where(state => state == PlayerCustomState.Controller)
                .Subscribe(_ =>
                {
                    ResetRegisteredControllers();
                })
                .AddTo(_controllersPanelView);
            
            _inSceneDataEntity.hadFinishedPopUpWindow
                .Where(state => state == PlayerCustomState.Controller)
                .Subscribe(_ =>
                {
                    _toMessageWindowSenderView.SendControllerSettingsEvent();
                })
                .AddTo(_controllersPanelView);
            
            
            foreach (var leftInput in _inputCaseUnknownControllerEntity.joyconLeftInputs)
            {
                leftInput.LeftControllerSet
                    .Where(on=>on)
                    .Where(_=>_inSceneDataEntity.finishedPopUpWindowState==PlayerCustomState.Controller)
                    .Subscribe(_=>OnClickRegisterButton(leftInput.joyconLeft))
                    .AddTo(_controllersPanelView);
                
                leftInput.BackPrevMenu
                    .Where(on=>on)
                    .Where(_=>_inSceneDataEntity.finishedPopUpWindowState==PlayerCustomState.Controller)
                    .Subscribe(_=>_inSceneDataEntity.SetSettingsState(PlayerCustomState.PlayerCount))
                    .AddTo(_controllersPanelView);
            }

            foreach (var rightInput in _inputCaseUnknownControllerEntity.joyconRightInputs)
            {
                rightInput.RightControllerSet
                    .Where(on=>on)
                    .Where(_=>_inSceneDataEntity.finishedPopUpWindowState==PlayerCustomState.Controller)
                    .Subscribe(_=>OnClickRegisterButton(rightInput._joyconRight))
                    .AddTo(_controllersPanelView);
            }
        }

        private void ResetRegisteredControllers()
        {
            _controllersPanelView.ResetControllerImages(_inSceneDataEntity.MaxPlayerCount);
            var joycons = new List<Joycon>(_inSceneDataEntity.RegisteredJoycons);
            foreach (var joycon in joycons)
            {
                _inSceneDataEntity.CancelRegisteredController(joycon);
            }
        }

        private void OnClickRegisterButton(Joycon registerJoycon)
        {
            if (_inSceneDataEntity.RegisteredJoycons.LastOrDefault()==null)
            {
                if (registerJoycon.isLeft)
                {
                    RegisterController(registerJoycon);
                    SEManager.Instance.Play(SEPath.CONTROLLER_SELECT);
                }
                return;
            }
            
            if (_inSceneDataEntity.RegisteredJoycons.LastOrDefault()==registerJoycon)
            {
                _inSceneDataEntity.CancelRegisteredController(registerJoycon);
                _controllersPanelView.ResetPaintImage(_inSceneDataEntity.RegisteredJoycons.Count);
                SEManager.Instance.Play(SEPath.JUMP);
                return;
            }
            
            if (_inSceneDataEntity.RegisteredJoycons.Contains(registerJoycon))
            {
                return;
            }

            if (_inSceneDataEntity.RegisteredJoycons.LastOrDefault()?.isLeft != registerJoycon?.isLeft)
            {
                SEManager.Instance.Play(SEPath.CONTROLLER_SELECT);
                RegisterController(registerJoycon);
            }
        }

        private void RegisterController(Joycon registeredJoycon)
        {
            _inSceneDataEntity.SetJoycon(registeredJoycon);
            _controllersPanelView.PaintImage(_inSceneDataEntity.RegisteredJoycons.Count-1);
            
            if (_inSceneDataEntity.RegisteredJoycons.Count==_inSceneDataEntity.MaxPlayerCount*2)
            {
                _inSceneDataEntity.SetJoyconNumData();
                _inSceneDataEntity.SetSettingsState(PlayerCustomState.Character);
            }
        }

    }
}