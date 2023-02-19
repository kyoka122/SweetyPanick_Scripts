using UnityEngine;

namespace InGame.MyCamera.Interface
{
    public interface ICameraEvent
    {
        public void Shake();

        public void ShakeWithVelocity(Vector3 velocity);
    }
}