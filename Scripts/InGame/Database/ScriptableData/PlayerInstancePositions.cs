using System;
using MyApplication;
using UnityEngine;

namespace InGame.Database.ScriptableData
{
    public class PlayerInstancePositions:MonoBehaviour
    {
        public Vector2 CandyInstancePos=>candyInstancePos;
        public Vector2 MashInstancePos=>mashInstancePos;
        public Vector2 FuInstancePos=>fuInstancePos;
        public Vector2 KureInstancePos=>kureInstancePos;
        
        [SerializeField] private Vector2 candyInstancePos;
        [SerializeField] private Vector2 mashInstancePos;
        [SerializeField] private Vector2 fuInstancePos;
        [SerializeField] private Vector2 kureInstancePos;

        public Vector2 GetPosition(PlayableCharacter type)
        {
            switch(type)
            {
                case PlayableCharacter.None:
                    return Vector2.zero;
                case PlayableCharacter.Candy:
                    return candyInstancePos;
                case PlayableCharacter.Mash:
                    return mashInstancePos;
                case PlayableCharacter.Fu:
                    return fuInstancePos;
                case PlayableCharacter.Kure:
                    return kureInstancePos;
                default:
                    Debug.LogError($"PlayableCharacterType Is None");
                    return Vector2.zero;
            }
        }
    }
}