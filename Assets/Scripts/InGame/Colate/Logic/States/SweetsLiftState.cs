using System;
using System.Linq;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using InGame.Colate.Entity;
using InGame.Colate.View;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Colate.Logic
{
    public class SweetsLiftState:BaseColateStateLogic
    {
        public override ColateState state => ColateState.SweetsLift;
        
        public SweetsLiftState(ColateEntity colateEntity, ColateView colateView, ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent, DefaultSweetsLiftView[] sweetsLiftViews) 
            : base(colateEntity, colateView, colateStatusView, spawnEnemyEvent, sweetsLiftViews)
        {
        }

        protected override void Enter()
        {
            InitLifts();
            AppearLifts();
            disposables.Add(
                colateView.Attacked.Subscribe(_ =>
                    {
                        nextStateInstance = new DroppingState(colateEntity, colateView, colateStatusView, 
                            spawnEnemyEvent,sweetsLiftViews);
                    }
                ).AddTo(colateView));
            base.Enter();
        }

        protected override void Update()
        {
            if (nextStateInstance.state!=ColateState.SweetsLift)
            {
                stage = Event.Exit;
                return;
            }
            Drift();
            //MEMO: お菓子リフトの更新
            foreach (var sweetsBoardView in sweetsLiftViews)
            {
                if (sweetsBoardView.fixState!=FixState.Fixed)
                {
                    continue;
                }
                SetVelocityByLiftState(sweetsBoardView);
            }
            base.Update();
        }

        protected override void Exit()
        {
            DisAppearLifts();
            base.Exit();
        }
        
        /// <summary>
        /// お菓子リフトの順番をランダムにして初期化
        /// </summary>
        private void InitLifts()
        {
            var randomSortedPositions=sweetsLiftViews
                .Select(view=>view.GetPosition().x)
                .OrderBy(_ => Guid.NewGuid())
                .ToArray();//MEMO: ランダムに並び替え

            for (int i = 0; i < sweetsLiftViews.Length; i++)
            {
                sweetsLiftViews[i].SetXPosition(randomSortedPositions[i]);
                sweetsLiftViews[i].InitToBreakState();
                sweetsLiftViews[i].SetLiftState(LiftState.Up);
                sweetsLiftViews[i].SetYPosition(colateEntity.SweetBoardInitPosY);
            }
        }
        
        private void AppearLifts()
        {
            foreach (var sweetsBoardView in sweetsLiftViews)
            {
                sweetsBoardView.SetActive(true);
                
                ParticleSystem particle =
                    colateEntity.bigMiscParticle.GetObject(sweetsBoardView.AppearParticleInstanceTransform);
                particle.Play();
                particle.GetAsyncParticleSystemStoppedTrigger()
                    .ToObservable().FirstOrDefault()
                    .Subscribe(_ =>
                    {
                        colateEntity.bigMiscParticle.ReleaseObject(particle);                        
                    }).AddTo(particle);
            }
        }

        private void DisAppearLifts()
        {
            foreach (var sweetsBoardView in sweetsLiftViews)
            {
                sweetsBoardView.SetActive(false);
                
                ParticleSystem particle =
                    colateEntity.bigMiscParticle.GetObject(sweetsBoardView.AppearParticleInstanceTransform);
                particle.Play();
                particle.GetAsyncParticleSystemStoppedTrigger()
                    .ToObservable().FirstOrDefault()
                    .Subscribe(_ =>
                    {
                        colateEntity.bigMiscParticle.ReleaseObject(particle);
                    }).AddTo(particle);
            }
        }

        private void SetVelocityByLiftState(DefaultSweetsLiftView defaultSweetsLiftView)
        {
            switch (defaultSweetsLiftView.liftState)
            {
                case LiftState.Up:
                    if (defaultSweetsLiftView.GetPosition().y>colateEntity.SweetBoardMaxPosY)//高さ制限
                    {
                        defaultSweetsLiftView.SetLiftState(LiftState.UpStay);
                        break;
                    }
                    defaultSweetsLiftView.SetYVelocity(colateEntity.SweetBoardMoveSpeed);
                    break;
                case LiftState.Down:
                    if (defaultSweetsLiftView.GetPosition().y<colateEntity.SweetBoardMinPosY)//高さ制限
                    {
                        defaultSweetsLiftView.SetLiftState(LiftState.DownStay);
                        break;
                    }
                    defaultSweetsLiftView.SetYVelocity(-colateEntity.SweetBoardMoveSpeed);
                    break;
                case LiftState.UpStay:
                    if (defaultSweetsLiftView.currentStayDuration>colateEntity.SweetBoardMoveStayDuration)//時間制限
                    {
                        defaultSweetsLiftView.SetCurrentDuration(0);
                        defaultSweetsLiftView.SetLiftState(LiftState.Down);
                        break;
                    }
                    defaultSweetsLiftView.SetCurrentDuration(defaultSweetsLiftView.currentStayDuration+Time.deltaTime);
                    defaultSweetsLiftView.SetYVelocity(0);
                    break;
                case LiftState.DownStay:
                    if (defaultSweetsLiftView.currentStayDuration>colateEntity.SweetBoardMoveStayDuration)//時間制限
                    {
                        defaultSweetsLiftView.SetCurrentDuration(0);
                        defaultSweetsLiftView.SetLiftState(LiftState.Up);
                        break;
                    }
                    defaultSweetsLiftView.SetCurrentDuration(defaultSweetsLiftView.currentStayDuration+Time.deltaTime);
                    defaultSweetsLiftView.SetYVelocity(0);
                    break;
                default:
                    Debug.LogError($"Not Find State:{defaultSweetsLiftView.liftState}");
                    return;
            }
        }
    }
}