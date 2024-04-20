namespace STO.Models
{
    public class PlayerAtGame
    {
        public PlayerAtGame(PlayerAtGameEntity playerAtGameEntity)
        {
            this.PlayerAtGameEntity = playerAtGameEntity;
        } 

        public PlayerAtGameEntity PlayerAtGameEntity { get; set; } = default!;

        public PlayerEntity PlayerEntity { get; set; } = default!;

        public GameEntity GameEntity { get; set; } = default!;
    }
}