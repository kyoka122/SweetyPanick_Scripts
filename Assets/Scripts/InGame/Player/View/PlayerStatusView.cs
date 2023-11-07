using System;
using MyApplication;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace InGame.Player.View
{
    public class PlayerStatusView:MonoBehaviour
    {
        //[SerializeField]private GameObject characterSelectPanel;
        [SerializeField,EnumIndex(typeof(PlayableCharacterIndex))]private Image [] characterIconSprites;
        [SerializeField,EnumIndex(typeof(PlayableCharacterIndex))]private GameObject [] characterNameObjects;
        [SerializeField,EnumIndex(typeof(PlayableCharacterIndex))]private Sprite [] normalCharacterIconSprites;
        [SerializeField,EnumIndex(typeof(PlayableCharacterIndex))]private Sprite [] deathCharacterIconSprites;
        //[SerializeField,EnumIndex(typeof(PlayableCharacterIndex))]private GameObject [] characterSelectorIcons;
        [SerializeField] private Color characterDeathColor=Color.white;
        [SerializeField] private Color characterLivingColor=Color.white;

        
        //[SerializeField] private Slider hpSlider;
        [SerializeField] private Image hpSliderImage;
        [SerializeField] private Image hpSliderShadowImage;

        private PlayableCharacterIndex _currentStatusIndex;
        private CutOffTransition _sliderCutOffTransition;
        private RectTransform _hpSliderRectTransform;
        
        public void Init(PlayableCharacterIndex character,int maxHp,float currentHp,int healHp,bool isUsed,bool isDead)
        {
            _currentStatusIndex = character;
            _hpSliderRectTransform = hpSliderImage.GetComponent<RectTransform>();
            
            hpSliderImage.material = new Material(hpSliderImage.material);
            _sliderCutOffTransition = new CutOffTransition(hpSliderImage.material,1,1);
            hpSliderShadowImage.material = new Material(hpSliderShadowImage.material);
            new CutOffTransition(hpSliderShadowImage.material,(float)healHp/maxHp,1);
            
            SetHpValue(currentHp/maxHp);
            SetCharacterSelectPanel(false);
            
            ChangeCharacterIcon(_currentStatusIndex);
            ChangeCharacterNameSprite(_currentStatusIndex);
            SetActiveHpBarShadow(false);
            
            SetSpritesByCharacterState(isDead ? CharacterHpFaceSpriteType.Death : CharacterHpFaceSpriteType.Normal);

            if (!isUsed)
            {
                gameObject.SetActive(false);
            }
        }
        
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
        //必要ないためコメントアウト
        //public bool IsActiveSelectPanel => characterSelectPanel.activeSelf;
        
        /// <summary>
        /// 必要ないため処理をコメントアウト済み
        /// </summary>
        /// <param name="active"></param>
        public void SetCharacterSelectPanel(bool active)
        {
           // characterSelectPanel.SetActive(active);
        }

        public void ChangeCharacterIcon(PlayableCharacterIndex character)
        {
            foreach (var icon in characterIconSprites)
            {
                icon.gameObject.SetActive(false);
            }
            characterIconSprites[(int)character].gameObject.SetActive(true);
        }

        public void SetActiveHpBarShadow(bool on)
        {
            hpSliderShadowImage.gameObject.SetActive(on);
        }
        
        public void ChangeCharacterNameSprite(PlayableCharacterIndex character)
        {
            foreach (var characterNameObj in characterNameObjects)
            {
                characterNameObj.SetActive(false);
            }
            characterNameObjects[(int)character].SetActive(true);
        }
        
        /// <summary>
        /// 必要ないため処理をコメントアウト済み
        /// </summary>
        public void SelectNextCharacter(PlayableCharacterIndex character)
        {
            // foreach (var selectorIcon in characterSelectorIcons)
            // {
            //     selectorIcon.SetActive(false);
            // }
            // characterSelectorIcons[(int)character].SetActive(true);
        
        }

        public void SetHpValue(float currentHpRate)
        {
            _sliderCutOffTransition.SetCutOffX(currentHpRate);
        }

        public Vector3[] GetHpSliderEdge()
        {
            var corners = new Vector3[4];
            _hpSliderRectTransform.GetWorldCorners(corners);
            return corners;
        }

        public void SetSpritesByCharacterState(CharacterHpFaceSpriteType spriteType)
        {
            //Debug.Log($"type:{_currentStatusIndex},spriteType:{spriteType}");
            Image characterIconImage = characterIconSprites[(int) _currentStatusIndex];
            switch (spriteType)
            {
                case CharacterHpFaceSpriteType.Normal:
                    characterIconImage.sprite = normalCharacterIconSprites[(int) _currentStatusIndex];
                    characterIconImage.color = characterLivingColor;
                    hpSliderImage.color = characterLivingColor;
                    break;
                case CharacterHpFaceSpriteType.Death:
                    characterIconImage.sprite = deathCharacterIconSprites[(int) _currentStatusIndex];
                    characterIconImage.color = characterDeathColor;
                    hpSliderImage.color = characterDeathColor;
                    break;
                default:
                    Debug.LogError($"CharacterIconType Is Not Found");
                    return;
            }
        }

        private void OnDestroy()
        {
            Destroy(hpSliderShadowImage.material);
            Destroy(hpSliderImage.material);
        }
    }
}