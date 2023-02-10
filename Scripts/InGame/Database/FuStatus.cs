using InGame.Player.Installer;
using InGame.Player.View;
using MyApplication;
using UnityEngine;

namespace InGame.Database
{
    public class FuStatus:BaseCharacterCommonStatus
    {
        public float inflateGumTime;
        public float gumMoveToEnemyTime;
        public Vector2 inflatedGumScale;
        public Vector2 toGumInstanceVec;
        public float gumInflateToMoveInterval;
        public float gumMoveSpeed;
        public float gumAliveTime;
        
        public FuStatus(CharacterBaseParameter characterBaseParameter, FuParameter fuParameter)
            : base(characterBaseParameter)
        {
            inflateGumTime = fuParameter.InflateGumTime;
            gumMoveToEnemyTime = fuParameter.GumMoveToEnemyTime;
            inflatedGumScale = fuParameter.InflatedGumScale;
            toGumInstanceVec = fuParameter.ToGumInstanceVec;
            gumInflateToMoveInterval = fuParameter.GumInflateToMoveInterval;
            gumMoveSpeed = fuParameter.GumMoveSpeed;
            gumAliveTime = fuParameter.GumAliveTime;
        }

        public FuStatus Clone()
        {
            return (FuStatus) MemberwiseClone();
        }
    }
}