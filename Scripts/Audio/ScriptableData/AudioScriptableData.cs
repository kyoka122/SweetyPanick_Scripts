using UnityEngine;

namespace Audio.ScriptableData
{
    [CreateAssetMenu(fileName = "AudioScriptableData", menuName = "ScriptableObjects/AudioScriptableData")]
    public class AudioScriptableData:ScriptableObject
    {
        public float InitBGMVolume=>initBGMVolume;
        public float InitSEVolume=>initSEVolume;

        [SerializeField] private float initBGMVolume=0.5f;
        [SerializeField] private float initSEVolume=0.5f;
    }
}