using UnityEngine;

namespace InGame.Enemy.View
{
    public interface IEnemyDecoyAble
    {
        public bool isDecoyAble { get; }
        
        public void OnDecoy(Transform newDecoyTransform);

        public void OffDecoy();
    }
}