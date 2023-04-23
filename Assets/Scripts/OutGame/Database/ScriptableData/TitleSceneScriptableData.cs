using UnityEngine;

namespace OutGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "TitleSceneScriptableData", menuName = "ScriptableObjects/TitleSceneScriptableData")]
    public class TitleSceneScriptableData:ScriptableObject
    {
        public float CreditPopUpDuration => creditPopUpDuration;
        public float CreditPopDownDuration => creditPopDownDuration;
        public float CreditScrollFactor => creditScrollFactor;

        [SerializeField] private float creditPopUpDuration = 1f;
        [SerializeField] private float creditPopDownDuration = 0.8f;
        [SerializeField] private float creditScrollFactor = 0.001f;
    }
}