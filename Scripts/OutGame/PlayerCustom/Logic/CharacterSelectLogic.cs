using System;
using System.Collections.Generic;
using System.Linq;
using InGame.Database;
using InGame.Player.Entity;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.PlayerCustom.Data;
using OutGame.PlayerCustom.Entity;
using OutGame.PlayerCustom.Installer;
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
        //private List<PlayerInputEntity> _playerInputEntities; //MEMO: キャラクターチェンジの度にリセット
        //private List<CharacterSelectCursorView> _characterSelectCursorViews; //MEMO: キャラクターチェンジの度にリセット
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
            _inSceneDataEntity.hadFinishedPopUpWindow
                .Where(state => state == PlayerCustomState.Character)
                .Subscribe(_ =>
                {
                    _toMessageWindowSenderView.SendCharacterSettingsEvent();
                })
                .AddTo(_characterSelectPanelView);
            
            _inSceneDataEntity.changedSettingsState
                .Where(state => state == PlayerCustomState.Character)
                .Subscribe(_ =>
                {
                    InstallPlayerInputEntity();
                })
                .AddTo(_characterSelectPanelView);
        }

        private void InstallPlayerInputEntity()
        {
            //MEMO: 違う画面から戻ってきたときに必ずリストを初期化したいため、ここでインスタンス。
            _playerCursorData = new List<PlayerCursorData>();
            
            for (int i = 0; i < _inSceneDataEntity.MaxPlayerCount; i++)
            {
                var playerInputEntity = new CharacterSelectInputEntity(_inSceneDataEntity.RegisteredPlayerSelectController[i]);
                var playerCursorData = new PlayerCursorData(i+1, playerInputEntity,
                    _constDataEntity.CharacterSelectCursorInstaller.Install(_constDataEntity.CharacterSelectCursorPrefab,
                        i+1,_characterSelectPanelView.UpPanelRectTransform.transform,150f*i));

                _playerCursorData.Add(playerCursorData);
                _disposables.Add(playerInputEntity);
            }
            
            RegisterInputObserver();
        }

        private void RegisterInputObserver()
        {
            foreach (var playerCursorData in _playerCursorData)
            {
                _onCancelControllerDisposables.Add(
                    playerCursorData.characterSelectInputEntity.next
                    .Where(on => on)
                    .Where(_ => _inSceneDataEntity.finishedPopUpWindowState == PlayerCustomState.Character)
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

                        if (_inSceneDataEntity.useCharacterData.Any(
                            data => data.playerNum == playerCursorData.playerNum))
                        {
                            return;
                        }

                        SetCharacter(playerCursorData, icon);

                    })
                    .AddTo(_characterSelectPanelView));

                _onCancelControllerDisposables.Add(
                    playerCursorData.characterSelectInputEntity.back
                        .Where(on => on)
                        .Where(_ => _inSceneDataEntity.finishedPopUpWindowState == PlayerCustomState.Character)
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
                        .Where(_ => _inSceneDataEntity.finishedPopUpWindowState == PlayerCustomState.Character)
                        .Subscribe(_ =>
                        {
                            TryMoveCursor(playerCursorData,
                                new Vector2(playerCursorData.characterSelectInputEntity.horizontalValue.Value,
                                    playerCursorData.characterSelectInputEntity.verticalValue.Value));
                        })
                        .AddTo(playerCursorData.characterSelectCursorView));

                //TODO: 長時間Backボタンを押したら戻る処理に変更
                _onCancelControllerDisposables.Add(
                    playerCursorData.characterSelectInputEntity.back
                        .Where(on => on)
                        .Where(_ => _inSceneDataEntity.finishedPopUpWindowState == PlayerCustomState.Character)
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

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}