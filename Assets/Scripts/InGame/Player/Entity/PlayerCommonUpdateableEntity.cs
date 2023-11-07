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
        public IObservable<bool> OnIsInStage => _onIsInStageSubject.SkipLatestValueOnSubscribe();
        public bool IsDead => _inGameDatabase.GetPlayerUpdateableData(_type).isDead;
        public IObservable<bool> OnReviving => _onRevivingSubject.SkipLatestValueOnSubscribe();
        public bool IsReviving => _inGameDatabase.GetPlayerUpdateableData(_type).isReviving;
        public bool canAttackedByEnemy { get; private set; } = true;
        public bool IsUsed=> _inGameDatabase.GetPlayerUpdateableData(_type).isUsed;
        public bool IsInStage => _inGameDatabase.GetPlayerUpdateableData(_type).isInStage;
        public int PlayerNum => _inGameDatabase.GetPlayerUpdateableData(_type).playerNum;
        public float CurrentHp=> _inGameDatabase.GetPlayerUpdateableData(_type).currentHp;
        public bool HavingKey=> _inGameDatabase.GetAllStageData().havingKey;

        public float NearnessFromTargetView => _inGameDatabase.GetCharacterInStageData(_type).nearnessFromTargetView;
        public bool IsWarping => _inGameDatabase.GetCharacterInStageData(_type).isWarping;

        //TODO: キャラ切り替えできるようになったら条件を変える
        public int LivingPlayerCount => Mathf.Min(
            _commonDatabase.GetMaxPlayerCount(),
            _inGameDatabase.GetAllPlayerUpdateableData().Where(data=>data!=null).Count(data => data.isInStage));
        
        public int LivingPlayer => 
            _inGameDatabase.GetAllPlayerUpdateableData().Where(data=>data!=null).Count(data => data.isUsed);
        
        public int LivingCharacterCount => _inGameDatabase.GetAllPlayerUpdateableData()
            .Count(data => data.isDead == false);
        
        public bool IsOtherPlayerWarping  => _inGameDatabase.GetAllCharacterInStageData()
            .Any(status=>status.Value.isWarping);

        public Transform GetEachTransform(PlayableCharacter type) => _inGameDatabase.GetCharacterInStageData(type).transform;
        public PlayerUpdateableData GetPlayerUpdateableData(PlayableCharacter type) => _inGameDatabase.GetPlayerUpdateableData(type);
        public bool IsUsedEachPlayer(PlayableCharacter type) => _inGameDatabase.GetPlayerUpdateableData(type).isUsed;
        public Transform GetCanvasTransform => _inGameDatabase.GetUIData().Canvas.transform;
        
        public bool HadUsedFirstActionKey(Key key)
            => _inGameDatabase.GetFirstActionKeyData(_type).Contains(key);
        
        private readonly InGameDatabase _inGameDatabase;
        private readonly CommonDatabase _commonDatabase;
        private readonly PlayableCharacter _type;
        private readonly ReactiveProperty<bool> _onIsInStageSubject;
        private readonly ReactiveProperty<bool> _onRevivingSubject;
        
        public PlayerCommonUpdateableEntity(InGameDatabase inGameDatabase,CommonDatabase commonDatabase,
            PlayableCharacter type,int playerNum, Transform playerTransform)
        {
            _type = type;
            _inGameDatabase = inGameDatabase;
            _commonDatabase = commonDatabase;
            _onIsInStageSubject = new ReactiveProperty<bool>(IsInStage);
            _onRevivingSubject = new ReactiveProperty<bool>(IsReviving);
            InitDatabase(playerTransform);
        }

        private void InitDatabase(Transform playerTransform)
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
            canAttackedByEnemy = canDamage && !IsDead;
        }

        private void Damage(int damageValue)
        {
            PlayerUpdateableData updateableData = _inGameDatabase.GetPlayerUpdateableData(_type);
            updateableData.currentHp -= damageValue;
            if (updateableData.currentHp<=0)
            {
                Debug.Log($"Dead!!!!!");
                updateableData.isDead = true;
            }
            _inGameDatabase.SetPlayerUpdateableData(_type,updateableData);
        }

        public void SetCurrentHp(float hp)
        {
            PlayerUpdateableData updateableData =  _inGameDatabase.GetPlayerUpdateableData(_type);
            updateableData.currentHp = hp;
            _inGameDatabase.SetPlayerUpdateableData(_type,updateableData);
        }
        
        public void SetIsReviving(bool isReviving)
        {
            Debug.Log($"isReviving:{isReviving}");
            PlayerUpdateableData updateableData =  _inGameDatabase.GetPlayerUpdateableData(_type);
            updateableData.isReviving = isReviving;
            _onRevivingSubject.Value = isReviving;
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
        public void SetOnInStageCharacter(bool use)
        {
            PlayerUpdateableData updateableData = _inGameDatabase.GetPlayerUpdateableData(_type);
            Debug.Log($"InStageCharacter");
            _onIsInStageSubject.Value = use;
            updateableData.isInStage = use;
            _inGameDatabase.SetPlayerUpdateableData(_type,updateableData);
        }

        public void SetHealedParameter()
        {
            PlayerUpdateableData updateableData = _inGameDatabase.GetPlayerUpdateableData(_type);
            updateableData.isDead = false;
            _onIsInStageSubject.Value = true;
            updateableData.isInStage = true;
            _inGameDatabase.SetPlayerUpdateableData(_type,updateableData);
        }
        
        public void SetRevivedParameter()
        {
            PlayerUpdateableData updateableData = _inGameDatabase.GetPlayerUpdateableData(_type);
            
            _inGameDatabase.SetPlayerUpdateableData(_type,updateableData);
        }

        public void SetHavingKey()
        {
            var data = _inGameDatabase.GetAllStageData();
            data.havingKey = true;
            _inGameDatabase.SetAllStageData(data);
        }

        public void AddUsedFirstActionKey(Key key)
        {
            _inGameDatabase.AddFirstActionKeyData(_type,key);
        }


        public void Dispose()
        {
            _onIsInStageSubject?.Dispose();
            _onRevivingSubject?.Dispose();
        }
    }
}