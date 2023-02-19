using System;
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
        public SquareRange ObjectInScreenRange => _inGameDatabase.GetStageSettings().ObjectInScreenRange;
        public SquareRange InPlayerGroupRange => _inGameDatabase.GetStageSettings().InPlayerGroupRange;
        public ParticleSystem PunchParticle => GetCommonCharacterConstData().PunchParticle;
        public ParticleSystem SkillParticle => GetCommonCharacterConstData().SkillParticle;
        public ParticleSystem OnJumpParticle => GetCommonCharacterConstData().OnJumpParticle;
        public ParticleSystem OffJumpParticle => GetCommonCharacterConstData().OffJumpParticle;
        public ParticleSystem RunParticle => GetCommonCharacterConstData().RunParticle;

        public float MaxSpeed => GetCharacterCommonStatus().maxSpeed;
        public float AccelerateRateX => GetCharacterCommonStatus().accelerateRateX;
        public float DecelerateRateXOnPunching => GetCharacterCommonStatus().decelerateRateXOnPunching;
        public float DecelerateRateX => GetCharacterCommonStatus().decelerateRateX;
        public float JumpValue => GetCharacterCommonStatus().jumpValue;
        public float HighJumpValue => GetCharacterCommonStatus().highJumpValue;
        public float ToGroundDistance => GetCharacterCommonStatus().toGroundDistance;
        public float ToSweetsDistance => GetCharacterCommonStatus().toSweetsDistance;
        public float ToSlopeDistance => GetCharacterCommonStatus().toSlopeDistance;
        public float GimmickSweetsFixingTime => GetCharacterCommonStatus().gimmickSweetsFixingTime;
        public float NormalSweetsFixingTime => GetCharacterCommonStatus().normalSweetsFixingTime;
        public float KnockBackValue => GetCharacterCommonStatus().knockBackValue;
        public float WarpDuration => GetCharacterCommonStatus().warpDuration;
        public float WarpPosOffsetY => GetCharacterCommonStatus().warpPosOffsetY;
        public float MaxColliderSizeX => GetCharacterCommonStatus().maxColliderSizeX;
        public int HealValue => _inGameDatabase.GetKureStatus().healValue;
        public float StageBottom => _inGameDatabase.GetStageSettings().StageBottom;
        
        public Vector2 FixSweetsParticleSize(SweetsType type)
        {
            return type==SweetsType.Sweets ? _inGameDatabase.GetStageSettings().FixNormalSweetsParticleSize : 
                _inGameDatabase.GetStageSettings().FixGimmickSweetsParticleSize;
        }

        public Vector2 GetInstancePositionCaseMoveStage(StageArea type, PlayableCharacter character) =>
            _inGameDatabase.GetPlayerInstancePositions(type).GetPosition(character);

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