using MyApplication;
using UnityEngine;

namespace InGame.Enemy.View
{
    public interface IEnemyDamageAble
    {
        public void OnDamaged(Struct.DamagedInfo info);
    }
}