using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using OutGame.Database;
using OutGame.Database.ScriptableData;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Utility.PanelAnimation;

namespace OutGame.Title
{
    public class TitleBehaviour:MonoBehaviour,IDisposable
    {
        [SerializeField] private GameObject creditObj;
        [SerializeField] private Scrollbar creditScrollVar;
        
        private Action _toNextSceneEvent;
        private AllTitleInputObserver _allTalkInputObserver;
        private PopAnimation _popAnimation;
        private CancellationToken _token;
        private OutGameDatabase _outGameDatabase;
        
        private bool _isOnCredit;
        private bool _isPopUpCredit;
        private bool _isPopDownCredit;
        
        public void Init(Action toNextSceneEvent,OutGameDatabase outGameDatabase)
        {
            _toNextSceneEvent = toNextSceneEvent;
            _allTalkInputObserver = new AllTitleInputObserver();
            _outGameDatabase = outGameDatabase;
            
            _token = this.GetCancellationTokenOnDestroy();
            creditObj.SetActive(false);
            creditObj.transform.localScale=Vector3.zero;
            TitleSceneScriptableData data = _outGameDatabase.GetTitleSceneScriptableData();
            if (data==null)
            {
                Debug.LogError($"Not Found TitleSceneScriptableData");
                return;
            }
            _popAnimation = new PopAnimation(creditObj, data.CreditPopUpDuration, data.CreditPopDownDuration);
        }

        public void StartWaitingInput()
        {
            _allTalkInputObserver.OnNext
                .Subscribe(_ => _toNextSceneEvent?.Invoke())
                .AddTo(this);

            _allTalkInputObserver.SwitchCredit
                .Subscribe(_ => TrySwitchCreditView())
                .AddTo(this);

            _allTalkInputObserver.OnChangeScrollCreditValue.Subscribe(value =>
            {
                if (_isOnCredit)
                {
                    float nextScrollVarValue = creditScrollVar.value +
                                            value * _outGameDatabase.GetTitleSceneScriptableData().CreditScrollFactor;
                    creditScrollVar.value = MyMathf.InRange(nextScrollVarValue, 0, 1);
                }
            }).AddTo(this);
        }

        private void TrySwitchCreditView()
        {
            if (_isPopUpCredit || _isPopDownCredit)
            {
                return;
            }
            if (_isOnCredit)
            {
                _isPopDownCredit = true;
                PopDownCredit();
                return;
            }
            _isPopUpCredit = true;
            PopUpCredit();
        }

        private async void PopUpCredit()
        {
            creditObj.SetActive(true);
            Debug.Log($"PopUp!");
            await _popAnimation.Enter(_token);
            Debug.Log($"PopUpFinish!");
            _isPopUpCredit = false;
            _isOnCredit = true;
        }
        
        private async void PopDownCredit()
        {
            Debug.Log($"PopDown!");
            await _popAnimation.Exit(_token);
            creditObj.SetActive(false);
            Debug.Log($"PopDownFinish!");
            _isPopDownCredit = false;
            _isOnCredit = false;
        }

        public void Dispose()
        {
            _allTalkInputObserver?.Dispose();
        }
    }
}