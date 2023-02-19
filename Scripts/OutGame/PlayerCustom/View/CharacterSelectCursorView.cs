using MyApplication;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OutGame.PlayerCustom.View
{
    public class CharacterSelectCursorView:MonoBehaviour
    {
        public bool isSelected { get; private set; }
        
        private readonly Color selectingColor = new(1,1,1);
        private readonly Color selectedColor = new Color(102/255f, 102/255f, 102 / 255f);
        
        [SerializeField] private TextMeshProUGUI playerNumText;
        [SerializeField] private Image image;

        private RectTransform _rectTransform;
        
        public void Init()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        
        public void ChangePlayerNumText(int playerNum)
        {
            playerNumText.text = playerNum+"P";
        }
        
        public void SetRectAnchorPosition(Vector2 pos)
        {
            _rectTransform.anchoredPosition = pos;
        }

        public Vector2 GetRectAnchorPosition()
        {
            return _rectTransform.anchoredPosition;
        }
        
        public Vector2 GetRectPos()
        {
            return _rectTransform.position;
        }

        public void SetSelected(bool selected)
        {
            isSelected = selected;
        }
        
        public void OffSelectableIcon()
        {
            image.color = selectedColor;
        }

        public void OnSelectableIcon()
        {
            image.color = selectingColor;
        }
    }
}