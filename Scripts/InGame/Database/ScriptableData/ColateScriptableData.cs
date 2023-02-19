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
        public float ToGroundDistance => toGroundDistance;
        public float ColateFlyHeight => colateFlyHeight;

        public Vector2 ThrowEnemyPower => throwEnemyPower;
        public Vector2 NockBackPower => nockBackPower;
        


        [SerializeField] private ColateView prefab;
        [SerializeField] private ColateInstaller installer;
        
        [SerializeField] private int maxHp=15;
        [SerializeField] private float moveSpeed=2f;
        [SerializeField] private float confuseDuration=5f;
        [SerializeField] private float throwEnemyInterval = 8f;
        [SerializeField] private float surfaceSpeed=2f;
        
        [SerializeField] private float toSideWallDistance=3f;
        [SerializeField] private float toGroundDistance=1f;
        [SerializeField] private float colateFlyHeight = 10f;

        [SerializeField] private Vector2 throwEnemyPower = new(1,1);
        [SerializeField] private Vector2 nockBackPower = new(1,0);
    }
}