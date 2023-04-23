using UnityEngine;

namespace InGame.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "SceneLoadData", menuName = "ScriptableObjects/SceneLoadData")]
    public class SceneLoadData:ScriptableObject
    {
        public float BlackScreenFadeInDuration => blackScreenFadeInDuration;
        public float BlackScreenFadeOutDuration => blackScreenFadeOutDuration;
        public float LoadFadeInDuration => loadFadeInDuration;
        public float LoadFadeOutDuration => loadFadeOutDuration;
        public float LoadBackgroundFadeOutDuration => loadBackgroundFadeOutDuration;
        public float WaveScrollSpeed => waveScrollSpeed;
        public float BackgroundScrollSpeed => backgroundScrollSpeed;
        public float GameOverFadeInDuration => gameOverFadeInDuration;
        public float GameOverBGMFadeOutDuration => gameOverBGMFadeOutDuration;


        //[SerializeField] private float longLoaderXSize=18;
        [SerializeField,Header("黒背景のフェードインにかかる時間")] private float blackScreenFadeInDuration=1.5f;
        [SerializeField,Header("黒背景のフェードアウトにかかる時間")] private float blackScreenFadeOutDuration=1.5f;
        
        [SerializeField,Header("ロード画面のフェードインにかかる時間")] private float loadFadeInDuration=0.5f;
        [SerializeField,Header("ロード画面のフェードアウトにかかる時間")] private float loadFadeOutDuration=0.5f;
        [SerializeField,Header("ロード背景のフェードアウトにかかる時間")] private float loadBackgroundFadeOutDuration=1.5f;
        [SerializeField] private float waveScrollSpeed = 1;
        [SerializeField] private float backgroundScrollSpeed = 1;
        [SerializeField] private float gameOverFadeInDuration = 3;
        [SerializeField] private float gameOverBGMFadeOutDuration = 1;

    }
}