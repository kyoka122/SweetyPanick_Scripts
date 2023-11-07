using System;
using System.Collections.Generic;
using MyApplication;
using UnityEngine;
using UnityEngine.UI;

namespace OutGame.PlayerCustom.View
{
    public class CharacterIconView:MonoBehaviour
    {
        public PlayableCharacter Type => type;
        public Dictionary<int,UISelectState> selectTypes { get; private set; }
        
        [SerializeField] private PlayableCharacter type;
        [SerializeField] private Image iconImage;
        [SerializeField] private Color selectedColor =Color.HSVToRGB(320/360f,15/100f,60/100f);
        [SerializeField] private Color overlapColor =Color.HSVToRGB(320/360f,15/100f,60/100f);
        [SerializeField] private Color noneColor = Color.HSVToRGB(0, 0, 1);
        
        private int _overlapCursorCount;

        public void Init()
        {
            selectTypes = new Dictionary<int, UISelectState>();
            for (int i = 1; i < Enum.GetValues(typeof(PlayableCharacter)).Length; i++)
            {
                selectTypes.Add(i,UISelectState.None);
            }
        }
        
        public void Reset()
        {
            for (var i = 0; i < selectTypes.Count; i++)
            {
                selectTypes[i] = UISelectState.None;
            }
            iconImage.color = noneColor;
        }
        
        public Rect GetRect()
        {
            return GetComponent<RectTransform>().rect;
        }
        
        public void SetType(int playerNum,UISelectState state)
        {
            selectTypes[playerNum] = state;
        }
        
        public void SetColor()
        {
            if (selectTypes.ContainsValue(UISelectState.Selected))
            {
                iconImage.color = selectedColor;
                return;
            }
            if (selectTypes.ContainsValue(UISelectState.Overlap))
            {
                iconImage.color = overlapColor;
                return;
            }
            iconImage.color = noneColor;
        }


        public bool IsOverlap(int playerNum)
        {
            return selectTypes[playerNum]==UISelectState.Overlap;
        }

        public bool IsSelected()
        {
            return selectTypes.ContainsValue(UISelectState.Selected);
        }

        public void SetTypeAll(UISelectState state)
        {
            for (int i = 0; i < selectTypes.Count; i++)
            {
                selectTypes[i] = state;
            }
        }
    }
}