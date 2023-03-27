using System.Linq;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.PlayerCustom.View;
using OutGame.PlayerCustom.Entity;
using Common.MyInput.PlayerCustom;
using UniRx;

namespace OutGame.PlayerCustom.Logic
{
    public class ControllerLogic
    {
        private readonly InputEntity _inputEntity;
        private readonly InSceneDataEntity _inSceneDataEntity;
        private readonly ControllersPanelView _controllersPanelView;
        private readonly ToMessageWindowSenderView _toMessageWindowSenderView;

        public ControllerLogic(InputEntity inputEntity,
            InSceneDataEntity inSceneDataEntity, ControllersPanelView controllersPanelView,
            ToMessageWindowSenderView toMessageWindowSenderView)
        {
            _inputEntity = inputEntity;
            _inSceneDataEntity = inSceneDataEntity;
            _controllersPanelView = controllersPanelView;
            _toMessageWindowSenderView = toMessageWindowSenderView;
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            _inSceneDataEntity.ChangedSettingsState
                .Where(state => state == PlayerCustomState.Controller)
                .Subscribe(_ =>
                {
                    ResetRegisteredControllers();
                    _controllersPanelView.InitByPlayerCount(_inSceneDataEntity.MaxPlayerCount);
                })
                .AddTo(_controllersPanelView);
            
            _inSceneDataEntity.HadFinishedPopUpWindow
                .Where(state => state == PlayerCustomState.Controller)
                .Subscribe(_ =>
                {
                    _toMessageWindowSenderView.SendControllerSettingsEvent();
                    _controllersPanelView.StartUseControllersChangeAnimation(_inSceneDataEntity.MaxPlayerCount);
                })
                .AddTo(_controllersPanelView);


            foreach (var input in _inputEntity.CustomInputs)
            {
                input.Next
                    .Where(on=>on)
                    .Where(_=>_inSceneDataEntity.finishedPopUpWindowState==PlayerCustomState.Controller)
                    .Subscribe(_=>OnRegisterButton(input))
                    .AddTo(_controllersPanelView);
                
                input.Back
                    .Where(on=>on)
                    .Where(_=>_inSceneDataEntity.finishedPopUpWindowState==PlayerCustomState.Controller)
                    .Subscribe(_=>
                    {
                        
                        OnCancelButton(input);
                    })
                    .AddTo(_controllersPanelView);

                //TODO: 長時間Backボタンを押したら戻る処理に変更
                input.Back
                    .Where(on => on)
                    .Where(_ => _inSceneDataEntity.finishedPopUpWindowState == PlayerCustomState.Controller)
                    .Where(_ => _inSceneDataEntity.SelectedControllers.Count == 0)
                    .Subscribe(_ => _inSceneDataEntity.SetSettingsState(PlayerCustomState.PlayerCount))
                    .AddTo(_controllersPanelView);
            }
        }

        private void ResetRegisteredControllers()
        {
            _controllersPanelView.ResetControllerImages(_inSceneDataEntity.MaxPlayerCount);
            _inSceneDataEntity.CancelAllRegisteredController();
        }

        private void OnRegisterButton(BasePlayerCustomInput controller)
        {
            if (_inSceneDataEntity.SelectedControllers.LastOrDefault()==null)
            {
                RegisterController(controller);
                SEManager.Instance.Play(SEPath.CONTROLLER_SELECT);
                return;
            }

            if (_inSceneDataEntity.SelectedControllers.Count==_inSceneDataEntity.MaxPlayerCount)
            {
                return;
            }
            
            if (_inSceneDataEntity.SelectedControllers.Contains(controller))
            {
                return;
            }

            //MEMO: 1Pのみキーボードを使用できる
            if (controller.DeviceType==MyInputDeviceType.Keyboard&&_inSceneDataEntity.SelectedControllers.Count==0)
            {
                return;
            }

            SEManager.Instance.Play(SEPath.CONTROLLER_SELECT);
            RegisterController(controller);
        }
        
        private void OnCancelButton(BasePlayerCustomInput controller)
        {
            if (_inSceneDataEntity.SelectedControllers.LastOrDefault()==controller)
            {
                _inSceneDataEntity.CancelRegisteredController(controller);
                _controllersPanelView.ResetPaintImage(_inSceneDataEntity.SelectedControllers.Count);
                SEManager.Instance.Play(SEPath.JUMP);
            }
        }

        private void RegisterController(BasePlayerCustomInput registeredController)
        {
            _inSceneDataEntity.SetController(registeredController);
            _controllersPanelView.PaintGamePadImage(registeredController.DeviceType,_inSceneDataEntity.SelectedControllers.Count-1);
            //_controllersPanelView.DebugText(_inSceneDataEntity.SelectedControllers.Count-1);
            
            if (_inSceneDataEntity.SelectedControllers.Count==_inSceneDataEntity.MaxPlayerCount)
            {
                _inSceneDataEntity.SetControllerToDatabase();
                _inSceneDataEntity.SetInGameControllerToDatabase();
                _inSceneDataEntity.SetSettingsState(PlayerCustomState.Character);
            }
        }
    }
}