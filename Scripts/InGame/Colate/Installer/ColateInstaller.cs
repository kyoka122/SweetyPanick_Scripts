using System;
using System.Collections.Generic;
using InGame.Colate.Entity;
using InGame.Colate.Logic;
using InGame.Colate.Manager;
using InGame.Colate.View;
using InGame.Database;
using InGame.Enemy.Interface;
using UnityEngine;

namespace InGame.Colate.Installer
{
    public class ColateInstaller:MonoBehaviour
    {
        [SerializeField] private ViewGenerator viewGenerator;
        
        public ColateController Install(InGameDatabase inGameDatabase,Func<Vector2, IColateOrderAble> spawnEnemyEvent)
        {
            var colateConstEntity=new ColateEntity(inGameDatabase);
            ColateView colateView = FindObjectOfType<ColateView>();
            if (colateView==null)
            {
                Debug.LogError($"Couldn`t Find ColateView.");
            }
            colateView.Init();
            UIData uiData = inGameDatabase.GetUIData();
            ColateStatusView colateStatusView = viewGenerator.GenerateColateStatusView(uiData.ColateStatusView,
                uiData.Canvas.transform,uiData.ColateStatusDataPos);
            var colateStateLogic = new TalkingState(colateConstEntity, colateView, colateStatusView, spawnEnemyEvent);

            var disposables = new List<IDisposable> {colateConstEntity,colateView};
            
            return new ColateController(colateStateLogic, disposables.ToArray());
        }
    }
}