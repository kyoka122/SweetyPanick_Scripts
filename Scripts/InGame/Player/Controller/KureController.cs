using System;
using System.Collections.Generic;
using InGame.Player.Logic;
using MyApplication;

namespace InGame.Player.Controller
{
    public class KureController:BasePlayerController
    {
        private readonly KureSkillLogic _kureSkillLogic;

        public KureController(int playerNum, PlayerMoveLogic playerMoveLogic, PlayerJumpLogic playerJumpLogic,
            PlayerPunchLogic playerPunchLogic, KureSkillLogic kureSkillLogic, PlayerReShapeLogic playerReShapeLogic,
            PlayerHealLogic playerHealLogic, PlayerStatusLogic playerStatusLogic, PlayerParticleLogic playerParticleLogic,
            PlayerFixSweetsLogic playerFixSweetsLogic, PlayerEnterDoorLogic playerEnterDoorLogic,
            PlayableCharacterSelectLogic playableCharacterSelectLogic, PlayerTalkLogic playerTalkLogic,
            List<IDisposable> disposables, IObservable<bool> onChangedUseDataUse)
            : base(playerNum, playerMoveLogic, playerJumpLogic, playerPunchLogic, kureSkillLogic, playerReShapeLogic,
                playerHealLogic, playerStatusLogic, playerParticleLogic, playerFixSweetsLogic, playerEnterDoorLogic,
                playableCharacterSelectLogic, playerTalkLogic, disposables, onChangedUseDataUse)
        {
            _kureSkillLogic = kureSkillLogic;
        }


        protected override void FixedUpdateEachPlayer()
        {
            //MEMO: 今の所特になし
        }

        public override void RegisterPlayerEvent(Action<FromPlayerEvent> playerEvent)
        {
            _kureSkillLogic.RegisterPlayerEvent(playerEvent);
        }
        
        protected override void TryConsumeHealPower()
        {
            _kureSkillLogic.ConsumeHealPower();
        }
    }
}