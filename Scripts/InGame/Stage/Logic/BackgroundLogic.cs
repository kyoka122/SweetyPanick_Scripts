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

            var data = _stageBaseEntity.GetCameraData(newStageArea);
            if(data==null)
            {
                _currentBackGroundView = null;
                _stageBaseEntity.SetCameraInitPos(Vector2.zero);
                _stageBaseEntity.SetPrevCameraPos(Vector2.zero);
                _canUpdateBackGroundPos = false;
                _stageBaseEntity.SetStageRangeCollider(null);
                return;
            }

            _currentBackGroundView = data.BackgroundView;
            if (_currentBackGroundView==null)
            {
                _canUpdateBackGroundPos = false;
                Debug.LogWarning($"Not Found BackGroundView! area:{newStageArea}");
            }
            else
            {
                _canUpdateBackGroundPos = true;
            }

            _stageBaseEntity.SetStageRangeCollider(_stageBaseEntity.GetStageAreaCollider2D(newStageArea));
          
            //_stageBaseEntity.SetCameraInitPos(_stageBaseEntity.CameraPos);
            _stageBaseEntity.SetPrevCameraPos(_stageBaseEntity.CameraPos);
            
            //MEMO: 子オブジェクトにすることで追従させる
            _currentBackGroundView.SetParent(_stageBaseEntity.CameraTransform);

            //Debug.Log($"backGroundCollider2D:{_currentBackGroundView.backGroundCollider2D.transform.position}",_currentBackGroundView.backGroundCollider2D);
            //Debug.Log($"stageRangeCollider2D:{_stageBaseEntity.stageRangeCollider2D.transform.position}",_stageBaseEntity.stageRangeCollider2D);
            SetBackGroundRange();
            SetStageRange();
            //Debug.Log($"astageRangeCollider2D:{_stageBaseEntity.stageRangeCollider2D.transform.position}",_stageBaseEntity.stageRangeCollider2D);
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
            //Debug.Log($"astageRangeCollider2D:{_stageBaseEntity.stageRangeCollider2D.transform.position}",_stageBaseEntity.stageRangeCollider2D);
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
                stageRange.height * backGroundRange.height / 2 / 7-5f;
            _currentBackGroundView.SetLocalXYPosition(new Vector2(-backGroundMoveXFactoredStage,-backGroundMoveYFactoredStage));
        }
    }
}