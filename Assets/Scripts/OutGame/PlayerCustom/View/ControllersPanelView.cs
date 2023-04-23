using System;
using System.Linq;
using MyApplication;
using OutGame.PlayerCustom.Data;
using UnityEngine;
using UnityEngine.UI;

namespace OutGame.PlayerCustom.View
{
    public class ControllersPanelView:MonoBehaviour
    {
        public GameObject PanelObj=>panelObj;
        
        private Color notRegisteredColor=>Color.HSVToRGB(188/360f, 15/100f, 60/100f);
        private Color registeredColor=>Color.HSVToRGB(0, 0, 100/100f);
        
        [SerializeField] private GameObject panelObj;
        [SerializeField] private EachPlayerControllers[] allPlayerControllers;
        
        
        public void Init()
        {
            PanelObj.SetActive(false);
            PanelObj.transform.localScale = Vector3.zero;
            foreach (var eachPlayerControllers in allPlayerControllers)
            {
                eachPlayerControllers.ImageObjectsParent.SetActive(false);
                eachPlayerControllers.ChangeAnimator.SetBool(UIAnimatorParameter.Change,false);
            }
        }

        public void InitByPlayerCount(int maxPlayerNum)
        {
            for (int i = 0; i < maxPlayerNum; i++)
            {
                allPlayerControllers[i].ChangeAnimator.enabled = true;
                allPlayerControllers[i].ImageObjectsParent.SetActive(true);
            }
        }
        
        public void StartUseControllersChangeAnimation(int maxPlayerNum)
        {
            for (int i = 0; i < maxPlayerNum; i++)
            {
                allPlayerControllers[i].ChangeAnimator.SetBool(UIAnimatorParameter.Change,true);
            }
        }
        
        public void ResetControllerImages(int maxPlayerNum)
        {
            for (int i = 0; i < maxPlayerNum; i++)
            {
                allPlayerControllers[i].ImageObjectsParent.SetActive(true);
                allPlayerControllers[i].ChangeAnimator.SetBool(UIAnimatorParameter.Change,false);
                foreach (var controller in allPlayerControllers[i].ControllerImageObjects)
                {
                    controller.ImageObj.GetComponent<Image>().color = notRegisteredColor;
                    controller.ImageObj.SetActive(true);
                }
            }
        }
        
        /// <summary>
        /// ImageのIndexは0から始まるので注意
        /// </summary>
        /// <param name="type"></param>
        /// <param name="controllerIndexNum"></param>
        public void PaintGamePadImage(MyInputDeviceType type, int controllerIndexNum)
        {
            var eachPlayerControllers = allPlayerControllers[controllerIndexNum];
            var image = eachPlayerControllers.ControllerImageObjects.FirstOrDefault(data => data.Type == type)?.ImageObj;
            if (image==null)
            {
                Debug.LogError($"Couldn`t Find ControllerImage");
                return;
            }

            eachPlayerControllers.ChangeAnimator.enabled = false;
            foreach (var controller in eachPlayerControllers.ControllerImageObjects)
            {
                if (controller.ImageObj==image)
                {
                    image.GetComponent<Image>().color = registeredColor;
                    continue;
                }
                controller.ImageObj.SetActive(false);
            }
        }

        public void ResetPaintImage(int controllerIndexNum)
        {
            Debug.Log($"Cancel.  playerNum:{controllerIndexNum}");
            allPlayerControllers[controllerIndexNum].ChangeAnimator.SetBool(UIAnimatorParameter.Change,true);
            foreach (var controller in allPlayerControllers[controllerIndexNum].ControllerImageObjects)
            {
                controller.ImageObj.GetComponent<Image>().color = notRegisteredColor;
                controller.ImageObj.SetActive(true);;
            }
        }

        
    }
}