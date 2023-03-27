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
        private bool _canUpdateBackGroundTransform = true;
        
        public BackgroundLogic(StageBaseEntity stageBaseEntity)
        {
            _stageBaseEntity = stageBaseEntity;
        }
        
        public void InitAtMoveStage(StageArea newStageArea)
        {
            Init(newStageArea);
        }

        public void UnsetBackGroundParent()
        {
            if (_currentBackGroundView!=null)
            {
                _currentBackGroundView.SetParent(null);
            }
        }

        private void Init(StageArea newStageArea)
        {
            var data = _stageBaseEntity.GetCameraData(newStageArea);
            if(data==null)
            {
                _currentBackGroundView = null;
                _stageBaseEntity.SetCameraInitPos(Vector2.zero);
                _stageBaseEntity.SetPrevCameraPos(Vector2.zero);
                _canUpdateBackGroundTransform = false;
                _stageBaseEntity.SetStageRangeCollider(null);
                return;
            }

            if (data.BackgroundView==null)
            {
                _canUpdateBackGroundTransform = false;
                Debug.LogWarning($"Not Found BackGroundView! area:{newStageArea}");
                return;
            }
            
            _currentBackGroundView = data.BackgroundView;
            _canUpdateBackGroundTransform = true;
            SetSizeRate();
            _stageBaseEntity.SetStageRangeCollider(_stageBaseEntity.GetStageAreaCollider2D(newStageArea));
          
            //_stageBaseEntity.SetCameraInitPos(_stageBaseEntity.CameraPos);
            _stageBaseEntity.SetPrevCameraPos(_stageBaseEntity.CameraPos);
            
            //MEMO: 子オブジェクトにすることで追従させる
            _currentBackGroundView.SetParent(_stageBaseEntity.CameraTransform);

            SetBackGroundRange();
            SetStageRange();
        }
        
        //MEMO: カメラ位置によって背景位置をずらすことで背景に奥行きを出す
        public void LateUpdateBackGround()
        {
            if (!_canUpdateBackGroundTransform)
            {
                return;
            }
            
            SetBackGroundRange();
#if UNITY_EDITOR
            SetStageRange();//MEMO: 実行中のColliderサイズの変更に対応させるため
            SetSizeRate();
#endif
            UpdateBackGroundPos(_stageBaseEntity.backGroundRangeRect, _stageBaseEntity.stageRangeRect);
            UpdateBackGroundSize(_stageBaseEntity.CameraSize);
        }

        /// <summary>
        /// 背景のワールドRectをセット
        /// </summary>
        private void SetBackGroundRange()
        {
            Vector2 colliderSize = _currentBackGroundView.backGroundCollider2D.size;
            Vector2 colliderOffset = _currentBackGroundView.backGroundCollider2D.offset;
            
            Rect worldBoxColliderRange=GetWorldColliderRangeRect(colliderOffset, colliderSize, _currentBackGroundView.GetPosition());
            _stageBaseEntity.SetBackGroundRangeRect(worldBoxColliderRange);
        }
        
        /// <summary>
        /// ステージ範囲のワールドRectをセット
        /// </summary>
        private void SetStageRange()
        {
            Vector2 colliderSize = _stageBaseEntity.stageRangeCollider2D.size;
            Vector2 colliderOffset = _stageBaseEntity.stageRangeCollider2D.offset;
            Vector2 colliderObjPos = _stageBaseEntity.stageRangeCollider2D.transform.position;
            Rect worldBoxColliderRange=GetWorldColliderRangeRect(colliderOffset, colliderSize, colliderObjPos);
            
            _stageBaseEntity.SetStageRangeRect(worldBoxColliderRange);
        }
        
        /// <summary>
        /// カメラのSizeに対する背景のSize拡張度を設定
        /// </summary>
        private void SetSizeRate()
        {
            var rate = (_currentBackGroundView.MaxSize-_currentBackGroundView.MinSize) /
                       (_stageBaseEntity.CameraMaxSize - _stageBaseEntity.CameraMinSize);
            _stageBaseEntity.SetBackGroundSizeRate(rate);
        }
        
        private Rect GetWorldColliderRangeRect(Vector2 localOffset,Vector2 localSize,Vector2 pos)
        {
            Vector2 rectPosition = pos + localOffset - localSize / 2f;
            Rect range = new Rect(rectPosition, localSize);
            return range;
        }

        
        private void UpdateBackGroundPos(Rect backGroundRange,Rect stageRange)
        {
            float backGroundMoveXFactoredStage = (_stageBaseEntity.CameraPos.x - stageRange.center.x) /
                (stageRange.width) * backGroundRange.width / 2 - _currentBackGroundView.backGroundCollider2D.offset.x;

            float backGroundMoveYFactoredStage = (_stageBaseEntity.CameraPos.y - stageRange.center.y) /
                stageRange.height * backGroundRange.height / 2 / 7-4f;
            _currentBackGroundView.SetLocalXYPosition(new Vector2(-backGroundMoveXFactoredStage,-backGroundMoveYFactoredStage));
        }
        
        private void UpdateBackGroundSize(float cameraSize)
        {
            float addSizeRate = _stageBaseEntity.backGroundSizeRate * (cameraSize - _stageBaseEntity.CameraMinSize);
            addSizeRate=Mathf.Max(0, addSizeRate);
            float size = _currentBackGroundView.MaxSize + addSizeRate;
            _currentBackGroundView.SetSize(size);
        }

    }
}