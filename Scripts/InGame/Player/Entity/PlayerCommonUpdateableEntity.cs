using System;
using System.Linq;
using InGame.Common.Database;
using InGame.Database;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.Entity
{
    public class PlayerCommonUpdateableEntity:IDisposable
    {
        public IObservable<bool> OnUse => _onUseSubject;
        public IObservable<bool> OnDead => _onDeadSubject;
        public bool IsDead => CurrentHp == 0;

        public bool canAttackedByEnemy { get; private set; } = true;
        public int PlayerNum => _inGameDatabase.GetPlayerUpdateableData(_type).playerNum;
        public int CurrentHp=> _inGameDatabase.GetPlayerUpdateableData(_type).currentHp;
        public bool HavingKey=> _inGameDatabase.GetAllStageData().havingKey;

        public float NearnessFromTargetView => _inGameDatabase.GetCharacterInStageData(_type).nearnessFromTargetView;
        public bool IsWarping => _inGameDatabase.GetCharacterInStageData(_type).isWarping;

        public int LivingPlayerCount => Mathf.Min(
            _commonDatabase.GetMaxPlayerCount(),
            _inGameDatabase.GetAllPlayerUpdateableData().Where(data=>data!=null).Count(data => data.isDead == false));
        
        public int LivingCharacterCount => _inGameDatabase.GetAllPlayerUpdateableData()
            .Count(data => data.isDead == false);
        
        public bool IsOtherPlayerWarping  => _inGameDatabase.GetAllCharacterInStageData()
            .Any(status=>status.Value.isWarping);

        public Transform GetEachTransform(PlayableCharacter type) => _inGameDatabase.GetCharacterInStageData(type).transform;
        
        private readonly InGameDatabase _inGameDatabase;
        private readonly CommonDatabase _commonDatabase;
        private readonly PlayableCharacter _type;
        private readonly ReactiveProperty<bool> _onDeadSubject;
        private readonly ReactiveProperty<bool> _onUseSubject;
        
        public PlayerCommonUpdateableEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase,
            PlayableCharacter type,int playerNum, Transform playerTransform)
        {
            _onDeadSubject = new ReactiveProperty<bool>();
            _onUseSubject = new ReactiveProperty<bool>(true);
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            _type = type;
            InitDatabase(playerNum,playerTransform);
        }

        private void InitDatabase(int playerNum,Transform playerTransform)
        {
            _inGameDatabase.SetCharacterInStageData(_type,new CharacterUpdateableInStageData(playerTransform));
        }


        public void SetPlayerNum(int newPlayerNum)
        {
            PlayerUpdateableData updateableData =  _inGameDatabase.GetPlayerUpdateableData(_type);
            updateableData.playerNum += newPlayerNum;
            Debug.Log($"playerNum:{PlayerNum}: currentHp={updateableData.currentHp}");
            _inGameDatabase.SetPlayerUpdateableData(_type,updateableData);
        }

        public void DamageDefault()
        {
            Debug.Log($"DamageDefault");
            Damage(1);
        }

        public void SetCanDamageFlag(bool canDamage)
        {
            canAttackedByEnemy = canDamage && !_onDeadSubject.Value;
        }
               
        public void Damage(int damageValue)
        {
            PlayerUpdateableData updateableData = _inGameDatabase.GetPlayerUpdateableData(_type);
            updateableData.currentHp -= damageValue;
            if (updateableData.currentHp<=0)
            {
                _onDeadSubject.Value = true;
                Debug.Log($"Dead!!!!!");
            }
            updateableData.isDead = true;
            _inGameDatabase.SetPlayerUpdateableData(_type,updateableData);
        }

        public void HealHp(int healValue)
        {
            PlayerUpdateableData updateableData =  _inGameDatabase.GetPlayerUpdateableData(_type);
            updateableData.currentHp += healValue;
            Debug.Log($"playerNum:{PlayerNum}: currentHp={updateableData.currentHp}");
            _inGameDatabase.SetPlayerUpdateableData(_type,updateableData);
        }

        public void SetInNearnessFromTargetView(float inTargetScreen)
        {
            var playerInStageData = _inGameDatabase.GetCharacterInStageData(_type);
            playerInStageData.nearnessFromTargetView = inTargetScreen;
            _inGameDatabase.SetCharacterInStageData(_type,playerInStageData);
        }

        public void SetWarping(bool on)
        {
            var playerInStageData = _inGameDatabase.GetCharacterInStageData(_type);
            playerInStageData.isWarping = on;
            _inGameDatabase.SetCharacterInStageData(_type,playerInStageData);
        }

        public void SetCanTarget(bool canTarget)
        {
            var playerInStageData = _inGameDatabase.GetCharacterInStageData(_type);
            playerInStageData.canTargetCamera = canTarget;
            _inGameDatabase.SetCharacterInStageData(_type,playerInStageData);
        }

        /// <summary>
        /// このキャラを使用し始める場合と使用し終わる場合（キャラクターチェンジの際など）に呼び出し
        /// </summary>
        /// <param name="use"></param>
        public void SetOnUseCharacter(bool use)
        {
            _onUseSubject.Value = use;
        }

        public void SetHavingKey()
        {
            var data = _inGameDatabase.GetAllStageData();
            data.havingKey = true;
            _inGameDatabase.SetAllStageData(data);
        }
        
        public void Dispose()
        {
            _onDeadSubject?.Dispose();
            _onUseSubject?.Dispose();
        }
    }
}