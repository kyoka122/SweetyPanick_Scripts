using UnityEngine;

namespace InGame.Stage.View
{
    public class CandyLightGimmickViews:DefaultGimmickSweetsView
    {
        [SerializeField] private Light pointLight;
        [SerializeField] private float smallLightValue = 1.45f;
        [SerializeField] private float smallLightRange = 2f;
        [SerializeField] private float lightingLightValue=0.7f;
        [SerializeField] private float lightingLightRange=14.6f;

        public override void Init()
        {
            pointLight.intensity = smallLightValue;
            pointLight.range = smallLightRange;
            base.Init();
        }
        
        protected override void EachSweetsEvent()
        {
            pointLight.intensity = lightingLightValue;
            pointLight.range = lightingLightRange;
        }
    }
}