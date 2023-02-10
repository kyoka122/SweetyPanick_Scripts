using UnityEngine;

namespace InGame.Stage.View
{
    public class CandyLightsGimmickView:DefaultGimmickSweetsView
    {
        [SerializeField] private Light[] pointLights;
        
        protected override void EachSweetsEvent()
        {
            foreach (var pointLight in pointLights)
            {
                pointLight.enabled = true;
            }
        }
    }
}