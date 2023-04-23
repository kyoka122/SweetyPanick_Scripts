namespace MyApplication
{
    public static class StageAnimatorParameter
    {
        public const string OnOpen = "OnOpen";
        public const string OnClose = "OnClose";
        public static readonly string OnHeal = "OnHeal";
        public static readonly string OnPress = "OnPress";//MarshmallowView
        public static readonly string OnRoll = "OnRoll";//MarshmallowView
    }
    
    public static class StageAnimationClipName
    {
        public const string DoorAnimation = "DoorAnimation";
        public const string KeyDoorAnimation = "KeyDoorAnimation";
        public static readonly string Heal = "HealAnimation";
    }

    public static class StageAnimationCallbackName
    {
        public const string OnCrepeImpact = "OnCrepeImpact";
        public const string OnCrepeSoundLoop = "OnCrepeSoundLoop";
        public const string OffCrepeSoundLoop = "OffCrepeSoundLoop";
    }
}