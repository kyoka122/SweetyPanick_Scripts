using System;
using System.Collections.Generic;
using System.Linq;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.PlayerCustom.Data;
using OutGame.PlayerCustom.Entity;
using OutGame.PlayerCustom.View;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OutGame.PlayerCustom.Logic
{
    public class CharacterSelectLogic:IDisposable
    {
        private readonly InSceneDataEntity _inSceneDataEntity;
        private readonly ConstDataEntity _constDataEntity;
        private readonly ToMessageWindowSenderView _toMessageWindowSenderView;
        private readonly CharacterSelectPanelView _characterSelectPanelView;
        private List<PlayerCursorData> _playerCursorData; //MEMO: キャラクターチェンジの度にリセット
        private readonly List<IDisposable> _disposables;
        private readonly List<IDisposable> _onCancelControllerDisposables;

        public CharacterSelectLogic(InSceneDataEntity inSceneDataEntity, ConstDataEntity constDataEntity,
            ToMessageWindowSenderView toMessageWindowSenderView, CharacterSelectPanelView characterSelectPanelView)
        {
            _inSceneDataEntity = inSceneDataEntity;
            _constDataEntity = constDataEntity;
            _toMessageWindowSenderView = toMessageWindowSenderView;
            _characterSelectPanelView = characterSelectPanelView;
            _disposables = new List<IDisposable>();
            _onCancelControllerDisposables = new List<IDisposable>();
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            _inSceneDataEntity.HadFinishedPopUpWindow
                .Where(state => state == PlayerCustomState.Character)
                .Subscribe(_ =>
                {
                    _toMessageWindowSenderView.SendCharacterSettingsEvent();
                })
                .AddTo(_characterSelectPanelView);
            
            _inSceneDataEntity.ChangedSettingsState
                .Where(state => state == PlayerCustomState.Character)
                .Subscribe(_ =>
                {
                    SetKeyConfigInfoAnimator();
                    InitPlayerInputEntity();
                    RegisterInputObserver();
                })
                .AddTo(_characterSelectPanelView);
                
            _inSceneDataEntity.ChangedSettingsState
                .Where(state => state != PlayerCustomState.Character)
                .Subscribe(_ =>
                {
                    _characterSelectPanelView.SetEnableAnimator(false);
                })
                .AddTo(_characterSelectPanelView);
        }

        private void InitPlayerInputEntity()
        {
            //MEMO: 違う画面から戻ってきたときに必ずリストを初期化したいため、ここでインスタンス。
            _playerCursorData = new List<PlayerCursorData>();
            
            for (int i = 0; i < _inSceneDataEntity.MaxPlayerCount; i++)
            {
                var playerInputEntity = new CharacterSelectInputEntity(_inSceneDataEntity.RegisteredPlayerSelectController[i]);
                var playerCursorData = new PlayerCursorData(i+1, playerInputEntity,
                    _constDataEntity.CharacterSelectCursorInstaller.Install(_constDataEntity.CharacterSelectCursorPrefab,
                        i+1,_characterSelectPanelView.UpPanelRectTransform.transform,_constDataEntity.CursorInterval*i));
                _playerCursorData.Add(playerCursorData);
                _disposables.Add(playerInputEntity);
            }
        }

        private void RegisterInputObserver()
        {
            foreach (var playerCursorData in _playerCursorData)
            {
                _onCancelControllerDisposables.Add(
                    playerCursorData.characterSelectInputEntity.next
                        .Where(on => on)
                        .Where(_ => _inSceneDataEntity.FinishedPopUpWindowState == PlayerCustomState.Character)
                        .Where(_ => _inSceneDataEntity.PlayerCustomState == PlayerCustomState.Character)
                        .Subscribe(_ =>
                        {
                            Vector2 clickPos =
                                _constDataEntity.WorldToScreenPoint(playerCursorData.characterSelectCursorView
                                    .GetRectPos());
                            var icon = GetClickIcon(clickPos);
                            if (icon == null)
                            {
                                return;
                            }
                            if (_inSceneDataEntity.useCharacterData.Any(data => data.playerNum == playerCursorData.playerNum))
                            {
                                return;
                            }
                            if (_inSceneDataEntity.useCharacterData.Any(data => data.type == icon.Type))
                            {
                                return;
                            }

                            SetCharacter(playerCursorData, icon);

                        })
                        .AddTo(_characterSelectPanelView));

                _onCancelControllerDisposables.Add(
                    playerCursorData.characterSelectInputEntity.back
                        .Where(on => on)
                        .Where(_ => _inSceneDataEntity.FinishedPopUpWindowState == PlayerCustomState.Character)
                        .Where(_ => _inSceneDataEntity.PlayerCustomState == PlayerCustomState.Character)
                        .Subscribe(_ =>
                        {
                            if (_inSceneDataEntity.useCharacterData
                                .Select(data => data.playerNum)
                                .Any(num => num == playerCursorData.playerNum))
                            {
                                return;
                            }

                            playerCursorData.characterSelectCursorView.OnSelectableIcon();
                            _inSceneDataEntity.CancelUseCharacter(playerCursorData.playerNum);
                        })
                        .AddTo(_characterSelectPanelView));

                _onCancelControllerDisposables.Add(
                    playerCursorData.characterSelectCursorView.FixedUpdateAsObservable()
                        .Where(_ => _inSceneDataEntity.FinishedPopUpWindowState == PlayerCustomState.Character)
                        .Where(_ => _inSceneDataEntity.PlayerCustomState == PlayerCustomState.Character)
                        .Subscribe(_ =>
                        {
                            TryMoveCursor(playerCursorData,
                                new Vector2(playerCursorData.characterSelectInputEntity.horizontalValue.Value,
                                    playerCursorData.characterSelectInputEntity.verticalValue.Value));
                        })
                        .AddTo(playerCursorData.characterSelectCursorView));

                _onCancelControllerDisposables.Add(
                    playerCursorData.characterSelectInputEntity.back
                        .Where(on => on)
                        .Where(_ => _inSceneDataEntity.FinishedPopUpWindowState == PlayerCustomState.Character)
                        .Where(_ => _inSceneDataEntity.PlayerCustomState == PlayerCustomState.Character)
                        .Where(_ => _inSceneDataEntity.useCharacterData.Count == 0)
                        .Subscribe(_ =>
                        {
                            DestroyCursor();
                            DisposeOnCancelController();
                            _inSceneDataEntity.SetSettingsState(PlayerCustomState.Controller);
                        })
                        .AddTo(playerCursorData.characterSelectCursorView));
            }
        }

        private void SetCharacter(PlayerCursorData playerCursorData, CharacterIconView icon)
        {
            playerCursorData.characterSelectCursorView.SetSelected(true);
            playerCursorData.characterSelectCursorView.OffSelectableIcon();
            SEManager.Instance.Play(SEPath.CLICK);
            _inSceneDataEntity.SetUseCharacter(playerCursorData.playerNum,icon.Type);
                     
            if (_inSceneDataEntity.useCharacterData.Count==_inSceneDataEntity.MaxPlayerCount)
            {
                _inSceneDataEntity.SetUseCharacterToDatabase();
                _inSceneDataEntity.SetSettingsState(PlayerCustomState.Finish);
            }
        }
        
        private CharacterIconView GetClickIcon(Vector2 cursorRectPos)
        {
            PointerEventData pointData = new PointerEventData(EventSystem.current);
            pointData.position = cursorRectPos;
            List<RaycastResult> raycastResults=new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointData,raycastResults);
            
            var selectedView = raycastResults
                .Select(hit => hit.gameObject.GetComponent<CharacterIconView>())
                .FirstOrDefault(characterIconView => characterIconView != null);
            return selectedView;
        }
        
        private void TryMoveCursor(PlayerCursorData data,Vector2 moveDirection)
        {
            if (data.characterSelectCursorView.isSelected)
            {
                return;
            }
            Vector2 currentPos=data.characterSelectCursorView.GetRectAnchorPosition();
            
            currentPos += moveDirection * 3;
            data.characterSelectCursorView.SetRectAnchorPosition(currentPos);
        }
        
        private void SetKeyConfigInfoAnimator()
        {
            _characterSelectPanelView.ResetAnimator();
            _characterSelectPanelView.InActiveAllKeyConfigAnimatorParameter();
            _characterSelectPanelView.SetChangeAnimationParameter(!IsUsedOnlyOneTypeController());
            
            foreach (var playerCustomInput in _inSceneDataEntity.RegisteredPlayerSelectController)
            {
                _characterSelectPanelView.SetBoolKeyConfigAnimator(
                    UIAnimatorParameter.GetKeyConfigAnimatorParameter(playerCustomInput.DeviceType),true);
            }
            _characterSelectPanelView.SetEnableAnimator(true);
        }

        private void DestroyCursor()
        {
            foreach (var data in _playerCursorData)
            {
                data.characterSelectCursorView.DestroyObj();
            }
        }
        
        private void DisposeOnCancelController()
        {
            foreach (var disposable in _onCancelControllerDisposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();
        }
        
        private bool IsUsedOnlyOneTypeController()
        {
            MyInputDeviceType type = _inSceneDataEntity.RegisteredPlayerSelectController[0].DeviceType;
            for (var i =1; i < _inSceneDataEntity.RegisteredPlayerSelectController.Count; i++)
            {
                //MEMO: Joyconの場合、左右が違っても同じコントローラー解説画面を使用するため
                if (type is MyInputDeviceType.JoyconLeft or MyInputDeviceType.JoyconRight)
                {
                    Debug.Log($"type is MyInputDeviceType.JoyconLeft or MyInputDeviceType.JoyconRight");
                    Debug.Log($"{_inSceneDataEntity.RegisteredPlayerSelectController[i].DeviceType==MyInputDeviceType.JoyconLeft}");
                    Debug.Log($"{_inSceneDataEntity.RegisteredPlayerSelectController[i].DeviceType==MyInputDeviceType.JoyconRight}");
                    if (_inSceneDataEntity.RegisteredPlayerSelectController[i].DeviceType is not
                        (MyInputDeviceType.JoyconLeft or MyInputDeviceType.JoyconRight))
                    {
                        return false;
                    }
                }
                else if (type!=_inSceneDataEntity.RegisteredPlayerSelectController[i].DeviceType)
                {
                    return false;
                }
            }
            return true;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}