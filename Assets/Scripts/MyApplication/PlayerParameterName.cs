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
        public static readonly string OnRevive = "OnRevive";
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
        public static readonly string Damaged = "Damaged";
        public static readonly string Death = "Death";

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
        public static readonly string OnFixParticle = "OnFixParticle";
        public static readonly string Skill = "Skill";
    }

    public static class PlayerEffectAnimatorParameter
    {
        public static readonly string OnChange = "OnChange";//MEMO: PlayerChangeCloudAnimatorのパラメータ
        public static readonly string Appear = "Appear";//MEMO: PlayerChangeCloudAnimatorのパラメータ
    }
    
    public static class PlayerEffectAnimationName
    {
        public static readonly string CloudOpen = "CloudOpenAnimation";
    }
    
    public static class PlayerEffectAnimationCallbackName
    {
        public static readonly string OnPlayer = "OnPlayer";
    }
    
    public static class PlayerKeyAnimationName
    {
        public static string GetKeyName(Key key)
        {
            key = key switch
            {
                Key.MarshmallowJump => Key.Jump,
                Key.EnterDoor => Key.Punch,
                _ => key
            };

            return key.ToString();
        }
        
        public static string GetDeviceName(MyInputDeviceType deviceType)
        {
            switch (deviceType)
            {
                case MyInputDeviceType.Procon:
                case MyInputDeviceType.GamePad:
                    return deviceType.ToString();
                case MyInputDeviceType.JoyconLeft:
                case MyInputDeviceType.JoyconRight:
                    return "Joycon";
            }

            return deviceType.ToString();
        }
        
    }

    public class PlayerName
    {
        public static string GetPlayerNameStrings(PlayableCharacterIndex playableCharacterIndex)
        {
            return playableCharacterIndex switch
            {
                PlayableCharacterIndex.Candy => "キャンディ",
                PlayableCharacterIndex.Mash => "マシュ",
                PlayableCharacterIndex.Fu => "フー",
                PlayableCharacterIndex.Kure => "クレー",
                _ => throw new ArgumentOutOfRangeException(nameof(playableCharacterIndex), playableCharacterIndex, null)
            };
        }
    }
}