using MyApplication;
using UnityEngine;

namespace InGame.Enemy.View
{
    public interface IEnemyBindable
    {
        public bool isBindable { get; }
        public Vector2 CenterPos { get; }
        
        public void OnBind();
        
        public void OffBind();
    }
}