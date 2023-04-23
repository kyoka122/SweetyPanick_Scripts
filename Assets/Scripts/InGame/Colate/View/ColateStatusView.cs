using UnityEngine;
using UnityEngine.UI;

namespace InGame.Colate.View
{
    public class ColateStatusView:MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;
        
        public void Init(int maxHp)
        {
            hpSlider.maxValue = maxHp;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void SetHpCalue(int currentHp)
        {
            hpSlider.value = currentHp;
        }
    }
}