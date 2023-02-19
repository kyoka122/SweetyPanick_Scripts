using UnityEngine;

namespace InGame.Stage.View
{
    public class BackgroundView:MonoBehaviour
    {
        public Vector2 initPos { get; private set; }
        private Rigidbody2D _rigidbody2D;
        
        public void Init()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
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

        public void SetVelocity(Vector2 force)
        {
            _rigidbody2D.velocity = force;
        }
    }
}