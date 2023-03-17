using InGame.Stage.View;
using MyApplication;
using UnityEngine;

namespace InGame.Database
{
    public class CameraInitData:MonoBehaviour
    {
        public StageArea StageArea => stageArea;
        public CameraMode CameraMode => cameraMode;
        public Vector3 InitPosition => initPosition;
        public BoxCollider2D StageAreaCollider => stageAreaCollider;
        public CompositeCollider2D StageAreaCompositeCollider => stageAreaCompositeCollider;
        public BackgroundView BackgroundView=>backgroundView;

        [SerializeField] private StageArea stageArea;
        [SerializeField] private Vector3 initPosition;
        [SerializeField] private CameraMode cameraMode;
        [SerializeField] private BoxCollider2D stageAreaCollider;
        [SerializeField] private CompositeCollider2D stageAreaCompositeCollider;
        [SerializeField] private BackgroundView backgroundView;
        
    }
}