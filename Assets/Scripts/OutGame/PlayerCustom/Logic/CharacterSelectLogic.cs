using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Database;
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
        private readonly CharacterIconView[] _characterIconViews;
        private List<PlayerCursorData> _playerCursorData; //MEMO: キャラクターチェンジの度にリセット
        private readonly List<IDisposable> _disposables;
        private readonly List<IDisposable> _onCancelControllerDisposables;

        public CharacterSelectLogic(InSceneDataEntity inSceneDataEntity, ConstDataEntity constDataEntity,
            ToMessageWindowSenderView toMessageWindowSenderView, CharacterSelectPanelView characterSelectPanelView,
            CharacterIconView[] characterIconViews)
        {
            _inSceneDataEntity = inSceneDataEntity;
            _constDataEntity = constDataEntity;
            _toMessageWindowSenderView = toMessageWindowSenderView;
            _characterSelectPanelView = characterSelectPanelView;
            _characterIconViews = characterIconViews;
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
                    //SetKeyConfigInfoAnimator();
                    InitPlayerInputEntity();
                    RegisterInputObserver();
                })
                .AddTo(_characterSelectPanelView);
                
            _inSceneDataEntity.ChangedSettingsState
                .Where(state => state != PlayerCustomState.Character)
                .Subscribe(_ =>
                {
                    //_characterSelectPanelView.SetEnableAnimator(false);
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
                        i+1,_characterSelectPanelView.PanelObj.transform,_characterSelectPanelView.GetCursorPos(i+1)));
                _playerCursorData.Add(playerCursorData);
                _disposables.Add(playerInputEntity);
            }
            foreach (var characterIconView in _characterIconViews)
            {
                characterIconView.Reset();
            }
        }

        private void RegisterInputObserver()
        {
            foreach (var characterIconView in _characterIconViews)
            {
                _onCancelControllerDisposables.Add(
                characterIconView.FixedUpdateAsObservable()
                    .Where(on =>
                    {
                        return _inSceneDataEntity.FinishedPopUpWindowState == PlayerCustomState.Character &&
                               _inSceneDataEntity.PlayerCustomState == PlayerCustomState.Character;
                    }).Subscribe(_ =>
                    {
                        foreach (var iconView in _characterIconViews)
                        {
                            iconView.SetColor();
                        }
                    }).AddTo(characterIconView));
            }

            foreach (var playerCursorData in _playerCursorData)
            {
                _onCancelControllerDisposables.Add(
                    playerCursorData.characterSelectInputEntity.next
                        .Where(on =>
                        {
                            return on && _inSceneDataEntity.FinishedPopUpWindowState == PlayerCustomState.Character &&
                                   _inSceneDataEntity.PlayerCustomState == PlayerCustomState.Character;
                        })
                        .Subscribe(_ =>
                        {
                            if (playerCursorData.characterSelectCursorView.selectType==UISelectState.Selected)
                            {
                                return;
                            }
                            
                            var selectedView=_characterIconViews
                                .Where(view=>view.IsOverlap(playerCursorData.playerNum))
                                .FirstOrDefault(view=>!view.IsSelected());
                            if (selectedView == null)
                            {
                                return;
                            }

                            SetCharacter(playerCursorData, selectedView);

                        })
                        .AddTo(_characterSelectPanelView));

                _onCancelControllerDisposables.Add(
                    playerCursorData.characterSelectInputEntity.back
                        .Where(on =>
                        {
                            return on && _inSceneDataEntity.FinishedPopUpWindowState == PlayerCustomState.Character &&
                                   _inSceneDataEntity.PlayerCustomState == PlayerCustomState.Character;
                        })
                        .Subscribe(_ =>
                        {
                            if (playerCursorData.characterSelectCursorView.selectType==UISelectState.Selected)
                            {
                                playerCursorData.characterSelectCursorView.SetType(UISelectState.None);
                                foreach (var characterIconView in _characterIconViews)
                                {
                                    characterIconView.SetType(playerCursorData.playerNum,UISelectState.None);
                                }
                                _inSceneDataEntity.CancelUseCharacter(playerCursorData.playerNum);
                            }
                            else //if (_inSceneDataEntity.useCharacterData.Count == 0)
                            {
                                DestroyCursor();
                                DisposeOnCancelController();
                                _inSceneDataEntity.SetSettingsState(PlayerCustomState.Controller);
                            }
                        })
                        .AddTo(_characterSelectPanelView));

                _onCancelControllerDisposables.Add(
                        playerCursorData.characterSelectCursorView.FixedUpdateAsObservable()
                        .Where(_ =>
                        {
                            return _inSceneDataEntity.FinishedPopUpWindowState == PlayerCustomState.Character &&
                                   _inSceneDataEntity.PlayerCustomState == PlayerCustomState.Character;
                        })
                        .Subscribe(_ =>
                        {
                            if (playerCursorData.characterSelectCursorView.selectType==UISelectState.Selected)
                            {
                                return;
                            }
                            MoveCursor(playerCursorData,
                                new Vector2(playerCursorData.characterSelectInputEntity.horizontalValue.Value,
                                    playerCursorData.characterSelectInputEntity.verticalValue.Value));
                            Vector2 clickPos =
                                _constDataEntity.WorldToScreenPoint(playerCursorData.characterSelectCursorView.GetRectPos());
                            var icon = GetOverlapIcon(clickPos);
                            if (icon == null)
                            {
                                playerCursorData.characterSelectCursorView.SetType(UISelectState.None);
                                foreach (var iconView in _characterIconViews)
                                {
                                    iconView.SetType(playerCursorData.playerNum,UISelectState.None);
                                }
                                return;
                            }

                            playerCursorData.characterSelectCursorView.SetType(UISelectState.Overlap);
                            foreach (var characterIconView in _characterIconViews)
                            {
                                if (characterIconView==icon)
                                {
                                    characterIconView.SetType(playerCursorData.playerNum,UISelectState.Overlap);
                                    continue;
                                }
                                characterIconView.SetType(playerCursorData.playerNum,UISelectState.None);
                            }

                        })
                        .AddTo(playerCursorData.characterSelectCursorView));
                
            }
        }

        private void SetCharacter(PlayerCursorData playerCursorData, CharacterIconView icon)
        {
            icon.SetType(playerCursorData.playerNum,UISelectState.Selected);
            icon.SetColor();
            playerCursorData.characterSelectCursorView.SetType(UISelectState.Selected);
            
            SEManager.Instance.Play(SEPath.CLICK);
            _inSceneDataEntity.SetUseCharacter(playerCursorData.playerNum,icon.Type);
            
            if (_inSceneDataEntity.useCharacterData.Count==_inSceneDataEntity.MaxPlayerCount)
            {
                _inSceneDataEntity.SetUseCharacterToDatabase();
                _inSceneDataEntity.SetSettingsState(PlayerCustomState.Finish);
            }
        }
        
        private CharacterIconView GetOverlapIcon(Vector2 cursorRectPos)
        {
            PointerEventData pointData = new PointerEventData(EventSystem.current);
            pointData.radius = _constDataEntity.CursorRayRadius;
            pointData.position = cursorRectPos;
            List<RaycastResult> raycastResults=new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointData,raycastResults);
            
            var selectedView = raycastResults
                .Select(hit => hit.gameObject.GetComponent<CharacterIconView>())
                .FirstOrDefault(characterIconView => characterIconView != null);
            return selectedView;
        }
        
        private void MoveCursor(PlayerCursorData data,Vector2 moveDirection)
        {
            Vector2 currentPos=data.characterSelectCursorView.GetRectAnchorPosition();
            
            currentPos += moveDirection * 3;
            data.characterSelectCursorView.SetRectAnchorPosition(currentPos);
        }
        
        private void SetKeyConfigInfoAnimator()
        {
            //_characterSelectPanelView.ResetAnimator();
            //_characterSelectPanelView.InActiveAllKeyConfigAnimatorParameter();
            //_characterSelectPanelView.SetChangeAnimationParameter(!IsUsedOnlyOneTypeController());
            
            /*foreach (var playerCustomInput in _inSceneDataEntity.RegisteredPlayerSelectController)
            {
                _characterSelectPanelView.SetBoolKeyConfigAnimator(
                    UIAnimatorParameter.GetKeyConfigAnimatorParameter(playerCustomInput.DeviceType),true);
            }*/
            //_characterSelectPanelView.SetEnableAnimator(true);
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