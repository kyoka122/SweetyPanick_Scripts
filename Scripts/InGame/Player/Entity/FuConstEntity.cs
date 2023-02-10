using InGame.Database;
using InGame.Player.View;
using MyApplication;
using UnityEngine;

namespace InGame.Player.Entity
{
    public class FuConstEntity
    {
        public readonly float inflateGumTime;
        public readonly float gumMoveToEnemyTime;
        public readonly Vector2 inflatedGumScale;
        public readonly float gumInflateToMoveInterval;
        public readonly float gumMoveSpeed;
        public readonly float gumAliveTime;
        public readonly SquareRange objectInScreenRange;
        public readonly Vector2 toGumInstanceVec;

        public readonly BindGumView bindGumView;

        public FuConstEntity(InGameDatabase inGameDatabase)
        {
            bindGumView = inGameDatabase.GetFuConstData().GumPrefab;
            var fuStatus = inGameDatabase.GetFuStatus();
            inflateGumTime = fuStatus.inflateGumTime;
            gumMoveToEnemyTime = fuStatus.gumMoveToEnemyTime;
            inflatedGumScale = fuStatus.inflatedGumScale;
            gumInflateToMoveInterval = fuStatus.gumInflateToMoveInterval;
            gumMoveSpeed = fuStatus.gumMoveSpeed;
            gumAliveTime = fuStatus.gumAliveTime;
            objectInScreenRange = inGameDatabase.GetStageSettings().ObjectInScreenRange;
            toGumInstanceVec = fuStatus.toGumInstanceVec;
        }
    }
}