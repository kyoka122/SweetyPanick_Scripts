﻿using System;
using Cinemachine;
using InGame.MyCamera.Controller;
using UnityEngine;

namespace InGame.MyCamera.View
{
    public class MainCameraView:MonoBehaviour,IReadOnlyCameraFunction
    {
        private CinemachineTargetGroup _targetGroup;
        private Camera _camera;
        private Transform _cameraTransform;
        
        [SerializeField] private int cinemaChineWeight = 1;
        [SerializeField] private int cinemaChineRadius = 1;
        
        public void Init(CinemachineTargetGroup targetGroup,Camera camera)
        {
            _targetGroup = targetGroup;
            Init(camera);
        }
        
        public void Init(Camera camera)
        {
            _camera = camera;
            _cameraTransform = _camera.transform;
        }

        public Vector2 GetPosition()
        {
            return _cameraTransform.position;
        }
        
        public Vector2 WorldToViewPortPoint(Vector2 position)
        {
            return _camera.WorldToViewportPoint(position);
        }

        public Vector2 WorldToScreenPoint(Vector2 position)
        {
            return _camera.WorldToScreenPoint(position);
        }

        public bool TryAddTargetGroup(Transform characterTransform)
        {
            if (_targetGroup.FindMember(characterTransform)==-1)
            {
                Debug.Log($"AddTargetGroup");
                _targetGroup.AddMember(characterTransform,cinemaChineWeight,cinemaChineRadius);
                return true;
            }

            return false;
        }

        public void ChangeWeight(Transform characterTransform,float weight)
        {
            if (weight<0||1<weight)
            {
                return;
            }
            
            var index = _targetGroup.FindMember(characterTransform);
            if (index==-1)
            {
                return;
            }

            if (Math.Abs(_targetGroup.m_Targets[index].weight - weight) < 0.01f)
            {
                return;
            }
            _targetGroup.RemoveMember(characterTransform);
            _targetGroup.AddMember(characterTransform,weight,cinemaChineRadius);
        }
        
        public void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        public void SetSize(float size)
        {
            _camera.orthographicSize = size;
        }
    }
}