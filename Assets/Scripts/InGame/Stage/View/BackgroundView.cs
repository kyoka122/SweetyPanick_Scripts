using MyApplication;
using UnityEngine;

namespace InGame.Stage.View
{
    [RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
    public class BackgroundView:MonoBehaviour
    {
        public float MinSize => minSize;
        public float MaxSize => maxSize;
        public BoxCollider2D backGroundCollider2D { get; private set; }

        [SerializeField] private float minSize;
        [SerializeField] private float maxSize;
        
        private Rigidbody2D _rigidbody2D;
        
        public void Init()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            backGroundCollider2D = GetComponent<BoxCollider2D>();
        }
        
        public Vector2 GetPosition()
        {
            return transform.position;
        }
        
        public void SetPosition(Vector2 pos)
        {
            transform.position = pos;
        }
        
        public void SetLocalXPosition(float posX)
        {
            transform.localPosition = new Vector3(posX, transform.localPosition.y, transform.localPosition.z);
        }
        
        public void SetLocalXYPosition(Vector2 pos)
        {
            transform.localPosition = new Vector3(pos.x, pos.y, transform.localPosition.z);
        }

        public void SetVelocity(Vector2 force)
        {
            _rigidbody2D.velocity = force;
        }

        public void SetParent(Transform cameraTransform)
        {
            transform.SetParent(cameraTransform);
        }

        public void SetSize(float size)
        {
            transform.localScale = new Vector2(size,size);
        }
    }
}