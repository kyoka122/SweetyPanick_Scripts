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
        public float CursorInterval => cursorInterval;
        
        public CharacterSelectCursorInstaller CharacterSelectCursorInstaller=>characterSelectCursorInstaller;
        public CharacterSelectCursorView CharacterSelectCursorView => characterSelectCursorView;
        public SquareRange CharacterSelectCursorRange => characterSelectCursorRange;
        public Vector2 CursorRayRadius => cursorRayRadius;

        [SerializeField] private float panelPopUpDuration;
        [SerializeField] private float panelPopDownDuration;
        [SerializeField] private float cursorInterval = 200;
        [SerializeField] private Vector2 cursorRayRadius = new(0.5f, 0.5f);
        [SerializeField] private CharacterSelectCursorInstaller characterSelectCursorInstaller;
        [SerializeField] private CharacterSelectCursorView characterSelectCursorView;
        [SerializeField] private SquareRange characterSelectCursorRange;
        
        public PlayerCustomSceneScriptableData Clone()
        {
            return MemberwiseClone() as PlayerCustomSceneScriptableData;
        }
    }
}