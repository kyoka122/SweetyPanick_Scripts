using InGame.Colate.Installer;
using InGame.Colate.View;
using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "ColateScriptableData", menuName = "ScriptableObjects/ColateScriptableData")]
    public class ColateScriptableData:ScriptableObject
    {
        public ColateView Prefab => prefab;
        public ColateInstaller Installer => installer;
        
        public int MaxHp => maxHp;
        public float MoveSpeed => moveSpeed;
        public float ConfuseDuration => confuseDuration;
        public float ThrowEnemyInterval => throwEnemyInterval;
        public float SurfaceSpeed => surfaceSpeed;
        public float ToSideWallDistance => toSideWallDistance;
        public float ToSideWallDistanceAtThrowEnemy => toSideWallDistanceAtThrowEnemy;
        public float ToGroundDistance => toGroundDistance;
        public float ColateFlyPosY => colateFlyPosY;

        public Vector2 ThrowEnemyPower => throwEnemyPower;
        public Vector2 ThrowEnemyPivot => throwEnemyPivot;
        public Vector2 NockBackPower => nockBackPower;
        public float SweetBoardMoveSpeed => sweetBoardMoveSpeed;
        public float SweetBoardMaxPosY => sweetBoardMaxPosY;
        public float SweetBoardMinPosY => sweetBoardMinPosY;
        public float SweetBoardMoveStayDuration => sweetBoardMoveStayDuration;
        public float SweetBoardInitPosY => sweetBoardInitPosY;
        public float DamagedRumbleDuration => damagedRumbleDuration;
        public float DamagedRumbleStrength => damagedRumbleStrength;
        public int DamagedRumbleVibrato => damagedRumbleVibrato;


        [SerializeField] private ColateView prefab;
        [SerializeField] private ColateInstaller installer;
        
        [SerializeField] private int maxHp=15;
        [SerializeField] private float moveSpeed=2f;
        [SerializeField] private float confuseDuration=5f;
        [SerializeField] private float throwEnemyInterval = 8f;
        [SerializeField] private float surfaceSpeed=2f;
        
        [SerializeField] private float toSideWallDistance=3f;
        [SerializeField] private float toSideWallDistanceAtThrowEnemy = 1f;
        [SerializeField] private float toGroundDistance=1f;
        [SerializeField] private float colateFlyPosY = 0.31f;

        [SerializeField] private Vector2 throwEnemyPower = new(1,1);
        [SerializeField] private Vector2 throwEnemyPivot = new(2,1);
        [SerializeField] private Vector2 nockBackPower = new(1,0);
        [SerializeField] private float sweetBoardMoveSpeed = 3;
        [SerializeField] private float sweetBoardMaxPosY = 0.27f;
        [SerializeField] private float sweetBoardMinPosY = -7.9f;
        [SerializeField] private float sweetBoardInitPosY=-8.16f;
        [SerializeField] private float sweetBoardMoveStayDuration = 2;
        [SerializeField] private float damagedRumbleDuration=0.2f;
        [SerializeField] private float damagedRumbleStrength = 0.5f;
        [SerializeField] private int damagedRumbleVibrato = 2;
    }
}