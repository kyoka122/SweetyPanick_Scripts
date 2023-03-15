﻿using System;
using InGame.Database;
using UniRx;
using UnityEngine;

namespace InGame.Colate.Entity
{
    public class ColateEntity:IDisposable
    {
        public int CurrentColateHp => _colateHp.Value;
        public IReadOnlyReactiveProperty<int> ColateHpChange => _colateHp;
        public IObservable<bool> DeadMotionFinished => _deadMotionFinished;
        public float ToSideWallDistance => _inGameDatabase.GetColateData().ToSideWallDistance;
        public float ToSideWallDistanceAtThrowEnemy => _inGameDatabase.GetColateData().ToSideWallDistanceAtThrowEnemy;
        public float ThrowEnemyInterval => _inGameDatabase.GetColateData().ThrowEnemyInterval;
        public Vector2 ThrowEnemyPower => _inGameDatabase.GetColateData().ThrowEnemyPower;
        public Vector2 ThrowEnemyPivot => _inGameDatabase.GetColateData().ThrowEnemyPivot;

        public float MoveSpeed => _inGameDatabase.GetColateData().MoveSpeed;
        public float ToGroundDistance => _inGameDatabase.GetColateData().ToGroundDistance;
        public Vector2 NockBackPower=>_inGameDatabase.GetColateData().NockBackPower;
        public float ConfuseDuration => _inGameDatabase.GetColateData().ConfuseDuration;
        public float SurfaceSpeed => _inGameDatabase.GetColateData().SurfaceSpeed;
        public float ColateFlyHeight => _inGameDatabase.GetColateData().ColateFlyHeight;

        private readonly Subject<bool> _deadMotionFinished;
        private readonly ReactiveProperty<int> _colateHp;
        private readonly InGameDatabase _inGameDatabase;
        
        public ColateEntity(InGameDatabase inGameDatabase)
        {
            _inGameDatabase = inGameDatabase;
            _colateHp = new ReactiveProperty<int>(_inGameDatabase.GetColateData().MaxHp);
            _deadMotionFinished = new Subject<bool>();
        }

        public void DamageDefault()
        {
            _colateHp.Value -= 1;
        }

        public void OnFinishedDeadMotion()
        {
            _deadMotionFinished.OnNext(true);
        }

        public void Dispose()
        {
            _colateHp?.Dispose();
            _deadMotionFinished?.Dispose();
        }
    }
}