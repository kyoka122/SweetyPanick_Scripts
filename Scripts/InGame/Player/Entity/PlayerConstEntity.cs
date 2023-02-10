using System;
using System.Linq;
using InGame.Common.Database;
using InGame.Database;
using MyApplication;
using UniRx;
using Unity;
using UnityEngine;

namespace InGame.Player.Entity
{
    //TODO: 別Entityに細かく分ける
    //MEMO: プレイ中のステータス管理用クラス
    public class PlayerConstEntity
    {
        public readonly PlayableCharacter Type;
        public Func<Vector2, Vector2> WorldToViewPortPoint => commonDatabase.GetIReadOnlyCameraController()
            .WorldToViewPortPoint;
        public SquareRange ObjectInScreenRange => inGameDatabase.GetStageSettings().ObjectInScreenRange;
        public SquareRange InPlayerGroupRange => inGameDatabase.GetStageSettings().InPlayerGroupRange;
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
        public float GimmickSweetsFixingTime => GetCharacterCommonStatus().gimmickSweetsFixingTime;
        public float NormalSweetsFixingTime => GetCharacterCommonStatus().normalSweetsFixingTime;
        public float KnockBackValue => GetCharacterCommonStatus().knockBackValue;
        public float WarpDuration => GetCharacterCommonStatus().warpDuration;
        public float WarpPosOffsetY => GetCharacterCommonStatus().warpPosOffsetY;
        public float MaxColliderSizeX => GetCharacterCommonStatus().maxColliderSizeX;
        public int HealValue => inGameDatabase.GetKureStatus().healValue;
        public float StageBottom => inGameDatabase.GetStageSettings().StageBottom;
        
        public Vector2 FixSweetsParticleSize(SweetsType type)
        {
            return type==SweetsType.Sweets ? inGameDatabase.GetStageSettings().FixNormalSweetsParticleSize : 
                inGameDatabase.GetStageSettings().FixGimmickSweetsParticleSize;
        }

        public Vector2 GetInstancePositionCaseMoveStage(StageArea type, PlayableCharacter character) =>
            inGameDatabase.GetPlayerInstancePositions(type).GetPosition(character);

        private BaseCharacterCommonStatus GetCharacterCommonStatus()=>inGameDatabase.GetCharacterCommonStatus(Type);
        private CharacterCommonConstData GetCommonCharacterConstData()=>inGameDatabase.GetCharacterConstData(Type);
        
        private readonly InGameDatabase inGameDatabase;
        private readonly CommonDatabase commonDatabase;
        
        public PlayerConstEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase,PlayableCharacter type)
        {
            Type = type;
            this.inGameDatabase = inGameDatabase;
            this.commonDatabase = commonDatabase;
        }

    }
}