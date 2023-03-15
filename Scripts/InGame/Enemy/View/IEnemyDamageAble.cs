﻿using MyApplication;
using UnityEngine;

namespace InGame.Enemy.View
{
    /// <summary>
    /// 敵のViewに継承するとダメージを与えられるになる。
    /// ViewをアタッチしたゲームオブジェクトにIsTriggerのCollider2Dをアタッチすること。
    /// </summary>
    public interface IEnemyDamageAble
    {
        public void OnDamaged(Struct.DamagedInfo info);
    }
}