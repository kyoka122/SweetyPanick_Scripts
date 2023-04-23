using System;
using UnityEngine;
using UnityEngine.UI;

namespace OutGame.PlayerCustom.View
{
    public class PlayerCountView:MonoBehaviour
    {
        public GameObject PanelObj => panelObj;
        
        [SerializeField] private GameObject panelObj;
        [SerializeField] private Transform oneButton;
        [SerializeField] private Transform twoButton;
        [SerializeField] private Transform threeButton;
        [SerializeField] private Transform fourButton;

        public void Init()
        {
            PanelObj.SetActive(false);
            PanelObj.transform.localScale = Vector3.zero;
        }
        
        public Transform GetViewTransform(int num)
        {
            return num switch
            {
                1 => oneButton,
                2 => twoButton,
                3 => threeButton,
                4 => fourButton,
                _ => throw new ArgumentOutOfRangeException(nameof(num), num, null)
            };
        }

        public void ResetAllButton()
        {
            oneButton.localScale = Vector2.one;
            twoButton.localScale = Vector2.one;
            threeButton.localScale = Vector3.one;
            fourButton.localScale= Vector3.one;
        }

    }
}