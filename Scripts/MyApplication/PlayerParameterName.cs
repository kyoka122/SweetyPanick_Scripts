using System;

namespace MyApplication
{
    public static class PlayerAnimatorParameter
    {
        public static readonly string VerticalMove = "VerticalMove";
        public static readonly string IsVerticalMove = "IsVerticalMove";
        public static readonly string HorizontalMove = "HorizontalMove";
        public static readonly string IsHorizontalMove = "IsHorizontalMove";
        public static readonly string OnPunch = "OnPunch";
        public static readonly string OnSkill = "OnSkill";
        public static readonly string OnFix = "OnFix";
        public static readonly string Fixing = "Fixing";
        public static readonly string Fixed = "Fixed";
        public static readonly string OnDamaged = "OnDamaged";
        public static readonly string OnDeath = "OnDeath";
        public static readonly string ForceIdle = "ForthIdle";
        
    }

    public static class PlayerAnimationName
    {
        public static readonly string Punch = "Attack";
        public static readonly string Down = "Down";
        public static readonly string Jump = "Jump";
        public static readonly string Run = "Run";
        public static readonly string Skill = "Skill";
        public static readonly string Wait = "Wait";
        public static readonly string Walk = "Walk";
        public static readonly string Yawn = "Yawn";
        public static readonly string Fix = "Fix";
        public static readonly string OnFix = "GimmickFixStart";
        public static readonly string Fixing = "GimmickFix";
        public static readonly string Fixed = "GimmickFixEnd";
        public static readonly string OnDamaged = "OnDamaged";
        public static readonly string OnDeath = "OnDeath";

        public static string GetEachName(PlayableCharacter character,string actionName)
        {
            return character switch
            {
                PlayableCharacter.None => null,
                PlayableCharacter.Candy => "Candy" + actionName,
                PlayableCharacter.Mash => "Mash" + actionName,
                PlayableCharacter.Fu => "Fu" + actionName,
                PlayableCharacter.Kure => "Kure" + actionName,
                _ => throw new ArgumentOutOfRangeException(nameof(character), character, null)
            };
        }
    }
    
    public static class PlayerAnimationEventName
    {
        public static readonly string Punch = "Punch";
        public static readonly string Skill = "Skill";
    }

    public static class PlayerEffectAnimatorParameter
    {
        public static readonly string OnChange = "OnChange";//MEMO: PlayerChangeCloudAnimatorのパラメータ
    }
    
    public static class PlayerEffectAnimationName
    {
        public static readonly string PlayerChangeCloudAnimation = "PlayerChangeCloudAnimation";
    }
}