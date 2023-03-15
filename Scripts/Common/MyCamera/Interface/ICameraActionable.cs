using UnityEngine;

namespace InGame.MyCamera.Interface
{
    public interface ICameraActionable
    {
        public void Shake();

        public void ShakeWithVelocity(Vector3 velocity);

        public Transform GetCameraTransform();
    }
}