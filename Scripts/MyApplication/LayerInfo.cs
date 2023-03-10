using UnityEngine;

namespace MyApplication
{
    public static class LayerInfo
    {
        public static readonly int UINum = 5;
        public static readonly int StageRangeNum = 7;
        public static readonly int GroundNum = 8;
        public static readonly int PlayerNum = 9;
        public static readonly int PlayerWeaponNum = 10;
        public static readonly int SweetsNum = 11;
        public static readonly int DoorNum = 12;
        public static readonly int EnemyNum = 13;
        public static readonly int EnemyFlyingNum = 14;
        public static readonly int PlayerSkillGoodsNum = 15;
        public static readonly int TrampolineNum = 16;
        public static readonly int NotCollideEnemyPlayerNum = 17;
        public static readonly int SideWallNum = 18;

        public static readonly LayerMask UIMask = 1 << UINum;
        public static readonly LayerMask GroundMask = 1 << GroundNum | 1 << TrampolineNum;
        public static readonly LayerMask SweetsMask = 1<<SweetsNum|1 << TrampolineNum;
        public static readonly LayerMask DoorMask = 1<<DoorNum;
        public static readonly LayerMask PlayerWeaponMask = 1<<PlayerWeaponNum;
    }
}