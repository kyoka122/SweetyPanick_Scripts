using OutGame.PlayerCustom.View;
using UnityEngine;

namespace OutGame.PlayerCustom.Installer
{
    public class CharacterSelectCursorInstaller:MonoBehaviour
    {
        public CharacterSelectCursorView Install(CharacterSelectCursorView prefab,int playerNum,Transform parent,Vector2 position)
        {
            var instance = Instantiate(prefab, parent);
            instance.Init();
            instance.SetRectAnchorPosition(position);
            instance.ChangePlayerNumText(playerNum);
            return instance;
        }
    }
}