using MyApplication;
using UnityEngine;

namespace InGame.Player.View
{
    [RequireComponent(typeof(Animator))]
    public class ActionKeyView:MonoBehaviour
    {
        private Animator _keyAnimator;
        private MyInputDeviceType _deviceType;

        public void Init(MyInputDeviceType deviceType)
        {
            _keyAnimator = GetComponent<Animator>();
            _deviceType = deviceType;
            SetBoolAnimation(PlayerKeyAnimationName.GetDeviceName(_deviceType),true);
        }

        public void ReInit()
        {
            SetBoolAnimation(PlayerKeyAnimationName.GetDeviceName(_deviceType),true);
        }

        public void SetBoolAnimation(string name,bool on)
        {
            _keyAnimator.SetBool(name,on);
        }
        
        public void SetRotation(Quaternion newRot)
        {
            transform.localRotation = newRot;
        }

        public void Rebind()
        {
            _keyAnimator.Rebind();
        }
    }
}