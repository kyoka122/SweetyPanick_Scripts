using System;
using System.Threading;
using MyApplication;
using UnityEngine;

namespace OutGame.Epilogue
{
    public class EpilogueBehaviour:MonoBehaviour
    {
        [SerializeField] Fungus.Flowchart flowchart;
        //[SerializeField] private Image fadeSpriteImage;
        
        //private ScreenFader _screenFader;
        private CancellationToken _token;
        private Action _toNextSceneEvent;
        
        public void Init(Action toNextSceneEvent)
        {
            //_screenFader = new ScreenFader(fadeSpriteImage);
            _toNextSceneEvent = toNextSceneEvent;
            CallStartTalk();
        }
        
        private void CallStartTalk()
        {
            flowchart.SendFungusMessage(FungusCallMethodName.StartTalk);
        }

        #region Callbacks

        public void MoveNextScene()
        {
            _toNextSceneEvent.Invoke();
        }
        
        #endregion
        
    }
}