using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.PlayerCustom.View;
using OutGame.PlayerCustom.Entity;
using UniRx;
using UnityEngine;

namespace OutGame.PlayerCustom.Logic
{
    public class PlayerCountButtonLogic
    {
        private readonly InputCaseUnknownControllerEntity _inputCaseUnknownControllerEntity;
        private readonly InSceneDataEntity _inSceneDataEntity;
        private readonly PlayerCountView _playerCountView;
        private readonly ToMessageWindowSenderView _toMessageWindowSenderView;

        public PlayerCountButtonLogic(InputCaseUnknownControllerEntity inputCaseUnknownControllerEntity,
            InSceneDataEntity inSceneDataEntity, PlayerCountView playerCountView,ToMessageWindowSenderView toMessageWindowSenderView)
        {
            _inputCaseUnknownControllerEntity = inputCaseUnknownControllerEntity;
            _inSceneDataEntity = inSceneDataEntity;
            _playerCountView = playerCountView;
            _toMessageWindowSenderView = toMessageWindowSenderView;
            RegisterObserver();
        }

        public void SetPlayerCountState()
        {
            _inSceneDataEntity.SetSettingsState(PlayerCustomState.PlayerCount);
        }

        private void RegisterObserver()
        {
            foreach (var leftInput in _inputCaseUnknownControllerEntity.joyconLeftInputs)
            {
                leftInput.HorizontalCommand
                    .Where(direction=>direction!=0)
                    .Where(_=>_inSceneDataEntity.finishedPopUpWindowState==PlayerCustomState.PlayerCount)
                    .Subscribe(value=>ChangeSelectingPlayerCountView(new Vector2(value,0)))
                    .AddTo(_playerCountView);

                leftInput.VerticalCommand
                    .Where(direction=>direction!=0)
                    .Where(_=>_inSceneDataEntity.finishedPopUpWindowState==PlayerCustomState.PlayerCount)
                    .Subscribe(value=>ChangeSelectingPlayerCountView(new Vector2(0,value)))
                    .AddTo(_playerCountView);
            }
            
            foreach (var rightInput in _inputCaseUnknownControllerEntity.joyconRightInputs)
            {
                rightInput.Next
                    .Where(on=>on)
                    .Where(_=>_inSceneDataEntity.finishedPopUpWindowState==PlayerCustomState.PlayerCount)
                    .Subscribe(_=>SelectPlayerCount())
                    .AddTo(_playerCountView);
            }

            _inSceneDataEntity.changedSettingsState
                .Where(state => state == PlayerCustomState.PlayerCount)
                .Subscribe(_ =>
                {
                    _playerCountView.ResetAllButton();
                    _inSceneDataEntity.SetMaxPlayerCount(1);
                    PopUp(_playerCountView. GetViewTransform(_inSceneDataEntity.selectingViewNum));
                })
                .AddTo(_playerCountView);
            
            _inSceneDataEntity.hadFinishedPopUpWindow
                .Where(state => state == PlayerCustomState.PlayerCount)
                .Subscribe(_ =>
                {
                    _toMessageWindowSenderView.SendPlayerCountSettingsEvent();
                })
                .AddTo(_playerCountView);
        }

        private void ChangeSelectingPlayerCountView(Vector2 input)
        {
            _inSceneDataEntity.SetSelectingPlayerCountViewCache(_inSceneDataEntity.selectingViewNum);
            _inSceneDataEntity.SetSelectingPlayerCountView(JudgeNextViewNum(input));
            
            PopDown(_playerCountView. GetViewTransform(_inSceneDataEntity.selectingViewNumCache));
            PopUp(_playerCountView. GetViewTransform(_inSceneDataEntity.selectingViewNum));
        }

        private int JudgeNextViewNum(Vector2 input)
        {
            switch (_inSceneDataEntity.selectingViewNum)
            {
                case 2 when input.x is 1 or -1:
                case 3 when input.y is 1 or -1:
                    return 1;
                case 1 when input.x is 1 or -1:
                case 4 when input.y is 1 or -1:
                    return 2;
                
                case 1 when input.y is 1 or -1:
                case 4 when input.x is 1 or -1:
                    return 3;
                case 2 when input.y is 1 or -1:
                case 3 when input.x is 1 or -1:
                    return 4;
                default:
                    Debug.LogError($"SelectingViewNum:{_inSceneDataEntity.selectingViewNum}, x:{input.x}, y:{input.y}");
                    return 0;
            }
        }

        private void SelectPlayerCount()
        {
            _inSceneDataEntity.SetMaxPlayerCount(_inSceneDataEntity.selectingViewNum);
            SEManager.Instance.Play(SEPath.CLICK);
            _inSceneDataEntity.SetSettingsState(PlayerCustomState.Controller);
        }

        private void PopUp(Transform popUpTarget)
        {
            popUpTarget.localScale *= 1.2f;
        }

        private void PopDown(Transform popDownTarget)
        {
            popDownTarget.localScale = Vector2.one;
        }
    }
}