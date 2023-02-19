using InGame.Colate.View;
using UnityEngine;

namespace InGame.Colate.Installer
{
    public class ViewGenerator:MonoBehaviour
    {
        public ColateView GenerateColate(ColateView colateView)
        {
            return Instantiate(colateView);
        }
        
        public ColateStatusView GenerateColateStatusView(ColateStatusView colateStatusView,Transform parent,Vector2 pos)
        {
            ColateStatusView instance = Instantiate(colateStatusView,parent);
            instance.GetComponent<RectTransform>().anchoredPosition = pos;
            return instance;
        }
    }
}