using InGame.Stage.Entity;
using InGame.Stage.View;
using MyApplication;
using UnityEngine;

namespace InGame.Stage.Logic
{
    public class BackgroundLogic
    {
        private readonly StageBaseEntity _stageBaseEntity;
        private BackgroundView _currentBackGroundView;
        private bool _canUpdateBackGroundPos = true;
        
        public BackgroundLogic(StageBaseEntity stageBaseEntity)
        {
            _stageBaseEntity = stageBaseEntity;
        }
        
        public void InitAtMoveStage(StageArea newStageArea)
        {
            Init(newStageArea);
        }

        private void Init(StageArea newStageArea)
        {
            if (_currentBackGroundView!=null)
            {
                _currentBackGroundView.SetParent(null);
            }
            _currentBackGroundView = _stageBaseEntity.GetCameraData(newStageArea).BackgroundView;
            
            if(_currentBackGroundView==null)
            {
                Debug.LogWarning($"Not Found BackGroundView! area:{newStageArea}");
                _canUpdateBackGroundPos = false;
                _stageBaseEntity.SetBackGroundRangeCollider(null);
                _stageBaseEntity.SetCameraInitPos(Vector2.zero);
                _stageBaseEntity.SetPrevCameraPos(Vector2.zero);
                return;
            }

            _canUpdateBackGroundPos = true;
            _stageBaseEntity.SetBackGroundRangeCollider(_stageBaseEntity.StageAreaCollider2D(newStageArea));
            _stageBaseEntity.SetCameraInitPos(_stageBaseEntity.CameraPos);
            _stageBaseEntity.SetPrevCameraPos(_stageBaseEntity.CameraPos);
            
            //MEMO: 子オブジェクトにすることで追従させる
            _currentBackGroundView.SetParent(_stageBaseEntity.CameraTransform);
            
            SetBackGroundRange();
            SetStageRange();
        }
        
        //MEMO: カメラ位置によって背景位置をずらすことで背景に奥行きを出す
        public void LateUpdateBackGround()
        {
            if (!_canUpdateBackGroundPos)
            {
                return;
            }
            
            SetBackGroundRange();
#if UNITY_EDITOR
            SetStageRange();//MEMO: 実行中のColliderサイズの変更に対応させるため
#endif
            UpdateBackGroundPos(_stageBaseEntity.backGroundRangeRect, _stageBaseEntity.stageRangeRect);
        }
        
        

        /// <summary>
        /// 背景のワールドRectをセット
        /// </summary>
        private void SetBackGroundRange()
        {
            Vector2 colliderSize = _currentBackGroundView.backGroundCollider2D.size;
            Vector2 colliderOffset = _currentBackGroundView.backGroundCollider2D.offset;
            Matrix4x4 localToWorldMatrix = _currentBackGroundView.transform.localToWorldMatrix;
            Rect worldBoxColliderRange=GetWorldColliderRangeRect(colliderOffset, colliderSize, localToWorldMatrix);
            _stageBaseEntity.SetBackGroundRangeRect(worldBoxColliderRange);
        }
        
        /// <summary>
        /// ステージ範囲のワールドRectをセット
        /// </summary>
        private void SetStageRange()
        {
            Vector2 colliderSize = _stageBaseEntity.stageRangeCollider2D.size;
            Vector2 colliderOffset = _stageBaseEntity.stageRangeCollider2D.offset;
            Matrix4x4 localToWorldMatrix = _stageBaseEntity.stageRangeCollider2D.transform.localToWorldMatrix;
            Rect worldBoxColliderRange=GetWorldColliderRangeRect(colliderOffset, colliderSize, localToWorldMatrix);
            _stageBaseEntity.SetStageRangeRect(worldBoxColliderRange);
        }
        
        private Rect GetWorldColliderRangeRect(Vector2 localOffset,Vector2 localSize,Matrix4x4 localToWorldMatrix)
        {
            Vector2 worldColliderSize = localToWorldMatrix * localSize;
            Vector2 worldColliderOffset = localToWorldMatrix * localOffset;
            Rect range = new Rect(worldColliderOffset, worldColliderSize);
            return range;
        }

        
        private void UpdateBackGroundPos(Rect backGroundRange,Rect stageRange)
        {
            float backGroundMoveXFactoredStage = (_stageBaseEntity.CameraPos.x - stageRange.center.x) /
                stageRange.width * backGroundRange.width + backGroundRange.x;
            float backGroundMoveYFactoredStage = (_stageBaseEntity.CameraPos.y - stageRange.center.y) /
                stageRange.height * backGroundRange.height * 1.5f + backGroundRange.y;
            _currentBackGroundView.SetLocalXYPosition(new Vector2(-backGroundMoveXFactoredStage, -backGroundMoveYFactoredStage));
        }
    }
}