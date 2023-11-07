using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace InGame.Stage.View
{
    public class CandyLightsGimmickView:DefaultGimmickSweetsView
    {
        public IObservable<bool> FixedCandy=>_fixedCandy;
    
        [SerializeField] private Light firstLight;
        [SerializeField] private Light endLight;
        [SerializeField] private Light[] aroundLights;
        [SerializeField] private float smallLightValue=1.02f;
        [SerializeField] private float smallLightRange=10.7f;
        [SerializeField] private float lightingLightValue=0.7f;
        [SerializeField] private float lightingLightRange=38f;
        
        [SerializeField] private Transform particleTransform;

        private Subject<bool> _fixedCandy;
        
        public override void Init()
        {
            _fixedCandy = new Subject<bool>();
            
            firstLight.intensity = smallLightValue;
            firstLight.range = smallLightRange;
            endLight.enabled = false;
            
            base.Init();
        }
        
        protected override void EachSweetsEvent()
        {
            _fixedCandy.OnNext(true);
        }

        public void OnLightAnimation(float particleMoveDuration)
        {
            particleTransform.gameObject.SetActive(true);
            particleTransform.position = firstLight.transform.position;
            particleTransform.transform
                .DOMove(endLight.transform.position, particleMoveDuration)
                .OnComplete(() =>
                {
                    endLight.enabled = true;
                    firstLight.intensity = lightingLightValue;
                    endLight.intensity = lightingLightValue;
                    firstLight.range = lightingLightRange;
                    endLight.range = lightingLightRange;
                    foreach (var aroundLight in aroundLights)
                    {
                        aroundLight.enabled = true;
                    }
                    particleTransform.gameObject.SetActive(false);
                });
        }

        protected override void OnDestroy()
        {
            _fixedCandy.Dispose();
        }
    }
}