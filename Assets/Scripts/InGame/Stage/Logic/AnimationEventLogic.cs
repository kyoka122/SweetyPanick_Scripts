using InGame.Common.Interface;
using InGame.Stage.Entity;
using KanKikuchi.AudioManager;
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
                        switch (name)
                        {
                            case StageAnimationCallbackName.OnCrepeImpact:
                                _stageGimmickEntity.CameraShakeEvent.Invoke(_stageGimmickEntity.CrepeCameraShakeVelocity);
                                SEManager.Instance.Play(SEPath.CREPE_ROLLING);
                                break;
                            case StageAnimationCallbackName.OnCrepeSoundLoop:
                                //MEMO: ↓ 複数対応はしていないため、複数クレープを置く際にはEntityに情報を持たせる。
                                SEManager.Instance.Play(SEPath.CREPE_ROLLING,isLoop:true);
                                break;
                            case StageAnimationCallbackName.OffCrepeSoundLoop:
                                SEManager.Instance.Stop(SEPath.CREPE_ROLLING);
                                break;
                        }
                    });
            }
        }

    }
}