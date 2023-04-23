using InGame.Player.View;
using Unity.VisualScripting;
using UnityEngine;

namespace InGame.Player.Installer
{
    public class ViewGenerator:MonoBehaviour
    {
        public CandyView GenerateCandy(CandyView candyView)
        {
            return Instantiate(candyView);
        }

        public MashView GenerateMash(MashView mashView)
        {
            return Instantiate(mashView);
        }

        public FuView GenerateFu(FuView fuView)
        {
            return Instantiate(fuView);
        }

        public KureView GenerateKure(KureView kureView)
        {
            return Instantiate(kureView);
        }
        
        public PlayerStatusView GeneratePlayerStatusView(PlayerStatusView playerStatusView,Transform parent,Vector2 pos)
        {
            PlayerStatusView instance = Instantiate(playerStatusView,parent);
            instance.GetComponent<RectTransform>().anchoredPosition = pos;
            return instance;
        }
        
    }
}