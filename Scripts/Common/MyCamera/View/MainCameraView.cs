using System;
using Cinemachine;
using InGame.MyCamera.Interface;
using UnityEngine;

namespace InGame.MyCamera.View
{
    public class MainCameraView:MonoBehaviour,IReadOnlyCameraFunction,ICameraEvent
    {
        private CinemachineTargetGroup _targetGroup;
        private CinemachineImpulseSource _cinemachineImpulseSource;
        private Camera _camera;
        private Transform _cameraTransform;
        
        [SerializeField] private int cinemaChineWeight = 1;
        [SerializeField] private int cinemaChineRadius = 1;
        
        public void Init(CinemachineTargetGroup targetGroup,CinemachineImpulseSource cinemachineImpulseSource,Camera camera)
        {
            _targetGroup = targetGroup;
            _cinemachineImpulseSource = cinemachineImpulseSource;
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
            if (_targetGroup==null)
            {
                return false;
            }
            if (_targetGroup.FindMember(characterTransform)==-1)
            {
                Debug.Log($"AddTargetGroup");
                _targetGroup.AddMember(characterTransform,cinemaChineWeight,cinemaChineRadius);
                return true;
            }

            return false;
        }
        
        public bool TryRemoveTargetGroup(Transform characterTransform)
        {
            if (_targetGroup.FindMember(characterTransform)!=-1)
            {
                Debug.Log($"RemoveTargetGroup");
                _targetGroup.RemoveMember(characterTransform);
                return true;
            }

            return false;
        }

        public void Shake()
        {
            if (_cinemachineImpulseSource!=null)
            {
                _cinemachineImpulseSource.GenerateImpulse();
            }
        }
        
        public void ShakeWithVelocity(Vector3 velocity)
        {
            if (_cinemachineImpulseSource!=null)
            {
                _cinemachineImpulseSource.GenerateImpulseWithVelocity(velocity);
            }
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