namespace MyApplication
{
    public static class EnemyAnimatorStateName
    {
        //MEMO: Walkがデフォルト状態とする
        public static readonly string Eat = "eat";
        public static readonly string Idle = "idle";
        public static readonly string Jump = "jump";
        public static readonly string Sleep = "sleep";
        public static readonly string Cry = "cry";
        public static readonly string Bark = "bark";
        public static string Walk = "walk";
    }
    
    public static class EnemyAnimationName
    {
        public static readonly string Bark = "bark";
    }
    
    public static class EnemyAnimationCallbackName
    {
        public static readonly string TriggerBarkVoice = "TriggerBarkVoice";
    }

    public static class SEVolume
    {
        public static float DOG_BARK = 1.9f;
    }
}