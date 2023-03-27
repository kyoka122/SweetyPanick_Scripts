using UnityEngine;

namespace InGame.Colate.View
{
    public class CrepeLiftView:DefaultSweetsLiftView
    {
        [SerializeField] private Material fadeMaterial;
        
        protected override Material FadeMaterial => fadeMaterial;
        
        public override void OnDestroy()
        {
            onFix?.Dispose();
            if (cutOffTransition.GetDisposeMaterial()!=null)
            {
                Destroy(cutOffTransition.GetDisposeMaterial());
            }
        }
    }
}