using InGame.Stage.Entity;
using InGame.Stage.View;
using UnityEngine;

namespace InGame.Stage.Logic
{
    public class MoveFloorLogic
    {
        private readonly MoveFloorView _moveFloorView;
        private readonly StageGimmickEntity _stageGimmickEntity;

        public MoveFloorLogic(MoveFloorView moveFloorView, StageGimmickEntity stageGimmickEntity)
        {
            _moveFloorView = moveFloorView;
            _stageGimmickEntity = stageGimmickEntity;
        }

        public void FixedUpdateFloor()
        {
            if (_moveFloorView.HadFixedGums())
            {
                if (_moveFloorView.Position.y < _moveFloorView.MoveYMax)
                {
                    _moveFloorView.SetYVelocity(_stageGimmickEntity.MoveFloorSpeed);
                    return;
                }
                _moveFloorView.SetYVelocity(0);
            }
            else
            {
                if (_moveFloorView.Position.y > _moveFloorView.InitPos.y)
                {
                    _moveFloorView.SetYVelocity(-_stageGimmickEntity.MoveFloorSpeed);
                    return;
                }
                _moveFloorView.SetYVelocity(0);
            }
        }
    }
}