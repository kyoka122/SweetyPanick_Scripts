using System.Linq;
using MyApplication;
using UnityEngine;

namespace InGame.Database.ScriptableData
{
    public class PlayerInstanceData:MonoBehaviour
    {
        public PlayerPosition[] PlayerPositions=>playerPositions;

        [SerializeField] private PlayerPosition[] playerPositions;
        public Vector2 GetPosition(PlayableCharacter type)
        {
            PlayerPosition position=PlayerPositions.FirstOrDefault(data => data.Type == type);
            if (position==null)
            {
                Debug.LogError($"Not Found Data. type:{type}");
                return Vector2.zero;
            }
            return position.Pos;
        }
    }
}