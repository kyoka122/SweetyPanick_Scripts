using System;
using System.Collections.Generic;
using InGame.Player.Logic;
using MyApplication;

namespace InGame.Player.Controller
{
    public class CandyController : BasePlayerController
    {
        public CandyController(int playerNum,PlayerMoveLogic playerMoveLogic, PlayerJumpLogic playerJumpLogic,
            PlayerPunchLogic playerPunchLogic, BasePlayerSkillLogic playerSkillLogic,
            PlayerReShapeLogic playerReShapeLogic, PlayerHealLogic playerHealLogic,
            PlayerStatusLogic playerStatusLogic, PlayerParticleLogic playerParticleLogic, 
            PlayerFixSweetsLogic playerFixSweetsLogic, PlayerEnterDoorLogic playerEnterDoorLogic,
            PlayableCharacterSelectLogic playableCharacterSelectLogic,PlayerTalkLogic playerTalkLogic,
            List<IDisposable> disposables,IObservable<bool> onDead) 
            : base(playerNum,playerMoveLogic, playerJumpLogic, playerPunchLogic, playerSkillLogic, playerReShapeLogic, 
                playerHealLogic, playerStatusLogic, playerParticleLogic, playerFixSweetsLogic, playerEnterDoorLogic,
                playableCharacterSelectLogic,playerTalkLogic,disposables,onDead)
        {
        }

        protected override void FixedUpdateEachPlayer()
        {
            //MEMO: 今の所特になし
        }

        public override void RegisterPlayerEvent(Action<FromPlayerEvent> playerEvent)
        {
            //MEMO: 今の所特になし
        }
    }
}