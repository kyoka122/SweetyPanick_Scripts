using MyApplication;

namespace InGame.Database
{
    public abstract class BaseCharacterCommonStatus
    {
        public PlayableCharacter characterType;

        public int MaxHp { get; }
        public float MaxSpeed{ get; }
        public float AccelerateRateX{ get; }
        public float DecelerateRateXOnPunching{ get; }
        public float DecelerateRateX{ get; }
        public int BoundDelayCount { get; }
        public float BoundValue { get; }
        public float JumpValue{ get; }
        public float HighJumpValue{ get; }
        public float ToGroundDistance{ get; }
        public float ToSweetsDistance{ get; }
        public float ToSlopeDistance{ get; }
        public float NormalSweetsFixingTime{ get; }
        public float GimmickSweetsFixingTime{ get; }
        public float NormalSweetsSpecialistFixingTime { get; }
        public float GimmickSweetsSpecialistFixingTime { get; }
        public float KnockBackValue{ get; }
        public float WarpDuration{ get; }
        public float WarpPosOffsetY{ get; }
        public float MaxColliderSizeX{ get; }

        public BaseCharacterCommonStatus(CharacterBaseParameter characterBaseParameter)
        {
            characterType = characterBaseParameter.characterType;
            MaxHp = characterBaseParameter.maxHp;
            MaxSpeed = characterBaseParameter.maxSpeed;
            AccelerateRateX = characterBaseParameter.accelerateRateX;
            DecelerateRateXOnPunching = characterBaseParameter.decelerateRateXOnPunching;
            DecelerateRateX = characterBaseParameter.decelerateRateX;
            BoundDelayCount=characterBaseParameter.boundDelayCount;
            BoundValue=characterBaseParameter.boundValue;
            JumpValue = characterBaseParameter.jumpValue;
            HighJumpValue = characterBaseParameter.highJumpValue;
            ToGroundDistance = characterBaseParameter.toGroundDistance;
            ToSweetsDistance = characterBaseParameter.toSweetsDistance;
            ToSlopeDistance = characterBaseParameter.toSlopeDistance;
            NormalSweetsFixingTime = characterBaseParameter.normalSweetsFixingTime;
            GimmickSweetsFixingTime = characterBaseParameter.gimmickSweetsFixingTime;
            NormalSweetsSpecialistFixingTime = characterBaseParameter.normalSweetsSpecialistFixingTime;
            GimmickSweetsSpecialistFixingTime = characterBaseParameter.gimmickSweetsSpecialistFixingTime;
            KnockBackValue = characterBaseParameter.knockBackValue*10;
            WarpDuration = characterBaseParameter.warpDuration;
            WarpPosOffsetY = characterBaseParameter.warpPoaOffsetY;
            MaxColliderSizeX=characterBaseParameter.maxColliderSizeX;
        }
        
    }
}