using UnityEngine;

namespace InGame.Stage.View
{
    public class CandyLightGimmickViews:DefaultGimmickSweetsView
    {
        [SerializeField] private Light pointLight;
        
        protected override void EachSweetsEvent()
        {
            pointLight.enabled = true;
        }
    }
}