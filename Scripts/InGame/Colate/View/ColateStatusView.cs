using UnityEngine;
using UnityEngine.UI;

namespace InGame.Colate.View
{
    public class ColateStatusView:MonoBehaviour
    {
        [SerializeField] private GameObject characterSelectPanel;
        [SerializeField] private GameObject icon;
        [SerializeField] private Slider hpSlider;
        
        public void Init(int maxHp)
        {
            hpSlider.maxValue = maxHp;
        }
    }
}