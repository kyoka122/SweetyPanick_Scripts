using InGame.Enemy.Installer;
using InGame.Enemy.View;
using InGame.Player.View;
using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "EnemyScriptableData", menuName = "ScriptableObjects/EnemyScriptableData")]
    public class EnemyScriptableData:ScriptableObject
    {
        public DefaultEnemyView[] DefaultEnemyPrefab => defaultEnemyPrefab;
        public EnemyInstaller EnemyInstaller => enemyInstaller;
        public SpriteEffectView ShockSprite => shockSprite;
        public float MoveSpeed=>moveSpeed;
        public float HavingKeyEnemyMoveSpeed=>havingKeyEnemyMoveSpeed;
        public float FlyAwaySpeed => flyAwaySpeed;
        public float ChangeDirectionTime => changeDirectionTime;
        public float ToSweetsDistance => toSweetsDistance;
        public float ToWallDistance => toWallDistance;
        public float ToGroundDistance => toGroundDistance;
        public float EatUpTime => eatUpTime;
        public float FlyMinAngle => flyMinAngle;
        public float FlyMaxAngle => flyMaxAngle;
        public float FlyRotateAnimationSpeed => flyRotateAnimationSpeed;
        public float GravityScale => gravityScale;
        public float GumReleaseReactionTime => gumReleaseReactionTime;
        public float ShockSpriteDuration => shockSpriteDuration;
        public int TrampolineBoundDelayCount => trampolineBoundDelayCount;
        public float TrampolineBoundValue => trampolineBoundValue;

        [SerializeField] private DefaultEnemyView[] defaultEnemyPrefab;
        [SerializeField] private EnemyInstaller enemyInstaller;
        [SerializeField] private SpriteEffectView shockSprite;
        
        [SerializeField] private float moveSpeed;
        [SerializeField] private float havingKeyEnemyMoveSpeed=3.5f;
        [SerializeField] private float flyAwaySpeed;
        [SerializeField] private float changeDirectionTime;
        [SerializeField] private float toSweetsDistance;
        [SerializeField] private float toWallDistance;
        [SerializeField] private float toGroundDistance = 1;
        [SerializeField] private float eatUpTime;
        [SerializeField] private float flyMinAngle;
        [SerializeField] private float flyMaxAngle;
        [SerializeField] private float flyRotateAnimationSpeed;
        [SerializeField] private float gravityScale;
        [SerializeField] private float gumReleaseReactionTime;
        [SerializeField] private float shockSpriteDuration;
        [SerializeField] private int trampolineBoundDelayCount = 12;
        [SerializeField] private float trampolineBoundValue = 3;
        
        public EnemyScriptableData()
        {
            if (flyMinAngle>flyMaxAngle)
            {
                Debug.LogError($"flyMinAngle > flyMaxAngle");
            }
        }
    }
}