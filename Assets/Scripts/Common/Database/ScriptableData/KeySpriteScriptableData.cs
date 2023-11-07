using System.Collections.Generic;
using System.Linq;
using MyApplication;
using UnityEngine;

namespace Common.Database.ScriptableData
{
    [CreateAssetMenu(fileName = "KeySpriteScriptableData", menuName = "ScriptableObjects/KeySpriteScriptableData")]
    public class KeySpriteScriptableData:ScriptableObject
    {
        [SerializeField] private List<EachDeviceKeyAnimationData> eachDeviceKeySpriteData;

        public Animation GetAnimation(Key key,MyInputDeviceType deviceType)
        {
            Animation sprite=eachDeviceKeySpriteData.FirstOrDefault(data => data.Device==deviceType)?.Sprites
                .FirstOrDefault(d=>d.Key==key)?.Sprite;

            if (sprite==null)
            {
                Debug.LogError($"Not Found Sprite. Key:{key}, MyInputDeviceType:{deviceType}");
            }

            return sprite;
        }
    }
}