using System;
using InGame.Common.Database;
using InGame.Database;
using InGame.Player.View;
using MyApplication;
using InGame.Database.ScriptableData;
using UnityEngine;

namespace InGame.Enemy.Entity
{
    public abstract class BaseEnemyEntity
    {
        public Func<Vector2, Vector2> WorldToViewPortPoint => _commonDatabase.GetReadOnlyCameraController().WorldToViewPortPoint;
        public SquareRange ObjectInScreenRange => inGameDatabase.GetStageSettings().ObjectInScreenRange;
        public float StageBottom => inGameDatabase.GetStageSettings().StageBottom;
        
        public SpriteEffectView shockSprite { get; }
        public virtual float moveSpeed { get; }
        public float flyAwaySpeed { get; }
        public float changeDirectionTime { get; }
        public float toSweetsDistance { get; }
        public float toWallDistance { get; }
        public float eatUpTime { get; }
        public float flyMaxAngle { get; }
        public float flyMinAngle { get; }
        public float flyRotateAnimationAngle { get; }
        public float originGravityScele { get; }
        public float gumReleaseReactionTime { get; }
        public float shockSpriteDuration { get; }
        
        public bool hadMoved { get; private set; }

        protected readonly InGameDatabase inGameDatabase;
        private readonly CommonDatabase _commonDatabase;
        
        public BaseEnemyEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase)
        {
            EnemyScriptableData data = inGameDatabase.GetEnemyData();
            shockSprite = data.ShockSprite;
            moveSpeed = data.MoveSpeed;
            flyAwaySpeed = data.FlyAwaySpeed;
            changeDirectionTime = data.ChangeDirectionTime;
            toSweetsDistance = data.ToSweetsDistance;
            toWallDistance = data.ToWallDistance;
            eatUpTime = data.EatUpTime;
            flyMinAngle = data.FlyMinAngle;
            flyMaxAngle = data.FlyMaxAngle;
            flyRotateAnimationAngle = data.FlyRotateAnimationSpeed * 100f;
            originGravityScele = data.GravityScale;
            gumReleaseReactionTime = data.GumReleaseReactionTime;
            shockSpriteDuration = data.ShockSpriteDuration;
            this.inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
        }
        
        public void SetHadMoved()
        {
            hadMoved = true;
        }
    }
}