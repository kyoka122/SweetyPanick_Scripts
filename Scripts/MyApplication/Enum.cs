namespace MyApplication
{
    public enum PlayableCharacter
    {
        //TODO: キャラ4種の名前
        None,
        Candy,
        Mash,
        Fu,
        Kure
    }
    
    public enum PlayableCharacterIndex
    {
        //配列Index用
        Candy,
        Mash,
        Fu,
        Kure
    }

    public enum SweetsType
    {
        None,
        Sweets,
        GimmickSweets
    }

    public enum FromPlayerEvent
    {
        AllPlayerHeal, //MEMO: クレーのスキル使用イベント
    }
    public enum ToPlayerEvent
    {
        ConsumeKureHealPower
    }

    public enum StageEvent
    {
        None,
        EnterFirstStageGoalDoor,
        EnterSecondStageMiddleDoor,
        EnterSecondHiddenStageDoor,
        EnterSecondStageGoalDoor
    }

    public enum GimmickSweets
    {
        //TODO: 名前変更
        First,
        Second
    }
    
    //お菓子の修復状態ステート
    public enum FixState
    {
        None,
        Fixed,
        Fixing,
        Broken,
        Breaking
    }

    public enum DoorType
    {
        None,
        FirstStageGoal,
        SecondStageMiddle,
        SecondHiddenStage,
        SecondStageGoal
    }
    
    public enum StageArea
    {
        None,
        FirstStageFirst,
        SecondStageFirst,
        SecondStageMiddle,
        SecondHiddenStage,
        SecondStageGoal,
        ColateStageFirst,
        ColateStageFinish
    }

    public enum EnemyState
    {
        Idle,
        Walk,
        Eat,
        isDecoyed,
        isPulled,
        Bind,
        ChangeDirection,
        Punched,
        Fly,
        Death
    }

    public enum GumState
    {
        Inflating,
        Move,
        Catching,
        Caught,
        Destroy
    }

    public enum PlayerCustomState
    {
        None,
        PlayerCount,
        Controller,
        Character,
        Finish
    }

    public enum GroundType
    {
        None,
        Default,
        Trampoline
    }

    public enum CameraMode
    {
        None,
        Chase,
        Freeze
    }
    
    public enum ColateState
    {
        None,
        Talking,//会話シーン
        Surface,//空中に向かって浮上している時
        Dropping,//地面に落下している時（空中）
        IsGround,//地面に落下した時
        ThrowEnemies,//ステージに敵を投げ入れているとき
        Drift,//空中を左右に漂っているとき
        Dead,//HPが0のとき
    }

    public enum ColateSpriteType
    {
        Stand,
        RideChocolate,
        Falling,
        Confuse,
    }

    /// <summary>
    /// ダメージを与えた者の種類
    /// </summary>
    public enum Attacker
    {
        Player,
        Crepe
    }

    public enum MyInputDeviceType
    {
        None,
        Keyboard,
        Procon,
        GamePad,
        JoyconLeft,
        JoyconRight
    }

    public enum TalkPartActionType
    {
        EnterBossStage,
    }

}