using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using InGame.Database;
using KanKikuchi.AudioManager;
using MyApplication;
using OutGame.Database;
using OutGame.Database.ScriptableData;
using OutGame.Prologue;
using TalkSystem;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace InGame.OutGame.Score
{
    public class ScoreSceneBehaviour:MonoBehaviour,IDisposable
    {
        private const int Zero = 0;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private GameObject finishTextObj;
        [SerializeField] private Dialogs showScoreDialogs;
        [SerializeField] private Dialogs thanksDialog;
        [SerializeField] private TextMeshProUGUI bossDefeatBonusText;
        [SerializeField] private MaskableGraphic bossDefeatBonusMessageImage;
        [SerializeField] private MaskableGraphic bossDefeatBonusMessage;
        [SerializeField] private ScoreSceneScriptableData scoreSceneData;
        [SerializeField] private Image medalImage;
        
        private AllNextKeyObserver _allNextKeyObserver;
        
        private int _score;
        private Action _onMoveNextScene;
        private ScoreSceneState _state;
        
        private enum ScoreSceneState
        {
            None,
            TalkingScoreAnimationStart,
            ScoreAnimation,
            TalkingThankYouForPlaying,
            WaitFinishGameTrigger
        }

        public void Init(InGameDatabase inGameDatabase,OutGameDatabase outGameDatabase, Action onMoveNextScene)
        {
            _allNextKeyObserver = new AllNextKeyObserver();
            showScoreDialogs.Init(_allNextKeyObserver,outGameDatabase.GetDialogFaceSpriteScriptableData());
            thanksDialog.Init(_allNextKeyObserver,outGameDatabase.GetDialogFaceSpriteScriptableData());
            _onMoveNextScene = onMoveNextScene;
            
            _score = inGameDatabase.GetAllStageData().score;
            
#if UNITY_EDITOR
            //MEMO: Debug!!////////////////////////////
            if (_score == 0)
            {
                _score = 223526;
            }
#endif
            InitUI();
        }

        private void InitUI()
        {
            Color colorChche = bossDefeatBonusText.color;
            bossDefeatBonusText.color=new Color(colorChche.r,colorChche.g,colorChche.b,0);
            colorChche = bossDefeatBonusMessageImage.color;
            bossDefeatBonusMessageImage.color=new Color(colorChche.r,colorChche.g,colorChche.b,0);
            colorChche = bossDefeatBonusMessage.color;
            bossDefeatBonusMessage.color=new Color(colorChche.r,colorChche.g,colorChche.b,0);
            
            RegisterClickObserver();
        }

        public void StartTalkingShowScoreAnimation()
        {
            _state = ScoreSceneState.TalkingScoreAnimationStart;
            showScoreDialogs.StartDialogs();
        }

        private void RegisterClickObserver()
        {
            _allNextKeyObserver.OnNext
                .Subscribe(_ =>
                {
                    SwitchClickEvent();
                }).AddTo(this);
        }

        private void SwitchClickEvent()
        {
            switch (_state)
            {
                case ScoreSceneState.WaitFinishGameTrigger:
                    _onMoveNextScene?.Invoke();
                    break;
                default:
                    Debug.Log($"Couldn`t Find ScoreSceneStateCase. state:{_state}");
                    break;
            }
        }

        #region TextAnimation
        
        private async void ExecuteScoreAnimation()
        {
            _state = ScoreSceneState.ScoreAnimation;
            bossDefeatBonusText.text = $"+{scoreSceneData.BossDefeatBonus}";
            int bonusAddedScore = _score + scoreSceneData.BossDefeatBonus;
            medalImage.sprite = GetMedalImage(bonusAddedScore);
            medalImage.transform.localScale = Vector3.one * scoreSceneData.MedalInitScaleFactor;
            BGMManager.Instance.FadeOut(BGMPath.PROLOGUE,scoreSceneData.BgmFadeOutDuration);
            await UniTask.Delay(TimeSpan.FromSeconds(scoreSceneData.BgmFadeOutDuration+scoreSceneData.ToScoreCountUpDelay),
                cancellationToken:this.GetCancellationTokenOnDestroy());
            
            SEManager.Instance.Play(SEPath.DRUM_ROLL);
            DOTween.Sequence()
                //MEMO: 基礎スコアカウント
                .Append(DOTween.To(() => Zero, SetScoreView, _score, scoreSceneData.ScoreCountUpDuration))
                .AppendCallback(()=>SEManager.Instance.Stop(SEPath.DRUM_ROLL))
                .AppendCallback(()=>SEManager.Instance.Play(SEPath.FINISH_DRUM_ROLL))
                
                //MEMO: ボーナススコア表示
                .AppendInterval(scoreSceneData.BossDefeatBonusFadeInDelay)
                .Append(bossDefeatBonusText.DOFade(1, scoreSceneData.BossDefeatBonusEnterDuration))
                .Join(bossDefeatBonusMessageImage.DOFade(1, scoreSceneData.BossDefeatBonusEnterDuration))
                .Join(bossDefeatBonusMessage.DOFade(1, scoreSceneData.BossDefeatBonusEnterDuration))
                
                //MEMO: ボーナススコアカウント
                .AppendInterval(scoreSceneData.BossDefeatBonusCountUpDelay)
                .AppendCallback(()=>SEManager.Instance.Play(SEPath.DRUM_ROLL))
                .Append(DOTween.To(() => _score, SetScoreView, bonusAddedScore,
                    scoreSceneData.BossDefeatBonusCountUpDuration))
                .Join(bossDefeatBonusText.transform.DOMoveY(scoreText.transform.position.y, scoreSceneData.BossDefeatBonusCountUpDuration))
                .Join(bossDefeatBonusText.DOFade(0, scoreSceneData.BossDefeatBonusCountUpDuration))
                .Join(bossDefeatBonusMessageImage.DOFade(0, scoreSceneData.BossDefeatBonusCountUpDuration))
                .Join(bossDefeatBonusMessage.DOFade(0, scoreSceneData.BossDefeatBonusCountUpDuration))
                .AppendCallback(()=>SEManager.Instance.Stop(SEPath.DRUM_ROLL))
                .AppendCallback(()=>SEManager.Instance.Play(SEPath.FINISH_DRUM_ROLL))
                
                //MEMO: メダル表示
                .AppendInterval(scoreSceneData.MedalEnterDelay)
                .Append(medalImage.DOFade(1,scoreSceneData.MedalEnterDuration))
                .Join(medalImage.transform.DOScale(Vector2.one,scoreSceneData.MedalEnterDuration))
                .AppendCallback(()=>SEManager.Instance.Play(SEPath.CHEER,1.5f))
                
                .AppendInterval(scoreSceneData.ToTextDelay)
                .AppendCallback(()=>BGMSwitcher.FadeIn(BGMPath.PROLOGUE,scoreSceneData.BgmFadeInDuration))
                .OnComplete(SetScoreShowedState);
        }

        private Sprite GetMedalImage(int score)
        {
            var medalImagesEnumerable = scoreSceneData.MedalImages.OrderByDescending(image => image.Score);
            foreach (var medalImage in medalImagesEnumerable)
            {
                if (medalImage.Score<=score)
                {
                    return medalImage.Sprite;
                }
            }

            return scoreSceneData.MedalImages.FirstOrDefault(image => image.Type == MedalType.None)?.Sprite;
        }

        private void SetScoreView(int score)
        {
            scoreText.text = score.ToString("D6");
        }
        
        private void SetScoreShowedState()
        {
            _state = ScoreSceneState.TalkingThankYouForPlaying;
            thanksDialog.StartDialogs();
        }

        #endregion


        #region Callback
   
        public void SetWaitScoreAnimationTrigger()
        {
            _state = ScoreSceneState.ScoreAnimation;
            ExecuteScoreAnimation();
        }
        
        public async void SetWaitFinishGameTrigger()
        {
            finishTextObj.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(scoreSceneData.CanOnTriggerFinishGameDelay),cancellationToken:this.GetCancellationTokenOnDestroy());
            _state = ScoreSceneState.WaitFinishGameTrigger;
        }
        
        
        #endregion

        public void Dispose()
        {
            _allNextKeyObserver?.Dispose();
        }
    }
}