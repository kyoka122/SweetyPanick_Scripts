using System;
using System.Collections.Generic;
using InGame.Player.Logic;
using MyApplication;

namespace InGame.Player.Controller
{
    public class FuController:BasePlayerController
    {
        public FuController(int playerNum, PlayerMoveLogic playerMoveLogic, PlayerJumpLogic playerJumpLogic,
            PlayerPunchLogic playerPunchLogic, FuSkillLogic fuSkillLogic, PlayerReShapeLogic playerReShapeLogic,
            PlayerHealLogic playerHealLogic, PlayerStatusLogic playerStatusLogic, PlayerParticleLogic playerParticleLogic,
            PlayerFixSweetsLogic playerFixSweetsLogic, PlayerEnterDoorLogic playerEnterDoorLogic,
            PlayableCharacterSelectLogic playableCharacterSelectLogic, PlayerTalkLogic playerTalkLogic,
            PlayerGetKeyLogic playerGetKeyLogic, List<IDisposable> disposables, IObservable<bool> onChangedUseData)
            : base(playerNum, playerMoveLogic, playerJumpLogic, playerPunchLogic, fuSkillLogic, playerReShapeLogic,
                playerHealLogic, playerStatusLogic, playerParticleLogic, playerFixSweetsLogic, playerEnterDoorLogic,
                playableCharacterSelectLogic, playerTalkLogic,playerGetKeyLogic, disposables, onChangedUseData)
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