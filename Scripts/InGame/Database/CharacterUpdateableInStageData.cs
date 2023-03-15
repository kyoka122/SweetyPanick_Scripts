using MyApplication;
using UnityEngine;

namespace InGame.Database
{
    //TODO: PlayerUpdateableDataにまとめる
    public struct CharacterUpdateableInStageData
    {
        public bool isWarping;
        public bool canTargetCamera;
        
        /// <summary>
        /// 0~1まで
        /// </summary>
        public float nearnessFromTargetView;
        public Transform transform;
        
        public CharacterUpdateableInStageData(Transform characterTransform)
        {
            isWarping = false;
            canTargetCamera = false;
            nearnessFromTargetView = 1;
            transform = characterTransform;
        }
    }
}