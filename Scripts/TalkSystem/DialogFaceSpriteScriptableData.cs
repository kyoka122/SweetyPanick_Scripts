using System.Linq;
using MyApplication;
using UnityEngine;

namespace TalkSystem
{
    [CreateAssetMenu(fileName = "DialogFaceSpriteScriptableData", menuName = "ScriptableObjects/DialogFaceSpriteScriptableData")]
    public class DialogFaceSpriteScriptableData:ScriptableObject
    {
        [SerializeField] private CandyFaceData[] candyFaces;
        [SerializeField] private FuFaceData[] fuFaces;
        [SerializeField] private MashFaceData[] mashFaces;
        [SerializeField] private KureFaceData[] kureFaces;
        [SerializeField] private QueenFaceData[] queenFaces;
        [SerializeField] private MobFaceData[] mobFaces;
        [SerializeField] private ColateFaceData[] colateFaces;

        public Sprite GetCandyFace(CandyFaceSpriteType type)
        {
            Sprite sprite = candyFaces.FirstOrDefault(face => face.Type == type)?.Sprite;
            if (sprite==null)
            {
                Debug.LogError($"Not Found CandyFaceSprite. type:{type}");
            }
            return sprite;
        }
        
        public Sprite GetFuFace(FuFaceSpriteType type)
        {
            Sprite sprite = fuFaces.FirstOrDefault(face => face.Type == type)?.Sprite;
            if (sprite==null)
            {
                Debug.LogError($"Not Found FuFaceSprite. type:{type}");
            }
            return sprite;
        }
        
        public Sprite GetMashFace(MashFaceSpriteType type)
        {
            Sprite sprite = mashFaces.FirstOrDefault(face => face.Type == type)?.Sprite;
            if (sprite==null)
            {
                Debug.LogError($"Not Found MashFaceSprite. type:{type}");
            }
            return sprite;
        }
        
        public Sprite GetKureFace(KureFaceSpriteType type)
        {
            Sprite sprite = kureFaces.FirstOrDefault(face => face.Type == type)?.Sprite;
            if (sprite==null)
            {
                Debug.LogError($"Not Found KureFaceSprite. type:{type}");
            }
            return sprite;
        }
        
        public Sprite GetQueenFace(QueenFaceSpriteType type)
        {
            Sprite sprite = queenFaces.FirstOrDefault(face => face.Type == type)?.Sprite;
            if (sprite==null)
            {
                Debug.LogError($"Not Found QueenFaceSprite. type:{type}");
            }
            return sprite;
        }
        
        public Sprite GetMobFace(MobFaceSpriteType type)
        {
            Sprite sprite = mobFaces.FirstOrDefault(face => face.Type == type)?.Sprite;
            if (sprite==null)
            {
                Debug.LogError($"Not Found MobFaceSprite. type:{type}");
            }
            return sprite;
        }

        public Sprite GetCoreteFace(ColateFaceSpriteType type)
        {
            Sprite sprite = colateFaces.FirstOrDefault(face => face.Type == type)?.Sprite;
            if (sprite==null)
            {
                Debug.LogError($"Not Found ColateFaceSprite. type:{type}");
            }
            return sprite;
        }
    }
}