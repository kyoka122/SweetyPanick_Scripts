using MyApplication;
using UnityEngine;

namespace InGame.Enemy.Interface
{
    public interface IColateOrderAble
    {
        public EnemyState state { get; }

        public void AddVelocity(Vector2 velocity);
    }
}