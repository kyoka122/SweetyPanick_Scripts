using System;
using Cinemachine;
using DG.Tweening;
using InGame.MyCamera.Interface;
using UnityEngine;

namespace Common.MyCamera.View
{
    public class MainCameraView:MonoBehaviour,IReadOnlyCameraFunction,ICameraActionable
    {
        public float CinemaChineMaxWeight => cinemaChineMaxWeight;
        public float CinemaChineMinWeight => cinemaChineMinWeight;
        public float CinemaChineWeightRate => cinemaChineMaxWeight - cinemaChineMinWeight;
        
        private CinemachineTargetGroup _targetGroup;
        private CinemachineImpulseSource _cinemachineImpulseSource;
        private Camera _camera;
        private Transform _cameraTransform;
        
        [SerializeField] private float cinemaChineMaxWeight = 1;
        [SerializeField] private float cinemaChineMinWeight = 0.7f;
        [SerializeField] private float cinemaChineMaxRadius = 1;
        [SerializeField] private float cinemaChineMinRadius = 0.7f;
        
        public void Init(CinemachineTargetGroup targetGroup,CinemachineImpulseSource cinemachineImpulseSource,Camera camera)
        {
            _targetGroup = targetGroup;
            _cinemachineImpulseSource = cinemachineImpulseSource;
            if (CinemaChineMaxWeight-CinemaChineMinWeight<0)
            {
                Debug.LogError($"Please Fix: CinemaChineMaxWeight < CinemaChineMinWeight");
            }
            if (cinemaChineMaxRadius-cinemaChineMinRadius<0)
            {
                Debug.LogError($"Please Fix: CinemaChineMaxRadius < CinemaChineMinRadius");
            }
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

        public float GetSize()
        {
            return _camera.orthographicSize;
        }

        public Vector2 WorldToViewPortPoint(Vector2 position)
        {
            return _camera.WorldToViewportPoint(position);
        }

        public Vector2 WorldToScreenPoint(Vector2 position)
        {
            return _camera.WorldToScreenPoint(position);
        }

        public Vector2 ScreenToWorldPoint(Vector2 position)
        {
            return _camera.ScreenToWorldPoint(position);
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
                _targetGroup.AddMember(characterTransform,cinemaChineMinRadius,cinemaChineMaxRadius);
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

        public void SetCameraTargetWeight(Transform characterTransform,float weight)
        {
            int index = _targetGroup.FindMember(characterTransform);
            if (index!=-1)
            {
                _targetGroup.m_Targets[index].weight = weight;
            }
        }
        
        public float GetCameraTargetWeight(Transform characterTransform)
        {
            int index = _targetGroup.FindMember(characterTransform);
            if (index!=-1)
            {
                return _targetGroup.m_Targets[index].weight;
            }

            Debug.LogError($"Not Found Transform :{characterTransform.gameObject.name}");
            return 1;
        }

        public void SetCameraRange(Transform characterTransform, float range)
        {
            int index = _targetGroup.FindMember(characterTransform);
            if (index != -1)
            {
                _targetGroup.m_Targets[index].radius = range;
            }
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

        public Transform GetCameraTransform()
        {
            return _cameraTransform;
        }

        public void ChangeWeight(Transform characterTransform,float weight)
        {
            if (weight<0||1<weight)
            {
                return;
            }
            
            var index = _targetGroup.FindMember(characterTransform);
            if (index<=-1)
            {
                return;
            }

            if (Math.Abs(_targetGroup.m_Targets[index].weight - weight) < 0.01f)
            {
                return;
            }
            //_targetGroup.RemoveMember(characterTransform);
            //_targetGroup.AddMember(characterTransform,weight,cinemaChineRadius);
        }
        
        public void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }
        
        public void DoMoveXYPosition(Vector2 pos,float duration)
        {
            transform.DOMove(new Vector3(pos.x,pos.y,transform.position.z),duration);
        }

        public void DoSize(float size,float duration)
        {
            DOTween.To(() => _camera.orthographicSize, val =>_camera.orthographicSize = val, 
                size, duration);
        }
    }
}