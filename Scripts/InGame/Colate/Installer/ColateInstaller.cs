using System;
using System.Collections.Generic;
using InGame.Colate.Entity;
using InGame.Colate.Logic;
using InGame.Colate.Manager;
using InGame.Colate.View;
using InGame.Database;
using InGame.Player.View;
using UnityEngine;
using Utility;

namespace InGame.Colate.Installer
{
    public class ColateInstaller:MonoBehaviour
    {
        [SerializeField] private ViewGenerator viewGenerator;
        [SerializeField] protected ParticleGeneratorView bigMiscParticleGeneratorView;
        [SerializeField] protected ParticleGeneratorView smallMiscParticleGeneratorView;
        
        public ColateController Install(InGameDatabase inGameDatabase,Func<Vector2, IColateOrderAble> spawnEnemyEvent,
            DefaultSweetsLiftView[] sweetsLifts)
        {
            var colateConstEntity=new ColateEntity(inGameDatabase,
                new ObjectPool<ParticleSystem>(bigMiscParticleGeneratorView),
                new ObjectPool<ParticleSystem>(smallMiscParticleGeneratorView));
            
            ColateView colateView = FindObjectOfType<ColateView>();
            if (colateView==null)
            {
                Debug.LogError($"Couldn`t Find ColateView.");
            }
            colateView.Init();
            
            StageUIData stageUIData = inGameDatabase.GetUIData();
            ColateStatusView colateStatusView = viewGenerator.GenerateColateStatusView(stageUIData.ColateStatusView,
                stageUIData.Canvas.transform,stageUIData.ColateStatusDataPos);
            colateStatusView.Init(inGameDatabase.GetColateData().MaxHp);
            colateStatusView.SetActive(false);
            
            foreach (var sweetsLiftView in sweetsLifts)
            {
                sweetsLiftView.Init();
            }

            var colateStateLogic = new TalkingState(colateConstEntity, colateView, colateStatusView, spawnEnemyEvent,sweetsLifts);
            var colateStatusLogic = new ColateStatusLogic(colateConstEntity,colateStatusView,colateView);
            var disposables = new List<IDisposable> {colateConstEntity,colateView};
            
            return new ColateController(colateStateLogic, colateStatusLogic,disposables.ToArray());
        }
    }
}