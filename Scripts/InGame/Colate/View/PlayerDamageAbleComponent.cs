using System;
using InGame.Enemy.View;
using MyApplication;
using UniRx;
using UnityEngine;

public class PlayerDamageAbleComponent:MonoBehaviour,IEnemyDamageAble
{
    public IObservable<bool> Attacked=>_attackedSubject;
    private Subject<bool> _attackedSubject;

    public void Init()
    {
        _attackedSubject = new Subject<bool>();
    }
    
    public void OnDamaged(Struct.DamagedInfo info)
    {
        if (info.attacker==Attacker.Player)
        {
            _attackedSubject.OnNext(true);
        }
    }

    private void OnDestroy()
    {
        _attackedSubject.Dispose();
    }
}