using System;
using System.Threading;
using InGame.Database;
using InGame.Stage.View;
using MyApplication;
using UniRx;
using UnityEngine;
using Utility;

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
        public bool IsGround => onGroundType != GroundType.None;
        
        public ParticleSystem runningParticle{ get; private set; }
        public ParticleSystem onJumpParticle{ get; private set; }
        public ParticleSystem offJumpParticle{ get; private set; }
        public ParticleSystem onPunchParticle{ get; private set; }
        public ParticleSystem onSkillParticle{ get; private set; }
        
        public ObjectPool<ParticleSystem> fixSweetsParticlePool { get; private set; }
        public float playerDirection { get; private set; }
        public bool isUsingSkill { get; private set; }
        public bool isNormalSweetsFixing { get; private set; }
        
        /// <summary>
        /// ここでPunch判定すると、パンチアニメーションの終わり途中でパンチできる
        /// </summary>
        public bool isPunching { get; private set; }
        public ISweets currentFixingSweets { get; private set; }
        public Vector2 prevStandPos { get; private set; }
        public bool isOpeningMenu { get; private set; }
        public GroundType onGroundType { get; private set; }
        public IBoundAble boundAble { get; private set; }
        public CancellationTokenSource fixingSweetsTokenSource { get; private set; }
        public float currentBoundDelayCount { get; private set; }

        public bool HavingKey => _inGameDatabase.GetAllStageData().havingKey;
        
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

        public void SetIsPunching(bool on)
        {
            isPunching = on;
        }

        public void SetIsNormalSweetsFixing(bool on)
        {
            isNormalSweetsFixing = on;
        }

        public void SetIsUsingSkill(bool on)
        {
            isUsingSkill = on;
        }
        
        public void SetHighJumpAbleStand(IBoundAble boundAble)
        {
            this.boundAble = boundAble;
        }
        
        public void SetGroundType(GroundType groundType)
        {
            onGroundType = groundType;
        }

        public void AddCurrentBoundDelayCount()
        {
            currentBoundDelayCount++;
        }
        
        public void ClearCurrentDelayCount()
        {
            currentBoundDelayCount = 0;
        }

        public void SetFixSweetsParticle(ObjectPool<ParticleSystem> newParticlePool)
        {
            fixSweetsParticlePool = newParticlePool;
        }

        
        public void AddScore(int score)
        {
            AllStageData data = _inGameDatabase.GetAllStageData();
            data.score += score;
            _inGameDatabase.SetAllStageData(data);
        }

        public void SetHavingKey(bool havingKey)
        {
            AllStageData data = _inGameDatabase.GetAllStageData();
            data.havingKey = havingKey;
            _inGameDatabase.SetAllStageData(data);
        }

        public void Dispose()
        {
            _isRunning?.Dispose();
            _isJumping?.Dispose();
            _onJump?.Dispose();
            _onPunch?.Dispose();
            _onSkill?.Dispose();
            _onFall?.Dispose();
            fixingSweetsTokenSource?.Dispose();
        }
    }
}