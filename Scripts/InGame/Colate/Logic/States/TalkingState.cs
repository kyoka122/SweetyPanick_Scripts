﻿using System;
using InGame.Colate.Entity;
using InGame.Colate.View;
using InGame.Enemy.Interface;
using MyApplication;
using UnityEngine;

namespace InGame.Colate.Logic
{
    public class TalkingState:BaseColateStateLogic
    {
        public override ColateState state => ColateState.Talking;
        
        public TalkingState(ColateEntity colateEntity, ColateView colateView,ColateStatusView colateStatusView,
            Func<Vector2, IColateOrderAble> spawnEnemyEvent) : base(colateEntity, colateView,colateStatusView,spawnEnemyEvent)
        {
        }

        protected override void Enter()
        {
            colateView.SetSprite(ColateSpriteType.Stand);
            base.Enter();
        }

        protected override void Update()
        {
            if (!isTalking)
            {
                nextStateInstance = new SurfaceState(colateEntity, colateView, colateStatusView, spawnEnemyEvent);
                stage = Event.Exit;
            }
        }
    }
}