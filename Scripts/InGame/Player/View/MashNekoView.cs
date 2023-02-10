using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Enemy.View;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.Player.View
{
    public class MashNekoView:MonoBehaviour
    {
        public IObservable<Collider2D> OnTriggerEnter=>_onTriggerEnterSubject;
        
        public IReadOnlyList<IEnemyDecoyAble> decoyAbleEnemies => _decoyAbleEnemies;

        public CancellationToken thisToken { get; private set; }

        private readonly Quaternion rightRot=Quaternion.Euler(0,180,0);
        private readonly Quaternion leftRot=Quaternion.Euler(0,0,0);
        
        private Subject<Collider2D> _onTriggerEnterSubject;
        private List<IEnemyDecoyAble> _decoyAbleEnemies;
        public void Init(float playerDirection)
        {
            _decoyAbleEnemies = new List<IEnemyDecoyAble>();
            _onTriggerEnterSubject = new Subject<Collider2D>();
            thisToken = this.GetCancellationTokenOnDestroy();
            if (playerDirection>=0)
            {
                transform.rotation = rightRot;
            }
            else
            {
                transform.rotation = leftRot;
            }
            RegisterObservable();
        }

        private void RegisterObservable()
        {
            this.OnTriggerEnter2DAsObservable()
                .Subscribe(collider2D =>
                {
                    _onTriggerEnterSubject.OnNext(collider2D);
                })
                .AddTo(this);
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public void RegisterDecoyEnemy(IEnemyDecoyAble newDecoyAbleEnemy)
        {
            _decoyAbleEnemies.Add(newDecoyAbleEnemy);
        }

        public void ReleaseEnemyFromList(IEnemyDecoyAble enemyDecoyAble)
        {
            _decoyAbleEnemies.Remove(enemyDecoyAble);
        }
        
        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}