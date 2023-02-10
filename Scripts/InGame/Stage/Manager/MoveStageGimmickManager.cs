using Cysharp.Threading.Tasks;
using InGame.Stage.Logic;

namespace InGame.Stage.Manager
{
    public class MoveStageGimmickManager
    {
        private readonly HealAnimationLogic _healAnimationLogic;
        private readonly DoorLogic _doorLogic;
        private readonly MoveFloorLogic[] _moveFloorLogics;
        private readonly BackgroundLogic _backgroundLogic;

        public MoveStageGimmickManager(HealAnimationLogic healAnimationLogic, DoorLogic doorLogic,
            MoveFloorLogic[] moveFloorLogics, BackgroundLogic backgroundLogic)
        {
            _healAnimationLogic = healAnimationLogic;
            _doorLogic = doorLogic;
            _moveFloorLogics = moveFloorLogics;
            _backgroundLogic = backgroundLogic;
        }

        public UniTask GetAllPlayerHealAnimationTask()
        {
            return _healAnimationLogic.PlayHealAnimationTask();
        }

        public void LateInit()
        {
            _backgroundLogic.LateInit();
        }

        public void FixedUpdate()
        {
            foreach (var moveFloorLogic in _moveFloorLogics)
            {
                moveFloorLogic.FixedUpdateFloor();
            }
            //_backgroundLogic.FixedUpdate();
        }

        public void LateUpdate()
        {
            _backgroundLogic.LateUpdate();
        }
    }
}