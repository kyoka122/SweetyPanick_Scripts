namespace MyApplication
{
    public static class EnemyAnimatorStateName
    {
        //MEMO: Walkがデフォルト状態とする
        //TODO: アニメーションの内容が分かってから消すかどうか決める
        public static readonly string Eat = "eat";
        //public static readonly string Eat2 = "eat2";
        public static readonly string Idle = "idle";
        //public static readonly string Idle2 = "idle2";
        public static readonly string Jump = "jump";
        //public static readonly string Jump2 = "jump2";
        public static readonly string Sleep = "sleep";
        //public static readonly string Sleep2 = "sleep2";
        //public static readonly string Walk = "walk"; //MEMO: 今の所walkはデフォルト状態なので、使用しない
        //public static readonly string Walk2 = "walk2";
        public static readonly string Cry = "cry";
    }
}