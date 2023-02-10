using MyApplication;
using UnityEngine;
using UnityEngine.UI;

namespace OutGame.PlayerCustom.View
{
    public class CharacterIconView:MonoBehaviour
    {
        public readonly Color offColor =Color.HSVToRGB(320/360f,15/100f,60/100f);
        public readonly Color onSelectableColor = Color.HSVToRGB(0, 0, 1);
        public PlayableCharacter Type => type;
        [SerializeField] private PlayableCharacter type;

        public Rect GetRect()
        {
            return GetComponent<RectTransform>().rect;
        }
        
        public void OnSelectableIcon()
        {
            //GetComponent<Image>()i
        }
        
        public void OffSelectableIcon()
        {
            
        }
        
        
    }
}