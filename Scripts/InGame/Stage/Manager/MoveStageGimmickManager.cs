using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using InGame.Stage.Logic;
using MyApplication;

namespace InGame.Stage.Manager
{
    public class MoveStageGimmickManager:IDisposable
    {
        private readonly HealAnimationLogic _healAnimationLogic;
        private readonly DoorLogic _doorLogic;
        private readonly MoveFloorLogic[] _moveFloorLogics;
        private readonly BackgroundLogic _backgroundLogic;
        private readonly AnimationEventLogic _animationEventLogic;
        private readonly CandyLightsLogic _candyLightsLogic;
        private readonly List<IDisposable> _disposables;

        public MoveStageGimmickManager(HealAnimationLogic healAnimationLogic, DoorLogic doorLogic,
            MoveFloorLogic[] moveFloorLogics, BackgroundLogic backgroundLogic,AnimationEventLogic animationEventLogic,
            CandyLightsLogic candyLightsLogic, List<IDisposable> disposables)
        {
            _healAnimationLogic = healAnimationLogic;
            _doorLogic = doorLogic;
            _moveFloorLogics = moveFloorLogics;
            _backgroundLogic = backgroundLogic;
            _animationEventLogic = animationEventLogic;
            _candyLightsLogic = candyLightsLogic;
            _disposables = disposables;
        }

        public UniTask GetAllPlayerHealAnimationTask()
        {
            return _healAnimationLogic.PlayHealAnimationTask();
        }

        public void FixedUpdate()
        {
            foreach (var moveFloorLogic in _moveFloorLogics)
            {
                moveFloorLogic.FixedUpdateFloor();
            }
        }

        public void LateUpdateBackGround()
        {
            _backgroundLogic.LateUpdateBackGround();
        }

        public void UnsetBackGround()
        {
            _backgroundLogic.UnsetBackGroundParent();
        }
        
        public void InitAtStageMove(StageArea newStageArea)
        {
            _doorLogic.InitAtStageMove();
            _backgroundLogic.InitAtMoveStage(newStageArea);
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