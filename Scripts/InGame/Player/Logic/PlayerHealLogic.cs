using InGame.Player.Entity;

namespace InGame.Player.Logic
{
    public class PlayerHealLogic
    {
        private readonly PlayerConstEntity _playerConstEntity;
        private readonly PlayerCommonUpdateableEntity _playerCommonUpdateableEntity;

        public PlayerHealLogic(PlayerConstEntity playerConstEntity,PlayerCommonUpdateableEntity playerCommonUpdateableEntity)
        {
            _playerConstEntity = playerConstEntity;
            _playerCommonUpdateableEntity = playerCommonUpdateableEntity;
        }

        public void HealHp()
        {
            _playerCommonUpdateableEntity.HealHp(_playerConstEntity.HealValue);
        }
    }
}