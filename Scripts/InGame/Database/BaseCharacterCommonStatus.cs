using InGame.Player.Installer;
using InGame.Player.View;
using MyApplication;
using UnityEngine;

namespace InGame.Database
{
    public abstract class BaseCharacterCommonStatus
    {
        public PlayableCharacter characterType;

        public int maxHp;
        public float maxSpeed;
        public float accelerateRateX;
        public float decelerateRateXOnPunching;
        public float decelerateRateX;
        public float jumpValue;
        public float highJumpValue;
        public float toGroundDistance;
        public float toSweetsDistance;
        public float toSlopeDistance;
        public float normalSweetsFixingTime;
        public float gimmickSweetsFixingTime;
        public float knockBackValue;
        public float warpDuration;
        public float warpPosOffsetY;
        public float maxColliderSizeX;
     
        public BaseCharacterCommonStatus(CharacterBaseParameter characterBaseParameter)
        {
            characterType = characterBaseParameter.characterType;
            maxHp = characterBaseParameter.maxHp;
            maxSpeed = characterBaseParameter.maxSpeed;
            accelerateRateX = characterBaseParameter.accelerateRateX;
            decelerateRateXOnPunching = characterBaseParameter.decelerateRateXOnPunching;
            decelerateRateX = characterBaseParameter.decelerateRateX;
            jumpValue = characterBaseParameter.jumpValue;
            highJumpValue = characterBaseParameter.highJumpValue;
            toGroundDistance = characterBaseParameter.toGroundDistance;
            toSweetsDistance = characterBaseParameter.toSweetsDistance;
            toSlopeDistance = characterBaseParameter.toSlopeDistance;
            normalSweetsFixingTime = characterBaseParameter.normalSweetsFixingTime;
            gimmickSweetsFixingTime = characterBaseParameter.gimmickSweetsFixingTime;
            knockBackValue = characterBaseParameter.knockBackValue*10;
            warpDuration = characterBaseParameter.warpDuration;
            warpPosOffsetY = characterBaseParameter.warpPoaOffsetY;
            maxColliderSizeX=characterBaseParameter.maxColliderSizeX;
        }
        
    }
}