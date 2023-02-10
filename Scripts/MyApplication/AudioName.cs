using System;
using KanKikuchi.AudioManager;

namespace MyApplication
{
    public static class AudioName
    {
        public static string GetAttackPath(PlayableCharacter type)
        {
            return type switch
            {
                PlayableCharacter.None => null,
                PlayableCharacter.Candy => SEPath.KURE_ATTACK,
                PlayableCharacter.Mash => SEPath.MASH_ATTACK,
                PlayableCharacter.Fu => SEPath.FU_ATTACK,
                PlayableCharacter.Kure => SEPath.KURE_ATTACK,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        /*public static string GetFixPath(PlayableCharacter type)
        {
            return type switch
            {
                PlayableCharacter.None => null,
                PlayableCharacter.Candy => SEPath.,
                PlayableCharacter.Mash => SEPath.,
                PlayableCharacter.Fu => SEPath.,
                PlayableCharacter.Kure => SEPath.KURE_FIXED,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }*/
    }
}