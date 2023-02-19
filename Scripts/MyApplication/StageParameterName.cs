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
        public static readonly string Heal = "HealAnimation";
    }

    public static class StageAnimationCallbackName
    {
        public const string OnCrepeCameraShake = "OnCrepeCameraShake";
    }
}