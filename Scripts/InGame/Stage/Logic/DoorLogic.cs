using System;
using System.Linq;
using InGame.Stage.View;
using InGame.Player.View;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Stage.Logic
{
    public class DoorLogic
    {
        private readonly StageObjectsView _stageObjectsView;
        private readonly Action<StageEvent> _stageEvent;

        public DoorLogic(StageObjectsView stageObjectsView,Action<StageEvent> stageEvent)
        {
            _stageObjectsView = stageObjectsView;
            _stageEvent = stageEvent;
            RegisterDoorEvent();
        }

        private void RegisterDoorEvent()
        {
            foreach (var doorData in _stageObjectsView.GetDoorData())
            {
                doorData.DoorView.Init();
                doorData.DoorView.EnterDoorObserver.Subscribe(SwitchDoorEvent);
            }
        }

        private void SwitchDoorEvent(DoorView doorView)
        {
            var doorType = _stageObjectsView
                .GetDoorData()
                .FirstOrDefault(data => data.DoorView == doorView)?
                .DoorType;
            
            switch (doorType)
            {
                case DoorType.None:
                    break;
                case DoorType.FirstStageGoal:
                    _stageEvent.Invoke(StageEvent.EnterFirstStageGoalDoor);
                    break;
                case DoorType.SecondStageMiddle:
                    _stageEvent.Invoke(StageEvent.EnterSecondStageMiddleDoor);
                    break;
                case DoorType.SecondHiddenStage:
                    _stageEvent.Invoke(StageEvent.EnterSecondHiddenStageDoor);
                    break;
                case DoorType.SecondStageGoal:
                    _stageEvent.Invoke(StageEvent.EnterSecondStageGoalDoor);
                    break;
                case null:
                    break;
                default:
                    Debug.LogError($"Could Not Find. doorType:{doorType}");
                    break;
            }
        }

    }
}