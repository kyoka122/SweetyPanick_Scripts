using System.Collections.Generic;
using System.Linq;
using InGame.Player.Installer;
using MyApplication;
using UnityEngine;

namespace InGame.Database.ScriptableData
{
    //MEMO: ScriptableObjectアセットとして生成し、初期パラメーターを設定する。
    [CreateAssetMenu(fileName = "PlayerScriptableData", menuName = "ScriptableObjects/PlayerScriptableData")]
    public class PlayerScriptableData:ScriptableObject
    {
        [SerializeField] private CharacterBaseParameter candyBaseParameter;
        [SerializeField] private CandyParameter candyParameter;
        [SerializeField] private CharacterBaseParameter mashBaseParameter;
        [SerializeField] private MashParameter mashParameter;
        [SerializeField] private CharacterBaseParameter fuBaseParameter;
        [SerializeField] private FuParameter fuParameter;
        [SerializeField] private CharacterBaseParameter kureBaseParameter;
        [SerializeField] private KureParameter kureParameter;

        public CharacterBaseParameter[] GetAllBaseParameter()
        {
            List<CharacterBaseParameter> data = new List<CharacterBaseParameter>
            {
                candyBaseParameter.Clone(),
                mashBaseParameter.Clone(),
                fuBaseParameter.Clone(),
                kureBaseParameter.Clone()
            };

            return data.Where(characterCommonStatus => characterCommonStatus!= null)
                .Where(registeredData => registeredData.characterType != PlayableCharacter.None)
                .ToArray();
        }
        
        public CharacterBaseParameter GetCommonParameter(PlayableCharacter playableCharacter)
        {
            CharacterBaseParameter selectedStatus =
                GetAllBaseParameter().FirstOrDefault(parameter => parameter.characterType == playableCharacter);
            if (selectedStatus==null)
            {
                Debug.LogWarning($"None PlayerStatus");
            }

            return selectedStatus;
        }
        
        public CandyParameter GetCandyParameter()
        {
            return candyParameter;
        }

        public MashParameter GetMashParameter()
        {
            return mashParameter;
        }

        public FuParameter GetFuParameter()
        {
            return fuParameter;
        }

        public KureParameter GetKureParameter()
        {
            return kureParameter;
        }

        public PlayableCharacter GetType(PlayableCharacter playableCharacter)
        {
            return GetCommonParameter(playableCharacter).characterType;
        }
        
        public BasePlayerInstaller GetInstaller(PlayableCharacter playableCharacter)
        {
            return GetCommonParameter(playableCharacter).installer;
        }

        public int GetHp(PlayableCharacter playableCharacter)
        {
            return GetCommonParameter(playableCharacter).maxHp;
        }

        public float GetMaxSpeed(PlayableCharacter playableCharacter)
        {
            return GetCommonParameter(playableCharacter).maxSpeed;
        }
        
        public float GetAccelerateRateX(PlayableCharacter playableCharacter)
        {
            return GetCommonParameter(playableCharacter).accelerateRateX;
        }
        
        public float GetDecelerateRateX(PlayableCharacter playableCharacter)
        {
            return GetCommonParameter(playableCharacter).decelerateRateX;
        }
    }
}