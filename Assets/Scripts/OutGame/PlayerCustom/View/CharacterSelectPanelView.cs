using System;
using MyApplication;
using UnityEngine;

namespace OutGame.PlayerCustom.View
{
    public class CharacterSelectPanelView:MonoBehaviour
    {
        //public RectTransform UpPanelRectTransform => upPanelRectTransform;
        //public RectTransform DownPanelRectTransform => downPanelRectTransform;
        //public Vector2 UpTargetExitEndPos=>upTargetExitEndPos;
        //public Vector2 DownTargetExitEndPos=>downTargetExitEndPos;
        //public Vector2 UpTargetEnterEndPos=>upTargetEnterEndPos;
        //public Vector2 DownTargetEnterEndPos=>downTargetEnterEndPos;

        //[SerializeField] private RectTransform upPanelRectTransform;
        //[SerializeField] private RectTransform downPanelRectTransform;
        //[SerializeField] private Vector2 upTargetExitEndPos;
        //[SerializeField] private Vector2 downTargetExitEndPos;
        //[SerializeField] private Vector2 upTargetEnterEndPos;
        //[SerializeField] private Vector2 downTargetEnterEndPos;
        //[SerializeField] private Animator keyConfigAnimator;
        public GameObject PanelObj => panelObj;
        [SerializeField] private GameObject panelObj;
        [SerializeField] private Transform firstInitIconPos;
        [SerializeField] private Transform secondInitIconPos;
        [SerializeField] private Transform thirdInitIconPos;
        [SerializeField] private Transform fourthInitIconPos;
        
        public void Init()
        {
            PanelObj.SetActive(false);
            PanelObj.transform.localScale = Vector3.zero;
            //keyConfigAnimator.enabled = false;
        }

        public Vector2 GetCursorPos(int num)
        {
            return num switch
            {
                1 => firstInitIconPos.localPosition,
                2 => secondInitIconPos.localPosition,
                3 => thirdInitIconPos.localPosition,
                4 => fourthInitIconPos.localPosition,
                _ => throw new ArgumentOutOfRangeException(nameof(num), num, null)
            };
        }
        
        /*public void ResetAnimator()
        {
            keyConfigAnimator.Rebind();
        }
        
        public void SetEnableAnimator(bool enable)
        {
            keyConfigAnimator.enabled = enable;
        }
        
        /// <summary>
        /// アクティブに設定すると、フェード有りアニメーションが流れるようになる
        /// </summary>
        /// <param name="active"></param>
        public void SetChangeAnimationParameter(bool active)
        {
            keyConfigAnimator.SetBool(UIAnimatorParameter.OnChangeAnimation,active);
        }

        public void SetBoolKeyConfigAnimator(string paramName,bool on)
        {
            keyConfigAnimator.SetBool(paramName,on);
        }
        
        public void InActiveAllKeyConfigAnimatorParameter()
        {
            keyConfigAnimator.SetBool(UIAnimatorParameter.UseJoycon,false);
            keyConfigAnimator.SetBool(UIAnimatorParameter.UseProcon,false);
            keyConfigAnimator.SetBool(UIAnimatorParameter.UseXBox,false);
            keyConfigAnimator.SetBool(UIAnimatorParameter.UseKeyboard,false);
        }*/
    }
}