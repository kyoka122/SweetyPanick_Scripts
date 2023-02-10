using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.Player.View
{
    public class WeaponView:MonoBehaviour
    {
        public Transform WeaponCenterTransform => weaponCenterTransform;
        public IReadOnlyList<Collider2D> TriggerStayColliders => new List<Collider2D>(_triggerStayColliders);
        
        public CancellationToken cancellationToken { get; private set; }
        

        [SerializeField] private Transform weaponCenterTransform;
        
        private List<Collider2D> _triggerStayColliders;
        
        
        public void Init()
        {
            _triggerStayColliders = new List<Collider2D>();
            cancellationToken = this.GetCancellationTokenOnDestroy();
            
            this.OnTriggerEnter2DAsObservable()
                .Subscribe(_triggerStayColliders.Add)
                .AddTo(this);
            
            this.OnTriggerExit2DAsObservable()
                .Subscribe(collider2D=>_triggerStayColliders.Remove(collider2D))
                .AddTo(this);
        }
    }
}