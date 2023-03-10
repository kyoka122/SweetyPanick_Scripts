using System;
using System.Threading;
using InGame.Database;
using InGame.Stage.View;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.Entity
{
    //TODO: アニメーション監視から変える
    public class PlayerCommonInStageEntity:IDisposable
    {
        public readonly PlayableCharacter CharacterType;
        
        public IReadOnlyReactiveProperty<bool> IsRunning => _isRunning;
        public IReadOnlyReactiveProperty<bool> IsJumping => _isJumping;
        public IObservable<bool> OnJump => _onJump;
        public IObservable<bool> OnPunch => _onPunch;
        public IObservable<bool> OnSkill => _onSkill;
        public IObservable<bool> OnFall => _onFall;
        public bool IsGround => playerOnGroundType != GroundType.None;
        
        public ParticleSystem runningParticle{ get; private set; }
        public ParticleSystem onJumpParticle{ get; private set; }
        public ParticleSystem offJumpParticle{ get; private set; }
        public ParticleSystem onPunchParticle{ get; private set; }
        public ParticleSystem onSkillParticle{ get; private set; }
        public float playerDirection { get; private set; }
        public bool IsUsingSkill { get; private set; }
        public bool IsFixing { get; private set; }
        public bool IsPunching { get; private set; }
        public ISweets currentFixingSweets { get; private set; }
        public Vector2 prevStandPos { get; private set; }
        public bool isOpeningMenu { get; private set; }
        public GroundType playerOnGroundType { get; private set; }
        public IHighJumpAbleStand highJumpAbleStand { get; private set; }
        public CancellationTokenSource fixingSweetsTokenSource { get; private set; }
        public bool HavingKey => _inGameDatabase.GetInStageData().havingKey;
        
        private readonly ReactiveProperty<bool> _isRunning;
        private readonly ReactiveProperty<bool> _isJumping;
        private readonly Subject<bool> _onJump;
        private readonly Subject<bool> _onPunch;
        private readonly Subject<bool> _onSkill;
        private readonly Subject<bool> _onFall;
        private readonly InGameDatabase _inGameDatabase;
        
        
        public PlayerCommonInStageEntity(PlayableCharacter characterType,InGameDatabase inGameDatabase)
        {
            CharacterType = characterType;
            _isRunning = new ReactiveProperty<bool>();
            _isJumping = new ReactiveProperty<bool>();
            _onJump = new Subject<bool>();
            _onPunch = new Subject<bool>();
            _onSkill = new Subject<bool>();
            _onFall = new Subject<bool>();
            _inGameDatabase = inGameDatabase;
            _inGameDatabase.SetInStageData(new InStageData());
        }
        
        
        public void SetRunningParticle(ParticleSystem newRunningParticle)
        {
            runningParticle = newRunningParticle;
        }
        
        public void SetOnJumpParticle(ParticleSystem newOnJumpParticle)
        {
            onJumpParticle = newOnJumpParticle;
        }
        
        public void SetOffJumpParticle(ParticleSystem newOffJumpParticle)
        {
            offJumpParticle = newOffJumpParticle;
        }
        
        public void SetOnPunchParticle(ParticleSystem newOnPunchParticle)
        {
            onPunchParticle = newOnPunchParticle;
        }
        
        public void SetOnSkillParticle(ParticleSystem newOnSkillParticle)
        {
            onSkillParticle = newOnSkillParticle;
        }

        public void SetPlayerDirection(float direction)
        {
            playerDirection = direction;
        }
        
        public void SetCurrentAnimation(string newAnimationName,PlayableCharacter type)
        {
            SetRunning(newAnimationName);
            CheckDoingAction(newAnimationName,type);
        }

        public void SetIsJumping(bool isJumping)
        {
            _isJumping.Value = isJumping;
        }

        public void OnJumpTrigger()
        {
            _onJump.OnNext(true);
        }

        public void OnPunchTrigger()
        {
            _onPunch.OnNext(true);
        }

        public void OnSkillTrigger()
        {
            _onSkill.OnNext(true);
        }

        public void OffSkillTrigger()
        {
            _onSkill.OnNext(false);
        }
        
        public void OnFallTrigger()
        {
            _onFall.OnNext(true);
        }

        public void SetFixingSweets(ISweets fixingSweets)
        {
            currentFixingSweets = fixingSweets;
        }

        public void SetFixingSweetsTokenSource(CancellationTokenSource tokenSource)
        {
            fixingSweetsTokenSource = tokenSource;
        }

        public void SetPrevStandPos(Vector2 pos)
        {
            prevStandPos = pos;
        }

        public void SetRunning(string newAnimationName)
        {
            _isRunning.Value = newAnimationName == PlayerAnimationName.Run;
        }
        
        public void SetIsOpeningMenu(bool on)
        {
            isOpeningMenu = on;
        }

        private void CheckDoingAction(string animationName,PlayableCharacter type)
        {
            IsPunching = animationName == PlayerAnimationName.GetEachName(type,PlayerAnimationName.Punch);
            IsUsingSkill = animationName == PlayerAnimationName.GetEachName(type,PlayerAnimationName.Skill);
            IsFixing = animationName == PlayerAnimationName.GetEachName(type, PlayerAnimationName.Fix)||
                       animationName == PlayerAnimationName.GetEachName(type, PlayerAnimationName.OnFix)||
                       animationName == PlayerAnimationName.GetEachName(type, PlayerAnimationName.Fixing)||
                       animationName == PlayerAnimationName.GetEachName(type, PlayerAnimationName.Fixed);

        }

        public void SetHighJumpAbleStand(IHighJumpAbleStand newStand)
        {
            highJumpAbleStand = newStand;
        }
        
        public void SetGroundType(GroundType groundType)
        {
            playerOnGroundType = groundType;
        }


        
        public void AddScore(int score)
        {
            InStageData data = _inGameDatabase.GetInStageData();
            data.score += score;
            _inGameDatabase.SetInStageData(data);
        }

        public void SetHavingKey(bool havingKey)
        {
            InStageData data = _inGameDatabase.GetInStageData();
            data.havingKey = havingKey;
            _inGameDatabase.SetInStageData(data);
        }

        public void Dispose()
        {
            _isRunning?.Dispose();
            _isJumping?.Dispose();
            _onJump?.Dispose();
            _onPunch?.Dispose();
            _onSkill?.Dispose();
            _onFall?.Dispose();
            _inGameDatabase?.Dispose();
            fixingSweetsTokenSource?.Dispose();
        }
    }
}