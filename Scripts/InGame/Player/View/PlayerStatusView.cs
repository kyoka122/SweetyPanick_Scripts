using MyApplication;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace InGame.Player.View
{
    public class PlayerStatusView:MonoBehaviour
    {
        [SerializeField]private GameObject characterSelectPanel;
        [SerializeField,EnumIndex(typeof(PlayableCharacterIndex))]private GameObject [] characterIcons;
        [SerializeField,EnumIndex(typeof(PlayableCharacterIndex))]private GameObject [] characterSelectorIcons;
        [SerializeField] private Slider hpSlider;
        
        public void Init(PlayableCharacterIndex character,int maxHp,int currentHp)
        {
            SetCharacterSelectPanel(false);
            ChangeCharacterIcon(character);
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
        }
        
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
        
        public bool IsActiveSelectPanel => characterSelectPanel.activeSelf;
        public void SetCharacterSelectPanel(bool active)
        {
            characterSelectPanel.SetActive(active);
        }

        public void ChangeCharacterIcon(PlayableCharacterIndex character)
        {
            foreach (var icon in characterIcons)
            {
                icon.SetActive(false);
            }
            characterIcons[(int)character].SetActive(true);
        }
        
        public void SelectNextCharacter(PlayableCharacterIndex character)
        {
            foreach (var selectorIcon in characterSelectorIcons)
            {
                selectorIcon.SetActive(false);
            }
            characterSelectorIcons[(int)character].SetActive(true);
        
        }

        public void SetHpValue(int currentHp)
        {
            hpSlider.value = currentHp;
        }
    }
}