using System;
using UnityEngine;
using UnityEngine.UI;

namespace OutGame.PlayerCustom.View
{
    public class PlayerCountView:MonoBehaviour
    {
        public GameObject PanelObj => panelObj;
        
        [SerializeField] private Vector2 defaultScale=Vector2.one;
        [SerializeField] private Vector2 popUpScale = Vector2.one * 1.2f;
        [SerializeField] private Color defaultImageColor = Color.white;
        [SerializeField] private Color popUpImageColor = Color.white;
        [SerializeField] private Color defaultTextColor = Color.white;
        [SerializeField] private Color popUpTextColor = Color.white;
        [SerializeField] private GameObject panelObj;
        [SerializeField] private Image oneButtonImage;
        [SerializeField] private Image twoButtonImage;
        [SerializeField] private Image threeButtonImage;
        [SerializeField] private Image fourButtonImage;
        [SerializeField] private Text oneButtonText;
        [SerializeField] private Text twoButtonText;
        [SerializeField] private Text threeButtonText;
        [SerializeField] private Text fourButtonText;
        
        
        public void Init()
        {
            PanelObj.SetActive(false);
            PanelObj.transform.localScale = Vector3.zero;
        }
        
        public Transform GetViewTransform(int num)
        {
            return num switch
            {
                1 => oneButtonImage.transform,
                2 => twoButtonImage.transform,
                3 => threeButtonImage.transform,
                4 => fourButtonImage.transform,
                _ => throw new ArgumentOutOfRangeException(nameof(num), num, null)
            };
        }
        
        public Image GetButtonImage(int num)
        {
            return num switch
            {
                1 => oneButtonImage,
                2 => twoButtonImage,
                3 => threeButtonImage,
                4 => fourButtonImage,
                _ => throw new ArgumentOutOfRangeException(nameof(num), num, null)
            };
        }
        
        public Text GetButtonText(int num)
        {
            return num switch
            {
                1 => oneButtonText,
                2 => twoButtonText,
                3 => threeButtonText,
                4 => fourButtonText,
                _ => throw new ArgumentOutOfRangeException(nameof(num), num, null)
            };
        }

        public void ResetAllButton()
        {
            oneButtonImage.transform.localScale = defaultScale;
            twoButtonImage.transform.localScale = defaultScale;
            threeButtonImage.transform.localScale = defaultScale;
            fourButtonImage.transform.localScale = defaultScale;
            
            oneButtonImage.color = defaultImageColor;
            twoButtonImage.color = defaultImageColor;
            threeButtonImage.color = defaultImageColor;
            fourButtonImage.color= defaultImageColor;
            
            oneButtonText.color = defaultTextColor;
            twoButtonText.color = defaultTextColor;
            threeButtonText.color = defaultTextColor;
            fourButtonText.color= defaultTextColor;
        }

        public void PopUp(int num)
        {
            Image buttonSprite = GetButtonImage(num);
            buttonSprite.transform.localScale = popUpScale;
            buttonSprite.color = popUpImageColor;
            GetButtonText(num).color = popUpTextColor;
        }

        public void PopDown(int num)
        {
            Image buttonSprite = GetButtonImage(num);
            buttonSprite.transform.localScale = defaultScale;
            buttonSprite.color = defaultImageColor;
            GetButtonText(num).color = defaultTextColor;
        }

    }
}