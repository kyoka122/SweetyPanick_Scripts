using System;
using System.Linq;
using InGame.Database;
using MyApplication;
using UniRx;
using UnityEngine;

namespace InGame.Player.Entity
{
    public class PlayerCommonUpdateableEntity:IDisposable
    {
        public IObservable<bool> OnDead => onDeadSubject;
        public bool IsDead => CurrentHp == 0;

        public bool canAttackedByEnemy { get; private set; } = true;
        public int PlayerNum => _inGameDatabase.GetPlayerUpdatedData(_type).playerNum;
        public int CurrentHp=> _inGameDatabase.GetPlayerUpdatedData(_type).currentHp;

        public float NearnessFromTargetView => _inGameDatabase.GetCharacterInStageData(_type).nearnessFromTargetView;
        public bool IsWarping => _inGameDatabase.GetCharacterInStageData(_type).isWarping;
            
        public bool IsOtherPlayerWarping  => _inGameDatabase.GetAllCharacterInStageData()
            .Any(status=>status.isWarping);

        public Transform GetEachTransform(PlayableCharacter type) => _inGameDatabase.GetCharacterInStageData(type)?.transform;
        
        private readonly InGameDatabase _inGameDatabase;
        private readonly PlayableCharacter _type;
        private readonly ReactiveProperty<bool> onDeadSubject;
        
        public PlayerCommonUpdateableEntity(InGameDatabase inGameDatabase,PlayableCharacter type,int playerNum,
            Transform playerTransform)
        {
            onDeadSubject = new ReactiveProperty<bool>();
            _inGameDatabase = inGameDatabase;
            _type = type;
            InitDatabase(playerNum,playerTransform);
        }

        private void InitDatabase(int playerNum,Transform playerTransform)
        {
            _inGameDatabase.SetCharacterInStageData(_type,new CharacterUpdateableInStageData(playerTransform));

            var data = new PlayerUpdateableData(playerNum,_inGameDatabase.GetCharacterCommonStatus(_type).maxHp,false);
            _inGameDatabase.SetPlayerUpdateableData(_type, data);

        }


        public void SetPlayerNum(int newPlayerNum)
        {
            PlayerUpdateableData updateableData =  _inGameDatabase.GetPlayerUpdatedData(_type);
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
            canAttackedByEnemy = canDamage && !onDeadSubject.Value;
        }
               
        public void Damage(int damageValue)
        {
            PlayerUpdateableData updateableData = _inGameDatabase.GetPlayerUpdatedData(_type);
            updateableData.currentHp -= damageValue;
            if (updateableData.currentHp<=0)
            {
                onDeadSubject.Value = true;
                Debug.Log($"Dead!!!!!");
            }

            Debug.Log($"IsDead: {IsDead}");
            updateableData.isDead = IsDead;
            _inGameDatabase.SetPlayerUpdateableData(_type,updateableData);
        }

        public void HealHp(int healValue)
        {
            PlayerUpdateableData updateableData =  _inGameDatabase.GetPlayerUpdatedData(_type);
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

        public void Dispose()
        {
            onDeadSubject?.Dispose();
        }
    }
}