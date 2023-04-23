using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using InGame.Player.View;
using MyApplication;
using UniRx;
using UnityEngine;
using Utility;

public class FuAnimatorView:PlayerAnimatorView
{
    [SerializeField] private ParticleGeneratorView fixedParticle;
    [SerializeField] private Transform fixParticleParent;
    
    private ObjectPool<ParticleSystem> _fixedParticlePool;
    
    public override void Init(PlayableCharacter type)
    {
        _fixedParticlePool = new ObjectPool<ParticleSystem>(fixedParticle);
        base.Init(type);
    }
    
    public override void CallbackAnimation(string animationClipName)
    {
        if (animationClipName==PlayerAnimationEventName.OnFixParticle)
        {
            ParticleSystem particle = _fixedParticlePool.GetObjectParentSet(fixParticleParent,
                Vector3.zero, Quaternion.identity, fixedParticle.Prefab.transform.localScale);
            particle.GetAsyncParticleSystemStoppedTrigger()
                .ToObservable()
                .FirstOrDefault()
                .Subscribe(_ =>
                {
                    _fixedParticlePool.ReleaseObject(particle);
                })
                .AddTo(this);
            particle.Play();
            
            return;
        }
        base.CallbackAnimation(animationClipName);
    }
}