using MyApplication;
using UnityEngine;

namespace InGame.Database
{
    public class CameraData:MonoBehaviour
    {
        public StageArea StageArea => stageArea;
        public CameraMode CameraMode => cameraMode;
        public Vector3 InitPosition => initPosition;
        public Collider2D StageAreaCollider => stageAreaCollider;

        [SerializeField] private StageArea stageArea;
        [SerializeField] private Vector3 initPosition;
        [SerializeField] private CameraMode cameraMode;
        [SerializeField] private Collider2D stageAreaCollider;
    }
}