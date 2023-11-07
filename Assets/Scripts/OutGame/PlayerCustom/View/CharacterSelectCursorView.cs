using System;
using MyApplication;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OutGame.PlayerCustom.View
{
    public class CharacterSelectCursorView:MonoBehaviour
    {
        public UISelectState selectType { get; private set; }
        
        [SerializeField] Color selectingColor = new(1,1,1);
        [SerializeField] Color selectedColor = new Color(102/255f, 102/255f, 102 / 255f);
        [SerializeField] Color overlapColor = new Color(212/255f, 193/255f, 205 / 255f);
        
        [SerializeField] private TextMeshProUGUI playerNumText;
        [SerializeField] private Image image;

        private RectTransform _rectTransform;
        private int _playerNum;
        
        public void Init()
        {
            _rectTransform = GetComponent<RectTransform>();
            Debug.Log($"GetRectTransform");
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

        public void SetType(UISelectState state)
        {
            selectType = state;
            image.color = state switch
            {
                UISelectState.None => selectingColor,
                UISelectState.Overlap => overlapColor,
                UISelectState.Selected => selectedColor,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }
        
        public void DestroyObj()
        {
            Destroy(gameObject);
        }
    }
}