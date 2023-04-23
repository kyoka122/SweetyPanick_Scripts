using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace InGame.Player.View
{
    public class SpriteEffectView:MonoBehaviour
    {
        [SerializeField] private Vector2 effectEndSize;
        private bool _finish;
        
        public void Play(float duration)
        {
            var renderer = GetComponent<SpriteRenderer>();
            DOTween.Sequence().Append(transform.DOScale(effectEndSize, duration))
                .Join(renderer.DOFade(1f, duration).SetEase(Ease.OutExpo))
                .OnComplete(()=>Destroy(gameObject));
        }
    }
}