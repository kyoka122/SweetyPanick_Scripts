using InGame.Database;
using InGame.Player.View;
using MyApplication;
using UnityEngine;

namespace InGame.Player.Entity
{
    public class MashConstEntity
    {
        public readonly float mashNekoAliveTime;
        public readonly Vector2 toMashNekoInstanceVec;
        public readonly SquareRange objectInScreenRange;
        public readonly MashNekoView mashNekoPrefab;

        public MashConstEntity(InGameDatabase inGameDatabase)
        {
            MashStatus mashStatus = inGameDatabase.GetMashStatus();
            mashNekoAliveTime = mashStatus.mashNekoAliveTime;
            toMashNekoInstanceVec = mashStatus.toMashNekoInstanceVec;
            objectInScreenRange = inGameDatabase.GetStageSettings().ObjectInScreenRange;
            mashNekoPrefab = inGameDatabase.GetMashConstData().MashNekoPrefab;
        }
    }
}