using UnityEngine;

namespace InGame.Enemy.View
{
    public class HavingKeyEnemyView:BaseEnemyView
    {
        [SerializeField] private Transform toGroundRayPosTransform;
        [SerializeField] private GameObject keyObj;
        
        public Vector2 GetToGroundRayPos()
        {
            return toGroundRayPosTransform.position;
        }

        public void AddYVelocity(float newYVelocity)
        {
            rigidbody2D.AddForce(new Vector2(0, newYVelocity));
        }

        public void DropKey()
        {
            keyObj.transform.SetParent(null);
            keyObj.GetComponent<Rigidbody2D>().simulated = true;
        }

        public void SetActiveKey(bool active)
        {
            keyObj.SetActive(active);
        }
    }
}