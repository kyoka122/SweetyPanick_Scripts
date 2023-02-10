using UnityEngine;

namespace OutGame.PlayerCustom.View
{
    public class CharacterSelectPanelView:MonoBehaviour
    {
        public RectTransform UpPanelRectTransform => upPanelRectTransform;
        public RectTransform DownPanelRectTransform => downPanelRectTransform;
        public Vector2 UpTargetExitEndPos=>upTargetExitEndPos;
        public Vector2 DownTargetExitEndPos=>downTargetExitEndPos;
        public Vector2 UpTargetEnterEndPos=>upTargetEnterEndPos;
        public Vector2 DownTargetEnterEndPos=>downTargetEnterEndPos;

        [SerializeField] private RectTransform upPanelRectTransform;
        [SerializeField] private RectTransform downPanelRectTransform;
        [SerializeField] private Vector2 upTargetExitEndPos;
        [SerializeField] private Vector2 downTargetExitEndPos;
        [SerializeField] private Vector2 upTargetEnterEndPos;
        [SerializeField] private Vector2 downTargetEnterEndPos;
    }
}