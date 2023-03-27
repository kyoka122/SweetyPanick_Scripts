using MyApplication;
using UnityEngine;

namespace InGame.Colate.View
{
    public interface IColateOrderAble
    {
        public Vector2 CenterPos { get; }
        EnemyState state { get; }
        public void AddVelocity(Vector2 velocity);
        public void Destroy();
    }
}