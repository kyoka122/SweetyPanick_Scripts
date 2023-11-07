using System;
using UnityEngine;

namespace Utility
{
    public class CutOffTransition
    {
        private static readonly int PropertyX = Shader.PropertyToID("_AlphaX");
        private static readonly int PropertyY = Shader.PropertyToID("_AlphaY");

        private readonly Material _material;

        public CutOffTransition(Material material,float initX,float initY)
        {
            _material = material;
            SetCutOffX(initX);
            SetCutOffY(initY);
        }

        /// <summary>
        /// X方向にフェードするCutOffの値をセットする。
        /// </summary>
        public void SetCutOffX(float alphaX)
        {
            _material.SetFloat(PropertyX,alphaX);
        }
        
        /// <summary>
        /// Y方向にフェードするCutOffの値をセットする。
        /// </summary>
        public void SetCutOffY(float alphaY)
        {
            _material.SetFloat(PropertyY,alphaY);
        }
        
        /// <summary>
        /// オブジェクトの破棄と同時にこのマテリアルも破棄する
        /// </summary>
        /// <returns></returns>
        public Material GetDisposeMaterial()
        {
            return _material!=null ? _material : null;
        }
    }
}