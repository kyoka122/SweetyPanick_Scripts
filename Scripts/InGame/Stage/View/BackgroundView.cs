using UnityEngine;

namespace InGame.Stage.View
{
    public class BackgroundView:MonoBehaviour
    {
        private Rigidbody2D _rigidbody2D;
        
        public void Init()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        public Vector2 GetPosition()
        {
            return transform.position;
        }
        
        public void SetPosition(Vector2 pos)
        {
            transform.position = pos;
        }

        public void SetVelocity(Vector2 force)
        {
            _rigidbody2D.velocity = force;
        }
    }
}