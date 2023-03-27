using OutGame.PlayerCustom.View;
using UnityEngine;

namespace OutGame.PlayerCustom.Installer
{
    public class CharacterSelectCursorInstaller:MonoBehaviour
    {
        public CharacterSelectCursorView Install(CharacterSelectCursorView prefab,int playerNum,Transform parent,float addPosition)
        {
            var instance = Instantiate(prefab, parent);
            instance.Init();
            instance.SetRectAnchorPosition(prefab.GetComponent<RectTransform>().position+new Vector3(0,0,addPosition));
            instance.ChangePlayerNumText(playerNum);
            return instance;
        }
    }
}