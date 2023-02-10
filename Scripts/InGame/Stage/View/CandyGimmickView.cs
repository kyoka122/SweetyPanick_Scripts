using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InGame.Stage.View
{
    public class CandyGimmickView:DefaultGimmickSweetsView
    {
        [SerializeField] private Collider2D repairCollider;

        protected override void EachSweetsEvent()
        {
            repairCollider.enabled = true;
        }
    }
}

