using UnityEngine;

namespace InGame.MyCamera.Controller
{
    public interface IReadOnlyCameraFunction
    {
        public Vector2 GetPosition();
        public Vector2 WorldToViewPortPoint(Vector2 position);
        public Vector2 WorldToScreenPoint(Vector2 position);
    }
}