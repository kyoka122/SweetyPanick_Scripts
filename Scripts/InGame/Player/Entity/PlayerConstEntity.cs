﻿using System;
using InGame.Common.Database;
using InGame.Database;
using MyApplication;
using UnityEngine;

namespace InGame.Player.Entity
{
    //TODO: 別Entityに細かく分ける
    //MEMO: プレイ中のステータス管理用クラス
    public class PlayerConstEntity
    {
        public readonly PlayableCharacter Type;
        public Func<Vector2, Vector2> WorldToViewPortPoint => _commonDatabase.GetReadOnlyCameraController()
            .WorldToViewPortPoint;
        
        public Func<Vector2, Vector2> WorldToScreenPoint => _commonDatabase.GetReadOnlyCameraController()
            .WorldToScreenPoint;
        
        public SquareRange ObjectInScreenRange => _inGameDatabase.GetStageSettings().ObjectInScreenRange;
        public SquareRange InPlayerGroupRange => _inGameDatabase.GetStageSettings().InPlayerGroupRange;
        public ParticleSystem PunchParticle => GetCommonCharacterConstData().PunchParticle;
        public ParticleSystem SkillParticle => GetCommonCharacterConstData().SkillParticle;
        public ParticleSystem OnJumpParticle => GetCommonCharacterConstData().OnJumpParticle;
        public ParticleSystem OffJumpParticle => GetCommonCharacterConstData().OffJumpParticle;
        public ParticleSystem RunParticle => GetCommonCharacterConstData().RunParticle;
        public GameObject TrailEffect => _inGameDatabase.GetUIData().TrailEffect;
        public float ActiveKeyEffectDuration => _inGameDatabase.GetUIData().ActiveKeyEffectDuration;

        public float MaxSpeed => GetCharacterCommonStatus().MaxSpeed;
        public float AccelerateRateX => GetCharacterCommonStatus().AccelerateRateX;
        public float DecelerateRateXOnPunching => GetCharacterCommonStatus().DecelerateRateXOnPunching;
        public float DecelerateRateX => GetCharacterCommonStatus().DecelerateRateX;
        public float JumpValue => GetCharacterCommonStatus().JumpValue;
        public float HighJumpValue => GetCharacterCommonStatus().HighJumpValue;
        public float BoundValue => GetCharacterCommonStatus().BoundValue;
        public float ToGroundDistance => GetCharacterCommonStatus().ToGroundDistance;
        public float ToSweetsDistance => GetCharacterCommonStatus().ToSweetsDistance;
        public float ToSlopeDistance => GetCharacterCommonStatus().ToSlopeDistance;
        public float GimmickSweetsFixingTime => GetCharacterCommonStatus().GimmickSweetsFixingTime;
        public float GimmickSweetsSpecialistFixingTime => GetCharacterCommonStatus().GimmickSweetsSpecialistFixingTime;
        public float NormalSweetsFixingTime => GetCharacterCommonStatus().NormalSweetsFixingTime;
        public float NormalSweetsSpecialistFixingTime => GetCharacterCommonStatus().NormalSweetsSpecialistFixingTime;
        public float KnockBackValue => GetCharacterCommonStatus().KnockBackValue;
        public float WarpDuration => GetCharacterCommonStatus().WarpDuration;
        public float WarpPosOffsetY => GetCharacterCommonStatus().WarpPosOffsetY;
        public float MaxColliderSizeX => GetCharacterCommonStatus().MaxColliderSizeX;
        public int BoundDelayCount => GetCharacterCommonStatus().BoundDelayCount;
        public int HealValue => _inGameDatabase.GetKureStatus().healValue;
        public float StageBottom => _inGameDatabase.GetStageSettings().StageBottom;

        public Vector2 FixSweetsParticleSize(SweetsType type)
        {
            return type==SweetsType.Sweets ? _inGameDatabase.GetStageSettings().FixNormalSweetsParticleSize : 
                _inGameDatabase.GetStageSettings().FixGimmickSweetsParticleSize;
        }

        public Vector2 GetInstancePositionCaseMoveStage(StageArea type, PlayableCharacter character) =>
            _inGameDatabase.GetPlayerInstanceData(type).GetPosition(character);

        private BaseCharacterCommonStatus GetCharacterCommonStatus()=>_inGameDatabase.GetCharacterCommonStatus(Type);
        private CharacterCommonConstData GetCommonCharacterConstData()=>_inGameDatabase.GetCharacterConstData(Type);
        
        private readonly InGameDatabase _inGameDatabase;
        private readonly CommonDatabase _commonDatabase;
        
        public PlayerConstEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase,PlayableCharacter type)
        {
            Type = type;
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
        }

    }
}