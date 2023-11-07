using Common.Interface;
using InGame.Stage.Entity;
using KanKikuchi.AudioManager;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Stage.Logic
{
    public class AnimationEventLogic
    {
        private readonly IAnimationCallbackSender[] _animationCallbackSenders;
        private readonly StageGimmickEntity _stageGimmickEntity;

        public AnimationEventLogic(IAnimationCallbackSender[] animationCallbackSenders,StageGimmickEntity stageGimmickEntity)
        {
            _animationCallbackSenders = animationCallbackSenders;
            Debug.Log($"animationCallbackSenders.Length:{animationCallbackSenders.Length}");
            Debug.Log($"animationCallbackSenders:{animationCallbackSenders[0]}");
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
                                SEManager.Instance.Play(SEPath.CREPE_ROLLING, volumeRate: 7f);
                                Debug.Log($"OnCrepeImpact");
                                break;
                            case StageAnimationCallbackName.OnCrepeSoundLoop:
                                //MEMO: ↓ 複数対応はしていないため、複数クレープを置く際にはEntityに情報を持たせる。
                                SEManager.Instance.Play(SEPath.CREPE_ROLLING, volumeRate: 7f,isLoop:true);
                                Debug.Log($"OnCrepeSoundLoop");
                                break;
                            case StageAnimationCallbackName.OffCrepeSoundLoop:
                                SEManager.Instance.Stop(SEPath.CREPE_ROLLING);
                                Debug.Log($"OffCrepeSoundLoop");
                                break;
                            
                            //MEMO: ↓SmallCrepe
                            case StageAnimationCallbackName.OnSmallCrepeImpact:
                                SEManager.Instance.Play(SEPath.CREPE_ROLLING, volumeRate: 6f, pitch: 3f);
                                Debug.Log($"OnSmallCrepeImpact");
                                break;
                            case StageAnimationCallbackName.OnSmallCrepeSoundLoop:
                                //MEMO: ↓ 複数対応はしていないため、複数クレープを置く際にはEntityに情報を持たせる。
                                SEManager.Instance.Play(SEPath.CREPE_ROLLING, isLoop: true, volumeRate: 6f,
                                    pitch: 3f);
                                Debug.Log($"OnSmallCrepeSoundLoop");
                                break;
                            case StageAnimationCallbackName.OffSmallCrepeSoundLoop:
                                Debug.Log($"OffSmallCrepeSoundLoop");
                                SEManager.Instance.Stop(SEPath.CREPE_ROLLING);
                                break;
                        }
                    });
            }
        }

    }
}