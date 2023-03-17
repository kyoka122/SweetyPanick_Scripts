using MyApplication;
using UnityEngine;

namespace InGame.Stage.View
{
    [RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
    public class BackgroundView:MonoBehaviour
    {
        public BoxCollider2D backGroundCollider2D { get; private set; }

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
            Debug.Log($"CurrentBackGroundPos:{transform.position}",gameObject);
            Debug.Log($"CurrentLocalBackGroundPos:{transform.localPosition}",gameObject);
            Debug.Log($"BackGround SetPos:{pos}",gameObject);
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
    }
}