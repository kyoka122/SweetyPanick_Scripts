using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Player.Entity;
using InGame.Player.View;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class PlayableCharacterSelectLogic
    {
        private const int ToPanelCloseFrame = 120;
        
        private readonly PlayerInputEntity _playerInputEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;
        private readonly PlayerStatusView _playerStatusView;
        
        private readonly CancellationToken _statusViewCancellationToken;
        private readonly int _playerMaxNum;
       
        private int _currentMoveDirectionValue;
        private PlayableCharacterIndex _playingCharacterIconIndex;
        private PlayableCharacterIndex _currentSelectingIconIndex;
        private bool _isSelecting;

        public PlayableCharacterSelectLogic(PlayerInputEntity playerInputEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity, PlayerCommonUpdateableEntity playerCommonUpdateableEntity,
            PlayerStatusView playerStatusView, PlayableCharacterIndex playingCharacter)
        {
            _playerMaxNum = Enum.GetValues(typeof(PlayableCharacterIndex)).Length-1;
            _playerInputEntity = playerInputEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerStatusView = playerStatusView;
            _statusViewCancellationToken = _playerStatusView.GetCancellationTokenOnDestroy();
            _playingCharacterIconIndex = playingCharacter;
            RegisterReactiveProperty();
        }

        //TODO: キャラクター切り替え実装後コメントアウト解除
        private void RegisterReactiveProperty()
        {
            /*
            _playerInputEntity.OnPlayerSelector
                .Where(_=>!_playerCommonUpdateableEntity.IsWarping)
                .Where(_=>!_isSelecting)
                .Where(onSelector=>!_playerStatusView.IsActiveSelectPanel==onSelector)
                .Subscribe(onSelector =>
                {
                    if (onSelector)
                    {
                        Debug.Log($"onSelector");
                        _playerStatusView.SetCharacterSelectPanel(true);
                        _currentSelectingIconIndex = _playingCharacterIconIndex;
                        _playerStatusView.SelectNextCharacter(_currentSelectingIconIndex);
                        return;
                    }
                    
                    _isSelecting = true;
                    Debug.Log($"offSelector");
                    ExeChangeCharacterTask();
                }).AddTo(_playerStatusView);;

            _playerInputEntity.PlayerSelectorMoveDirection
                .Where(_=>!_playerCommonInStageEntity.isOpeningMenu)
                .Where(_=>_playerStatusView.IsActiveSelectPanel)
                .Where(_=>!_isSelecting)
                .Subscribe(TryChangeSelectingCharacter)
                .AddTo(_statusViewCancellationToken);*/
        }

        private void TryChangeSelectingCharacter(int newDirectionValue)
        {
            if (_currentMoveDirectionValue == newDirectionValue) 
            {
                return;
            }

            _currentMoveDirectionValue = newDirectionValue;
            if (newDirectionValue==0)
            {
                return;
            }

            _currentSelectingIconIndex += newDirectionValue;
            _currentSelectingIconIndex = TryLoopIndex(_currentSelectingIconIndex);
            Debug.Log($"_currentSelectIconIndex:{_currentSelectingIconIndex}");
            _playerStatusView.SelectNextCharacter(_currentSelectingIconIndex);
        }

        //TODO
        private async void ExeChangeCharacterTask()
        {
            bool needToChangeRequest;
            if (false)
            {
                await RequestCharacterChange();
            }
            else
            {
                await DelayCloseSelectPanel();
            }
            SetOperatingCharacterIndex(_currentSelectingIconIndex);
        }
        
        private async UniTask DelayCloseSelectPanel()
        {
            await UniTask.DelayFrame(ToPanelCloseFrame, cancellationToken:_statusViewCancellationToken);
        }

        //TODO
        private async UniTask RequestCharacterChange()
        {
            /*switch (_currentSelectingIconIndex)
            {
                case PlayableCharacterIndex.Candy:
                    
                    break;
                case PlayableCharacterIndex.Mash:
                    break;
                case PlayableCharacterIndex.Fu:
                    break;
                case PlayableCharacterIndex.Kure:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (_currentSelectingIconIndex.ToString()==)
            {
                
            }*/
        }

        private void SetOperatingCharacterIndex(PlayableCharacterIndex characterType)
        {
            _playingCharacterIconIndex = characterType;
            _playerStatusView.ChangeCharacterIcon(_currentSelectingIconIndex);
            _playerStatusView.SetCharacterSelectPanel(false);
            _isSelecting = false;
            Debug.Log($"IconChange!:{_currentSelectingIconIndex}");
        }

        private PlayableCharacterIndex TryLoopIndex(PlayableCharacterIndex nextIndex)
        {
            if (nextIndex<0)
            {
                Debug.Log($"GetNextIconIndex");
                return (PlayableCharacterIndex)_playerMaxNum;
            }
            if ((int)nextIndex > _playerMaxNum)
            {
                Debug.Log($"GetNextIconIndex");
                return 0;
            }
            return nextIndex;
        }
    }
}