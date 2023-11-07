using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using InGame.Player.Entity;
using InGame.Player.View;
using UniRx;
using UnityEngine;

namespace InGame.Player.Logic
{
    public class PlayerHealLogic
    {
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;
        private readonly PlayerCommonInStageEntity _playerCommonInStageEntity;
        private readonly PlayerStatusView _playerStatusView;
        private TweenerCore<float, float, FloatOptions> _healTask;
        private ParticleSystem _hpBarHealEffectParticle;

        public PlayerHealLogic(PlayerConstEntity playerConstEntity,PlayerCommonUpdateableEntity playerCommonUpdateableEntity,
            PlayerCommonInStageEntity playerCommonInStageEntity,PlayerStatusView playerStatusView)
        {
            _playerConstEntity = playerConstEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
            _playerStatusView = playerStatusView;
            _playerCommonInStageEntity = playerCommonInStageEntity;
            RegisterObserver();
        }

        public void HealHp()
        {
            _playerCommonUpdateableEntity.SetCurrentHp(_playerConstEntity.HealValue);
        }

        public void TryKillHealTask()
        {
            _healTask?.Kill();
            _healTask = null;
            if (_hpBarHealEffectParticle!=null)
            {
                _playerCommonInStageEntity.healHpBarParticlePool.TryReleaseObject(_hpBarHealEffectParticle);
            }
        }

        public void TryReStartPlayHealTask()
        {
            Debug.Log($"type:{_playerConstEntity.Type},isDead:{_playerCommonUpdateableEntity.IsDead}");
            if (_playerCommonUpdateableEntity.IsDead)
            {
                PlayHealHpTask();
                Debug.Log($"ReHeal. type:{_playerConstEntity.Type}");
            }

            Debug.Log($"Not Is Dead. type:{_playerConstEntity.Type}");
            // else
            // {
            //     if (_playerCommonUpdateableEntity.CurrentHp==_playerConstEntity.MaxHp)
            //     {
            //         
            //     }
            //     _healTask = null;
            //     _playerCommonUpdateableEntity.SetHealedParameter();
            //     if ( _playerCommonInStageEntity.OnHadHealed())
            //     {
            //         
            //     }
            //     _playerCommonInStageEntity.OnHadHealed();
            //     _playerStatusView.SetActiveHpBarShadow(false);
            //     if (_hpBarHealEffectParticle!=null)
            //     {
            //         _playerCommonInStageEntity.healHpBarParticlePool.TryReleaseObject(_hpBarHealEffectParticle);
            //     }
            // }
        }

        private void RegisterObserver()
        {
            _playerCommonUpdateableEntity.OnIsInStage
                .Where(inStage => !inStage && _playerCommonUpdateableEntity.LivingPlayerCount >= 1)
                .Subscribe(_ =>
                {
                    if (_playerCommonUpdateableEntity.LivingPlayerCount>=1)
                    {
                        PlayHealHpTask();
                    }
                    
                }).AddTo(_playerStatusView);

        }

        private void PlayHealHpTask()
        {
            if (_healTask!=null)
            {
                return;
            }
            _playerStatusView.SetActiveHpBarShadow(true);
            _hpBarHealEffectParticle = _playerCommonInStageEntity.healHpBarParticlePool
                .GetObjectAndParentSet(_playerCommonUpdateableEntity.GetCanvasTransform,
                    _playerCommonUpdateableEntity.GetCanvasTransform.InverseTransformPoint(
                        GetHealingHpBarLocalRightEdge(0)), Quaternion.identity, Vector3.one);
            
            Debug.Log($"Start Heal!");
            float passedHealRate = _playerCommonUpdateableEntity.CurrentHp / _playerConstEntity.HealHpToRevive;
            Debug.Log($"passedHealRate:{passedHealRate}, duration:{_playerConstEntity.toReviveTime * (1f - passedHealRate)}");

            _healTask=DOTween.To(() => passedHealRate, value =>
                {
                    var newHp = value * _playerConstEntity.HealHpToRevive;
                    _playerCommonUpdateableEntity.SetCurrentHp(newHp);
                    float newHpRate = newHp / _playerConstEntity.MaxHp;
                    _playerStatusView.SetHpValue(newHpRate);
                    _hpBarHealEffectParticle.transform.localPosition=GetHealingHpBarLocalRightEdge(newHpRate);
                }, 1f, _playerConstEntity.toReviveTime * (1f - passedHealRate))
                .OnComplete(() =>
                {
                    Debug.Log($"Healed!");
                    _healTask = null;
                    _playerCommonInStageEntity.OnHadHealed();
                    _playerStatusView.SetActiveHpBarShadow(false);
                    _playerCommonInStageEntity.healHpBarParticlePool.TryReleaseObject(_hpBarHealEffectParticle);
                    _playerCommonUpdateableEntity.SetHealedParameter();
                });
        }

        private Vector2 GetHealingHpBarLocalRightEdge(float currentHpRate)
        {
            Vector3[] rect=_playerStatusView.GetHpSliderEdge();
            Vector3 worldRightEdge = new Vector3(rect[0].x + currentHpRate * (rect[3].x - rect[0].x),
                rect[0].y + (rect[1].y - rect[0].y) / 2, 0);
            return _playerCommonUpdateableEntity.GetCanvasTransform.InverseTransformPoint(worldRightEdge);
        }
    }
}