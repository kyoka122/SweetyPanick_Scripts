using UnityEngine;

namespace InGame.Enemy.View
{
    public interface IEnemyDamageAble
    {
        public void OnPunched(Vector2 playerPos);
        public void OnCrepeRolled();
    }
}