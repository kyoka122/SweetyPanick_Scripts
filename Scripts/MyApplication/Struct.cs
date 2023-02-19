using UnityEngine;

namespace MyApplication
{
    public class Struct
    {
        public struct DamagedInfo
        {
            public readonly Attacker attacker;
            public readonly Vector2 attackerPos;

            public DamagedInfo(Attacker attacker, Vector2 attackerPos)
            {
                this.attacker = attacker;
                this.attackerPos = attackerPos;
            }
        }
    }
}