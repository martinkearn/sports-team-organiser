namespace STO.Server.Models
{
    public class PlayerAtGame
    {
        public PlayerAtGame(PlayerAtGameEntity playerAtGameEntity)
        {
            this.PlayerAtGameEntity = playerAtGameEntity;
        } 

        public PlayerAtGameEntity PlayerAtGameEntity { get; set; } = default!;

        public Player Player { get; set; } = default!;

        public GameEntity GameEntity { get; set; } = default!;
    }
}