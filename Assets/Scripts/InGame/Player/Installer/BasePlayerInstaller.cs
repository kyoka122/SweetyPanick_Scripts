using InGame.Common.Database;
using InGame.Database;
using InGame.Player.Controller;
using InGame.Player.View;
using MyApplication;
using OutGame.Database;
using UnityEngine;

namespace InGame.Player.Installer
{
    public abstract class BasePlayerInstaller:MonoBehaviour
    {
        [SerializeField] protected ViewGenerator viewGenerator;
        [SerializeField] protected ParticleGeneratorView particleGeneratorView;
        [SerializeField] protected ParticleGeneratorView healHpBarParticleGeneratorView;
        [SerializeField] protected CallbackAnimatorGeneratorView reviveCharacterCallbackAnimatorView;
        
        
        public abstract BasePlayerController Install(int playerNum, StageArea stageArea,InGameDatabase inGameDatabase
            ,OutGameDatabase outGameDatabase,CommonDatabase commonDatabase);
    }
}