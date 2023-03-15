using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    /// <summary>
    /// Colorの同時フェードイン（Sequenceに追加して遅延実行することはできない、即時実行なら問題ない）、
    /// </summary>
    public static class DOTweenExtension
    {
        public static TweenerCore<Color, Color, ColorOptions> DOSameFades(this SpriteRenderer[] targets, float endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t=null;
            foreach (var target in targets)
            {
                t=target.DOFade(endValue, duration);
            }
            return t;
        }
        
        public static TweenerCore<Color, Color, ColorOptions> DOSameFades(this Image[] targets, float endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t=null;
            foreach (var target in targets)
            {
                t=target.DOFade(endValue, duration);
            }
            return t;
        }
        
        public static TweenerCore<Color, Color, ColorOptions> DOSameFades(this Material[] target, float endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t=null;
            foreach (var targetMaterial in target)
            {
                t = DOTween.ToAlpha(() => targetMaterial.color, x => targetMaterial.color = x, endValue, duration);
                t.SetTarget(target);
            }
            return t;
        }
        
        public static TweenerCore<Color, Color, ColorOptions> DOSameFades(this MaskableGraphic[] target, float endValue, float duration)
        {
            Debug.Log($"targetColor: {target[0].color} endValue:{endValue}",target[0]);
            TweenerCore<Color, Color, ColorOptions> t=null;
            foreach (var targetMaterial in target)
            {
                t = DOTween.ToAlpha(() => targetMaterial.color, x => targetMaterial.color = x, endValue, duration);
                t.SetTarget(target);
                Debug.Log($"targetMaterial: {targetMaterial.color}",targetMaterial);
            }
            return t;
        }


        public static TweenerCore<Color, Color, ColorOptions> DOFades(this Dictionary<SpriteRenderer,float> targetAndEndValues, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t=null;
            foreach (var targetAndEndValue in targetAndEndValues)
            {
                t=targetAndEndValue.Key.DOFade(targetAndEndValue.Value, duration);
            }
            return t;
        }
        
        public static TweenerCore<Color, Color, ColorOptions> DOFades(this Dictionary<Material,float> targetAndEndValues, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t=null;

            foreach (var targetMaterial in targetAndEndValues)
            {
                t=targetMaterial.Key.DOFade(targetMaterial.Value, duration);
            }
            return t;
        }
    }
}