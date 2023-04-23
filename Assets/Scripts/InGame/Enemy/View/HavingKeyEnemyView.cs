using UnityEngine;

namespace InGame.Enemy.View
{
    public class HavingKeyEnemyView:BaseEnemyView
    {
        private readonly Quaternion TowardRightRot = Quaternion.Euler(0, 180, 0);
        private readonly Quaternion TowardLeftRot = Quaternion.Euler(0, 0, 0);
        
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
        
        public override void SwitchDirection()
        {
            enemyDirectionX *= -1;
            SetSpriteRotation();
        }

        public override void SetDirection(int direction)
        {
            enemyDirectionX = direction > 0 ? 1 : -1;
            SetSpriteRotation();
        }
        
        private void SetSpriteRotation()
        {
            if (enemyDirectionX>0)
            {
                transform.rotation = TowardRightRot;
            }

            if (enemyDirectionX<0)
            {
                transform.rotation = TowardLeftRot;
            }
        }
    }
}