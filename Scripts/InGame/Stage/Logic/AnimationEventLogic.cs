using InGame.Common.Interface;
using InGame.Stage.Entity;
using MyApplication;
using UniRx;

namespace InGame.Stage.Logic
{
    public class AnimationEventLogic
    {
        private readonly IAnimationCallbackSender[] _animationCallbackSenders;
        private readonly StageGimmickEntity _stageGimmickEntity;

        public AnimationEventLogic(IAnimationCallbackSender[] animationCallbackSenders,StageGimmickEntity stageGimmickEntity)
        {
            _animationCallbackSenders = animationCallbackSenders;
            _stageGimmickEntity = stageGimmickEntity;
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            foreach (var animationCallbackSender in _animationCallbackSenders)
            {
                animationCallbackSender.OnAnimationEvent
                    .Subscribe(name =>
                    {
                        if (name==StageAnimationCallbackName.OnCrepeCameraShake)
                        {
                            _stageGimmickEntity.CameraShakeEvent.Invoke(_stageGimmickEntity.CrepeCameraShakeVelocity);
                        }
                    });
            }
        }

    }
}