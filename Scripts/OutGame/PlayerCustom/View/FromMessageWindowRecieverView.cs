using UniRx;
using UnityEngine;

namespace OutGame.PlayerCustom.View
{
    public class FromMessageWindowRecieverView:MonoBehaviour
    {
        private Subject<bool> _moveNextScene;
        public void Init(Subject<bool> moveNextScene)
        {
            _moveNextScene = moveNextScene;
        }

        public void SendCheerMessageEvent()
        {
            _moveNextScene.OnNext(true);
        }
    }
}