using MyApplication;
using UnityEngine;

namespace OutGame.PlayerCustom.Data
{
    public class EachPlayerControllers:MonoBehaviour
    {
        public Animator ChangeAnimator=>changeAnimator;
        public GameObject ImageObjectsParent=>imageObjectsParent;
        public ControllerImage[] ControllerImageObjects=>controllerImageObjects;
            
        [SerializeField] private Animator changeAnimator;
        [SerializeField] private GameObject imageObjectsParent;
        [SerializeField] private ControllerImage[] controllerImageObjects;
    }
}