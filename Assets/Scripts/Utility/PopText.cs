using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Utility
{
    public class PopText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void SetOutLineColor(Color color)
        {
            text.outlineColor = color;
        }
        
        public UniTask Pop(string textString,float distance,float enterDuration,float exitDuration)
        {
            text.text = textString;
            return DOTween.Sequence()
                .Append(text.DOFade(1, enterDuration))
                .Join(transform.DOMoveY(text.transform.position.y + distance, exitDuration + exitDuration))
                .Append(text.DOFade(0, exitDuration))
                .SetEase(Ease.InQuad)
                .ToUniTask();
        }
        
        public UniTask PopRumble(string textString,float distance,float enterDuration,float exitDuration,float rumblePower)
        {
            text.text = textString;
            return DOTween.Sequence()
                .Append(text.DOFade(1, enterDuration))
                .Join(transform.DOMoveY(text.transform.position.y + distance, exitDuration+exitDuration))
                .Join(transform.DOPunchPosition(Vector3.right*rumblePower, enterDuration+exitDuration))
                .Append(text.DOFade(0, exitDuration))
                .SetEase(Ease.InQuad)
                .ToUniTask();
        }
    }
}