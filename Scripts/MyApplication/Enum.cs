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
        EnterFirstStageMiddleDoor,
        EnterFirstHiddenStageDoor,
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
        FirstStageMiddle,
        FirstHiddenStage,
        FirstStageGoal,
        SecondStageMiddle,
        SecondHiddenStage,
        SecondStageGoal
    }
    
    public enum StageArea
    {
        None,
        FirstStageFirst,
        FirstStageMiddle,
        FirstHiddenStage,
        SecondStageFirst,
        SecondStageMiddle,
        SecondHiddenStage,
        SecondStageGoal
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
        Catched,
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


}