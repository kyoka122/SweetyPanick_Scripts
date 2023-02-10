using Audio.ScriptableData;
using KanKikuchi.AudioManager;

namespace Audio
{
    public static class AudioVolumeManager
    {
        private static AudioScriptableData _audioScriptableData;
        
        public static void Init(AudioScriptableData audioScriptableData)
        {
            _audioScriptableData = audioScriptableData;
            BGMManager.Instance.ChangeBaseVolume(_audioScriptableData.InitBGMVolume);
            SEManager.Instance.ChangeBaseVolume(_audioScriptableData.InitSEVolume);
        }
        
    }
}