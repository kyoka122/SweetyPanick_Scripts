using InGame.MyCamera.Installer;
using InGame.MyCamera.View;
using MyApplication;
using Unity;
using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "StageSettingsScriptableData", menuName = "ScriptableObjects/StageSettingsScriptableData")]
    public class StageSettingsScriptableData:ScriptableObject
    {
        public SquareRange ObjectInScreenRange => objectInScreenRange;
        public SquareRange InPlayerGroupRange => inPlayerGroupRange;
        public float StageBottom => stageBottom;
        public Vector2 FixNormalSweetsParticleSize => fixNormalSweetsParticleSize;
        public Vector2 FixGimmickSweetsParticleSize => fixGimmickSweetsParticleSize;
        public float BackGroundMoveRateX => backGroundMoveRateX;
        public float BackGroundMoveRateY => backGroundMoveRateY;
        public CameraInstaller CameraInstallerPrefab => cameraInstallerPrefab;

        [SerializeField] private SquareRange objectInScreenRange;
        [SerializeField] private SquareRange inPlayerGroupRange;
        [SerializeField] private float stageBottom;
        [SerializeField] private Vector2 fixNormalSweetsParticleSize=new(1,1);
        [SerializeField] private Vector2 fixGimmickSweetsParticleSize=new(2,2);
        [SerializeField] private float backGroundMoveRateX=0.9f;
        [SerializeField] private float backGroundMoveRateY=0.95f;
        [SerializeField] private CameraInstaller cameraInstallerPrefab;
        
    }
}