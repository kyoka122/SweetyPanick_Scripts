using InGame.Player.Installer;
using InGame.Player.View;
using MyApplication;
using UnityEngine;

namespace InGame.Database
{
    public class CharacterCommonConstData
    {
        public PlayableCharacter CharacterType { get; }
        public BasePlayerInstaller Installer { get; }
        //public BasePlayerView Prefab { get; }
        public ParticleSystem PunchParticle { get; }
        public ParticleSystem SkillParticle { get; }
        public ParticleSystem OnJumpParticle { get; }
        public ParticleSystem OffJumpParticle { get; }
        public ParticleSystem RunParticle { get; }

        public CharacterCommonConstData(CharacterBaseParameter characterBaseParameter)
        {
            CharacterType = characterBaseParameter.characterType;
            Installer = characterBaseParameter.installer;
            //Prefab = characterBaseParameter.prefab;
            PunchParticle = characterBaseParameter.punchParticle;
            SkillParticle = characterBaseParameter.skillParticle;
            OnJumpParticle = characterBaseParameter.onJumpParticle;
            OffJumpParticle = characterBaseParameter.offJumpParticle;
            RunParticle = characterBaseParameter.runParticle;
        }
    }
}