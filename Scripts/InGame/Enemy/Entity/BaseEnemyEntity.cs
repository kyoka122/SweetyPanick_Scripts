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
        public Func<Vector2, Vector2> WorldToViewPortPoint => _commonDatabase.GetIReadOnlyCameraController().WorldToViewPortPoint;
        public SquareRange ObjectInScreenRange => _inGameDatabase.GetStageSettings().ObjectInScreenRange;
        public float StageBottom => _inGameDatabase.GetStageSettings().StageBottom;
        
        public SpriteEffectView shockSprite { get; private set; }
        public float moveSpeed { get; private set; }
        public float flyAwaySpeed { get; private set; }
        public float changeDirectionTime { get; private set; }
        public float toSweetsDistance { get; private set; }
        public float toWallDistance { get; private set; }
        public float eatUpTime { get; private set; }
        public float flyMaxAngle { get; private set; }
        public float flyMinAngle { get; private set; }
        public float flyRotateAnimationAngle { get; private set; }
        public float originGravityScele { get; private set; }
        public float gumReleaseReactionTime { get; private set; }
        public float shockSpriteDuration { get; private set; }
        
        
        private readonly InGameDatabase _inGameDatabase;
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
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
        }
    }
}