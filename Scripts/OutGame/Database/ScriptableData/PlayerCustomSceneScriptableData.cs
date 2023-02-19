using MyApplication;
using OutGame.PlayerCustom.Installer;
using OutGame.PlayerCustom.View;
using UnityEngine;

namespace OutGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "PlayerCustomSceneScriptableData", menuName = "ScriptableObjects/PlayerCustomSceneScriptableData")]
    public class PlayerCustomSceneScriptableData:ScriptableObject
    {
        public float PanelPopUpDuration => panelPopUpDuration;
        public float PanelPopDownDuration => panelPopDownDuration;
        
        public CharacterSelectCursorInstaller CharacterSelectCursorInstaller=>characterSelectCursorInstaller;
        public CharacterSelectCursorView CharacterSelectCursorView => characterSelectCursorView;
        public SquareRange CharacterSelectCursorRange => characterSelectCursorRange;
        
        [SerializeField] private float panelPopUpDuration;
        [SerializeField] private float panelPopDownDuration;
        [SerializeField] private CharacterSelectCursorInstaller characterSelectCursorInstaller;
        [SerializeField] private CharacterSelectCursorView characterSelectCursorView;
        [SerializeField] private SquareRange characterSelectCursorRange;
        
        public PlayerCustomSceneScriptableData Clone()
        {
            return MemberwiseClone() as PlayerCustomSceneScriptableData;
        }
    }
}