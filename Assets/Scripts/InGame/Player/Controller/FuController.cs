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
            PlayerGetKeyLogic playerGetKeyLogic, ActionKeyLogic actionKeyLogic, PlayerReviveLogic playerReviveLogic,
            List<IDisposable> disposables, bool initUsed,bool isInStage,IObservable<bool> onChangedInStageData,IObservable<bool> onChangedRevivingData)
            : base(playerNum, playerMoveLogic, playerJumpLogic, playerPunchLogic, fuSkillLogic, playerReShapeLogic,
                playerHealLogic, playerStatusLogic, playerParticleLogic, playerFixSweetsLogic, playerEnterDoorLogic,
                playableCharacterSelectLogic, playerTalkLogic, playerGetKeyLogic, actionKeyLogic, playerReviveLogic,
                disposables, initUsed,isInStage,onChangedInStageData,onChangedRevivingData)
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