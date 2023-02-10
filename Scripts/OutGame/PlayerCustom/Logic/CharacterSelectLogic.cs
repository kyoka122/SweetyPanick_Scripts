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
    public class CharacterSelectLogic
    {
        private readonly InSceneDataEntity _inSceneDataEntity;
        private readonly ConstDataEntity _constDataEntity;
        private readonly ToMessageWindowSenderView _toMessageWindowSenderView;
        private readonly CharacterSelectPanelView _characterSelectPanelView;
        //private List<PlayerInputEntity> _playerInputEntities; //MEMO: キャラクターチェンジの度にリセット
        //private List<CharacterSelectCursorView> _characterSelectCursorViews; //MEMO: キャラクターチェンジの度にリセット
        private List<PlayerCursorData> _playerCursorData; //MEMO: キャラクターチェンジの度にリセット

        public CharacterSelectLogic(InSceneDataEntity inSceneDataEntity, ConstDataEntity constDataEntity,
            ToMessageWindowSenderView toMessageWindowSenderView, CharacterSelectPanelView characterSelectPanelView)
        {
            _inSceneDataEntity = inSceneDataEntity;
            _constDataEntity = constDataEntity;
            _toMessageWindowSenderView = toMessageWindowSenderView;
            _characterSelectPanelView = characterSelectPanelView;
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
            _playerCursorData = new List<PlayerCursorData>();
            
            for (int i = 0; i < _inSceneDataEntity.MaxPlayerCount; i++)
            {
                JoyconNumData joyconNumData = _inSceneDataEntity.RegisteredJoyconNumData[i];
                var playerCursorData = new PlayerCursorData(i+1,
                    new PlayerInputEntity(joyconNumData.rightJoyconNum, joyconNumData.leftJoyconNum),
                    _constDataEntity.CharacterSelectCursorInstanceManager.Install(_constDataEntity
                        .CharacterSelectCursorPrefab,i+1,_characterSelectPanelView.UpPanelRectTransform.transform,150f*i));
                _playerCursorData.Add(playerCursorData);
            }
            
            RegisterInputObserver();
        }

        private void RegisterInputObserver()
        {
            foreach (var playerCursorData in _playerCursorData)
            {
                playerCursorData.playerInputEntity.next
                    .Where(on=>on)
                    .Where(_ => _inSceneDataEntity.finishedPopUpWindowState == PlayerCustomState.Character)
                    .Subscribe(_ =>
                    {
                        Vector2 clickPos = _constDataEntity.WorldToScreenPoint(playerCursorData.characterSelectCursorView.GetRectPos());
                        var icon=GetClickIcon(clickPos);
                        Debug.Log($"icon:{icon}");
                        if (icon==null)
                        {
                            return;
                        }

                        playerCursorData.characterSelectCursorView.SetSelected(true);
                        playerCursorData.characterSelectCursorView.OffSelectableIcon();
                        SEManager.Instance.Play(SEPath.CLICK);
                        _inSceneDataEntity.SetUseCharacter(playerCursorData.playerNum,icon.Type);
                     
                        if (_inSceneDataEntity.useCharacterData.Count==_inSceneDataEntity.MaxPlayerCount)
                        {
                            _inSceneDataEntity.SetUseCharacterToDatabase();
                            Debug.Log("Set");
                            _inSceneDataEntity.SetSettingsState(PlayerCustomState.Finish);
                        }
                    })
                    .AddTo(_characterSelectPanelView);
                
                playerCursorData.playerInputEntity.back
                    .Where(on=>on)
                    .Where(_ => _inSceneDataEntity.finishedPopUpWindowState == PlayerCustomState.Character)
                    .Subscribe(_ =>
                    {
                        if (_inSceneDataEntity.useCharacterData
                            .Select(data=>data.playerNum)
                            .Any(num=>num==playerCursorData.playerNum))
                        {
                            return;
                        }
                        playerCursorData.characterSelectCursorView.OnSelectableIcon();
                        _inSceneDataEntity.CancelUseCharacter(playerCursorData.playerNum);
                    })
                    .AddTo(_characterSelectPanelView);

                playerCursorData.characterSelectCursorView.FixedUpdateAsObservable()
                    .Where(_ => _inSceneDataEntity.finishedPopUpWindowState == PlayerCustomState.Character)
                    .Subscribe(_ => TryMoveCursor(playerCursorData,
                        new Vector2(playerCursorData.playerInputEntity.horizontalValue.Value, 0)))
                    .AddTo(playerCursorData.characterSelectCursorView);

                playerCursorData.characterSelectCursorView.FixedUpdateAsObservable()
                    .Where(_ => _inSceneDataEntity.finishedPopUpWindowState == PlayerCustomState.Character)
                    .Subscribe(_ => TryMoveCursor(playerCursorData,
                        new Vector2(0, playerCursorData.playerInputEntity.verticalValue.Value)))
                    .AddTo(playerCursorData.characterSelectCursorView);
                
                playerCursorData.playerInputEntity.backPrevMenu
                    .Where(on=>on)
                    .Where(_ => _inSceneDataEntity.finishedPopUpWindowState == PlayerCustomState.Character)
                    .Subscribe(_=>
                    {
                        DestroyCursor();
                        _inSceneDataEntity.SetSettingsState(PlayerCustomState.Controller);
                    })
                    .AddTo(playerCursorData.characterSelectCursorView);
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
                _constDataEntity.CharacterSelectCursorInstanceManager.DestroyCursor(data.characterSelectCursorView);
            }
        }
    }
}