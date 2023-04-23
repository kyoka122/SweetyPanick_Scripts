using MyApplication;
using UnityEngine;

namespace InGame.Stage.View
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MoveFloorView:MonoBehaviour
    {
        public float MoveYMax => moveDestination.y;
        public Vector2 Position => transform.position;

        public Vector2 InitPos { get; private set; }
        
        [SerializeField] private GumGimmickView rightGum;
        [SerializeField] private GumGimmickView leftGum;//MEMO: 抽象化すべき？？
        [SerializeField] private Vector2 moveDestination;
        

        private Rigidbody2D _rigidbody;

        public void Init()
        {
            InitPos = transform.position;
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void SetConstrainsFreeze()
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public void SetConstrainsFreeY()
        {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX|RigidbodyConstraints2D.FreezeRotation;
        }
        
        public bool HadFixedGums()
        {
            return rightGum.fixState == FixState.Fixed && leftGum.fixState == FixState.Fixed;
        }

        public void SetYVelocity(float yVelocity)
        {
            _rigidbody.velocity = new Vector2(0, yVelocity);
        }
    }
}