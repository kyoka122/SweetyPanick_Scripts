﻿using UnityEngine;

namespace InGame.MyCamera.Interface
{
    public interface IReadOnlyCameraFunction
    {
        public Vector2 GetPosition();
        public float GetSize();
        public Vector2 WorldToViewPortPoint(Vector2 position);
        public Vector2 WorldToScreenPoint(Vector2 position);
        public Vector2 ScreenToWorldPoint(Vector2 position);
    }
}