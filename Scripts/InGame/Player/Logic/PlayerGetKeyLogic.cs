using DG.Tweening;
using InGame.Player.Entity;
using InGame.Player.View;
using KanKikuchi.AudioManager;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class PlayerGetKeyLogic
    {
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly BasePlayerView _playerView;
        private readonly GameObject _keyUI;

        public PlayerGetKeyLogic(PlayerCommonUpdateableEntity playerCommonUpdateableEntity,
            PlayerConstEntity playerConstEntity, PlayerCommonInStageEntity playerCommonInStageEntity,BasePlayerView playerView, GameObject keyUI)
        {
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerConstEntity = playerConstEntity;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            _playerView = playerView;
            _keyUI = keyUI;
            RegisterObserver();
        }

        private void RegisterObserver()
        {
            _playerView.OnCollisionEnterObj
                .Subscribe(collision2D =>
                {
                    GameObject colliderObj = collision2D.collider.gameObject;
                    if (colliderObj.gameObject.layer==LayerInfo.KeyNum)
                    {
                        _playerCommonUpdateableEntity.SetHavingKey();
                        colliderObj.SetActive(false);
                        if (_keyUI==null)
                        {
                            Debug.LogError($"Not Found KeyUI.");
                            return;
                        }
                        PlayActiveKeyUIAnimation(colliderObj.transform.position);
                    }
                }).AddTo(_playerView);
        }

        /// <summary>
        /// 鍵がUIの所まで移動するエフェクトアニメーション
        /// </summary>
        /// <param name="gotKeyPos">鍵を入手した場所</param>
        private void PlayActiveKeyUIAnimation(Vector3 gotKeyPos)
        {
            Transform effectTransform = _playerConstEntity.TrailEffect.transform;
            effectTransform.position = gotKeyPos;
            Transform destinationTransform = _keyUI.transform;
            
            
            _playerConstEntity.TrailEffect.SetActive(true);
            SEManager.Instance.Play(SEPath.WARP,pitch:1.5f,isLoop:true);
            DOTween.To(() => 0f, value =>
                {
                    //MEMO: 実行中に目的地が移動する場合があるので、DOMoveではなくLerpで処理
                    effectTransform.position = Vector3.Lerp(gotKeyPos, destinationTransform.position, value);
                }, 1f, _playerConstEntity.ActiveKeyEffectDuration)
                .OnComplete(() =>
                {
                    SEManager.Instance.Stop(SEPath.WARP);
                    SEManager.Instance.Play(SEPath.KEY_GET, volumeRate: 1.5f);
                    _keyUI.SetActive(true);
                    _playerConstEntity.TrailEffect.SetActive(false);
                });
        }
    }
}