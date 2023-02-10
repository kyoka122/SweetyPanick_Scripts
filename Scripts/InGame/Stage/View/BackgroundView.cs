using UnityEngine;

namespace InGame.Stage.View
{
    public class BackgroundView:MonoBehaviour
    {
        public Vector2 initPos { get; private set; }
        
        public void Init()
        {
            initPos = transform.position;
        }
        
        public Vector2 GetPosition()
        {
            return transform.position;
        }
        
        public void SetPosition(Vector2 pos)
        {
            transform.position = pos;
        }
    }
}