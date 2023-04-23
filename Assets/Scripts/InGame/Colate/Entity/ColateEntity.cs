using System;
using InGame.Database;
using MyApplication;
using UniRx;
using UnityEngine;
using Utility;

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
        public float ColateFlyHeight => _inGameDatabase.GetColateData().ColateFlyPosY;
        public float SweetBoardMoveSpeed=>_inGameDatabase.GetColateData().SweetBoardMoveSpeed;
        public float SweetBoardMaxPosY=>_inGameDatabase.GetColateData().SweetBoardMaxPosY;
        public float SweetBoardMinPosY=>_inGameDatabase.GetColateData().SweetBoardMinPosY;
        public float SweetBoardMoveStayDuration => _inGameDatabase.GetColateData().SweetBoardMoveStayDuration;
        public float SweetBoardInitPosY => _inGameDatabase.GetColateData().SweetBoardInitPosY;

        public float DamagedRumbleDuration => _inGameDatabase.GetColateData().DamagedRumbleDuration;
        public float DamagedRumbleStrength => _inGameDatabase.GetColateData().DamagedRumbleStrength;
        public int DamagedRumbleVibrato => _inGameDatabase.GetColateData().DamagedRumbleVibrato;
        
        public ObjectPool<ParticleSystem> bigMiscParticle { get; }
        public ObjectPool<ParticleSystem> smallMiscParticle { get; }
        public ColateState prevAttackState { get; private set; }

        private readonly Subject<bool> _deadMotionFinished;
        private readonly ReactiveProperty<int> _colateHp;
        private readonly InGameDatabase _inGameDatabase;

        public ColateEntity(InGameDatabase inGameDatabase,ObjectPool<ParticleSystem> bigMiscParticleObjectPool,
            ObjectPool<ParticleSystem> smallMiscParticleObjectPool)
        {
            _inGameDatabase = inGameDatabase;
            bigMiscParticle = bigMiscParticleObjectPool;
            smallMiscParticle = smallMiscParticleObjectPool;
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

        public void SetPrevAttackState(ColateState state)
        {
            prevAttackState = state;
        }

        public void Dispose()
        {
            _colateHp?.Dispose();
            _deadMotionFinished?.Dispose();
        }
    }
}