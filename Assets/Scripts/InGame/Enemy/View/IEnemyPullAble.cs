using UnityEngine;

namespace InGame.Enemy.View
{
    public interface IEnemyPullAble
    {
        public bool isPullAble { get; }
        
        public void OnPull(Transform weaponTransform);

        public void OffPull();
    }
}