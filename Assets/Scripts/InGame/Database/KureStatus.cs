using MyApplication;

namespace InGame.Database
{
    public class KureStatus:BaseCharacterCommonStatus
    {
        public int healValue { get; private set; }
        public KureStatus(CharacterBaseParameter characterBaseParameter, KureParameter kureParameter) 
            : base(characterBaseParameter)
        {
            healValue = kureParameter.HealValue;
        }

        public KureStatus Clone()
        {
            return (KureStatus) MemberwiseClone();
        }
    }
}