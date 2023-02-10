using UnityEngine;

namespace InGame.Database
{
    public class CharacterUpdateableInStageData
    {
        public bool isWarping;
        
        /// <summary>
        /// 0~1まで
        /// </summary>
        public float nearnessFromTargetView;
        public Transform transform;
        
        public CharacterUpdateableInStageData(Transform characterTransform)
        {
            isWarping = false;
            nearnessFromTargetView = 1;
            transform = characterTransform;
        }

        public CharacterUpdateableInStageData Clone()
        {
            return MemberwiseClone() as CharacterUpdateableInStageData;
        }
    }
}