using System;
using InGame.Player.Installer;
using InGame.Stage.View;
using InGame.Player.View;
using UnityEngine;

namespace MyApplication
{
    [Serializable]
    public class CandyParameter
    {
        public CandyView Prefab => prefab;
        [SerializeField] private CandyView prefab;
    }
    
    [Serializable]
    public class MashParameter
    {
        public MashView Prefab => prefab;
        public MashNekoView MashNekoPrefab => mashNekoPrefab;
        public float MashNekoAliveTime => mashNekoAliveTime;
        public Vector2 ToMashNekoInstanceVec => toMashNekoInstanceVec;

        [SerializeField] private MashView prefab;
        [SerializeField] private MashNekoView mashNekoPrefab;
        [SerializeField] private float mashNekoAliveTime;
        [SerializeField] private Vector2 toMashNekoInstanceVec;
    }
    
    [Serializable]
    public class FuParameter
    {
        public FuView Prefab => prefab;
        public BindGumView BindGumView => bindGumView;
        public float InflateGumTime => inflateGumTime;
        public float GumMoveToEnemyTime => gumMoveToEnemyTime;
        public Vector2 InflatedGumScale => inflatedGumScale;
        public Vector2 ToGumInstanceVec => toGumInstanceVec;
        public float GumInflateToMoveInterval => gumInflateToMoveInterval;
        public float GumMoveSpeed => gumMoveSpeed;
        public float GumAliveTime => gumAliveTime;
        
        
        //public int SkillDistance => skillDistance;//MEMO: 距離制限を設けるならコメントアウト解除
        
        [SerializeField] private FuView prefab;
        [SerializeField] private BindGumView bindGumView;
        [SerializeField] private float inflateGumTime;
        [SerializeField] private float gumMoveToEnemyTime;
        [SerializeField] private Vector2 inflatedGumScale;
        [SerializeField] private Vector2 toGumInstanceVec;
        [SerializeField] private float gumInflateToMoveInterval;
        [SerializeField] private float gumMoveSpeed;
        [SerializeField] private float gumAliveTime;

        //[SerializeField] private int skillDistance;
    }
    
    [Serializable]
    public class KureParameter
    {
        public KureView Prefab => prefab;
        public int HealValue => healValue;

        [SerializeField] private KureView prefab;
        [SerializeField] private int healValue;
    }
    
    
    //MEMO: Serializableすると、[SerializeField]がついたPlayerStatus型の変数がインスペクターに表示できるようになる。
    [Serializable]
    public class CharacterBaseParameter
    {
        public PlayableCharacter characterType=PlayableCharacter.None;
        public BasePlayerInstaller installer;
        public ParticleSystem punchParticle;
        public ParticleSystem skillParticle;
        public ParticleSystem onJumpParticle;
        public ParticleSystem offJumpParticle;
        public ParticleSystem runParticle;
        public int maxHp=5;
        public float maxSpeed=5;
        public float accelerateRateX=1.02f;
        public float decelerateRateXOnPunching=0.95f;
        public float decelerateRateX=0.89f;
        public float jumpValue=5;
        public float highJumpValue=8;
        public float toGroundDistance=1;
        public float toSweetsDistance=1;
        public float toSlopeDistance=1;
        public float normalSweetsFixingTime=1;
        public float gimmickSweetsFixingTime=1;
        public float knockBackValue=1;
        public float warpDuration=1;
        public float warpPoaOffsetY=1;
        public float maxColliderSizeX=1.3f;

        public CharacterBaseParameter Clone()
        {
            return (CharacterBaseParameter) MemberwiseClone();
        }
    }

    [Serializable]
    public class DoorData
    {
        public DoorType DoorType => doorType;
        public DoorView DoorView => doorView;
        
        [SerializeField] private DoorType doorType;
        [SerializeField] private DoorView doorView;
    }
    
    [Serializable]
    public struct SquareRange
    {
        public float canExistRangeXMin;
        public float canExistRangeXMax;
        public float canExistRangeYMin;
        public float canExistRangeYMax;
    }

    [Serializable]
    public class ColateSprite
    {
        public ColateSpriteType Type => type;
        public GameObject SpriteObj => spriteObj;
        
        [SerializeField] private ColateSpriteType type;
        [SerializeField] private GameObject spriteObj;
    }
    
    [Serializable]
    public class ControllerImage
    {
        public MyInputDeviceType Type=>type;
        public GameObject ImageObj=>imageObj;

        [SerializeField] private MyInputDeviceType type;
        [SerializeField] private GameObject imageObj;
    }
}