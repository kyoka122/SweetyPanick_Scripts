using System.Collections.Generic;
using Unity.VisualScripting;
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
        [SerializeField] private List<GameObject> joyconImageObjects;
        
        public void ResetControllerImages(int maxPlayerNum)
        {
            /*foreach (var joyconImageObject in joyconImageObjects)
            {
                Debug.Log($"Off!: {joyconImageObject}");
                joyconImageObject.SetActive(false);
            }*/
            
            for (int i = 0; i<joyconImageObjects.Count; i++)
            {
                if (i < maxPlayerNum*2)
                {
                    joyconImageObjects[i].SetActive(true);
                    joyconImageObjects[i].GetComponent<Image>().color = notRegisteredColor;
                    continue;
                }
                joyconImageObjects[i].SetActive(false);
            }
        }

        public void PaintImage(int joyconNum)
        {
            joyconImageObjects[joyconNum].GetComponent<Image>().color = registeredColor;
        }
        
        public void ResetPaintImage(int joyconNum)
        {
            Debug.Log($"Cancel:{joyconImageObjects[joyconNum]}");
            joyconImageObjects[joyconNum].GetComponent<Image>().color = notRegisteredColor;
        }
        
    }
}