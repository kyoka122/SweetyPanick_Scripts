using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InGame.SceneLoader.Entity;
using InGame.SceneLoader.View;
using MyApplication;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InGame.SceneLoader.Logic
{
    public class LoadLogic
    {
        public bool isLoading { get; private set; }
        public bool isFading { get; private set; }
        public bool canFinishLoading { get; private set; } = true;
        
        private readonly LoadEntity _loadEntity;
        private readonly LoadScreenView _loadScreenView;
        private readonly BlackFadeView _blackFadeView;
        private readonly LoadCameraView _loadCameraView;
        private CancellationTokenSource _tokenSource;

        public LoadLogic(LoadEntity loadEntity, LoadScreenView loadScreenView,BlackFadeView blackFadeView,LoadCameraView loadCameraView)
        {
            _loadEntity = loadEntity;
            _loadScreenView = loadScreenView;
            _blackFadeView = blackFadeView;
            _loadCameraView = loadCameraView;
            SetInitViewData();
        }

        private void SetInitViewData()
        {
            _loadEntity.SetWaveObjMoveDistance(_loadScreenView.WaveTransform.sizeDelta.x-_loadCameraView.cameraWidth);
            _loadEntity.SetLoadBackGroundMoveDistance(_loadScreenView.LoadBackGroundWidth-_loadCameraView.cameraWidth);
        }
        
        public async UniTask PlayLoadScreen(float loadDurationMin,CancellationToken token)
        {
            isFading = true;
            isLoading = true;
            canFinishLoading = false;
            _tokenSource = new CancellationTokenSource();
            _loadScreenView.InitOnFadeIn();
            _blackFadeView.InitOnFadeIn();
            _loadScreenView.SetActiveLoadScreenParent(true);
            _loadCameraView.SetActive(true);
            _blackFadeView.SetActive(true);
            

            //MEMO: ↓フェード
            await _blackFadeView.BlackScreenFader.FadeIn(_loadEntity.BlackScreenFadeInDuration,Ease.Unset,token);
            await _loadScreenView.FadeInLoadObjects(_loadScreenView.LoadScreenMaterials,_loadEntity.LoadFadeInDuration, Ease.Unset, token);
            _blackFadeView.BlackScreenFader.SetFadeOutCondition();
            _blackFadeView.SetActive(false);
            isFading = false;
            
            StartUpdateScrollAnimation();
            WaitLoad(loadDurationMin,token).Forget();
        }

        /// <summary>
        /// ロード中のスクロールアニメーション
        /// </summary>
        private void StartUpdateScrollAnimation()
        {
            _loadScreenView.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    UpdateRightScroll(_loadScreenView.LoadBackgroundRectTransform, _loadScreenView.loadBackGroundXDefaultPosition,
                        _loadEntity.loadBackGroundMoveDistance, _loadEntity.BackgroundScrollSpeed);
                    UpdateRightScroll(_loadScreenView.WaveTransform, _loadScreenView.waveObjXDefaultPosition,
                        _loadEntity.waveObjMoveDistance,_loadEntity.WaveScrollSpeed);
                }).AddTo(_tokenSource.Token);
        }


        private async UniTask WaitLoad(float duration,CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: token);
            canFinishLoading = true;
        }

        /// <summary>
        /// ロード中であればロード演出を終了する
        /// </summary>
        public async UniTask TryPlayLoadScreenFadeOut(CancellationToken token)
        {
            isFading = true;
            isLoading = false;
            _tokenSource.Cancel();
            _loadScreenView.LoadScreenFader.SetFadeInCondition();

            Debug.Log($"StartFadeOut");
            await _loadScreenView.FadeOutLoadObjects(_loadScreenView.LoadScreenMaterials,_loadEntity.LoadFadeOutDuration, Ease.Unset, token);
            await _loadScreenView.LoadScreenFader.FadeOut(_loadEntity.LoadBackgroundFadeOutDuration, Ease.Unset, token);
            
            _loadScreenView.SetActiveLoadScreenParent(false);
            _blackFadeView.SetActive(false);
            _loadCameraView.SetActive(false);
            isFading = false;
        }

        

        /// <summary>
        /// ゲームオブジェクトをスクロールする。Update関数で呼び出すこと
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="defaultPosX"></param>
        /// <param name="width"></param>
        /// <param name="speed"></param>
        private void UpdateLeftScroll(RectTransform rectTransform,float defaultPosX,float width,float speed)
        {
            rectTransform.Translate(-speed * Time.deltaTime, 0 , 0);

            if (defaultPosX - rectTransform.localPosition.x < -width)
            {
                rectTransform.localPosition=new Vector3(defaultPosX,rectTransform.localPosition.y,rectTransform.localPosition.z);
            }
        }

        /// <summary>
        /// ゲームオブジェクトをスクロールする。Update関数で呼び出すこと
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="defaultPosX"></param>
        /// <param name="width"></param>
        /// <param name="speed"></param>
        private void UpdateRightScroll(RectTransform rectTransform,float defaultPosX,float width,float speed)
        {
            rectTransform.Translate(speed * Time.deltaTime, 0, 0);
            
            if (rectTransform.localPosition.x - defaultPosX > width)
            {
                rectTransform.localPosition=new Vector3(defaultPosX,rectTransform.localPosition.y,rectTransform.localPosition.z);
            }
        }
    }
}