namespace STO.Models
{
    public class PlayerAtGame
    {
        public PlayerAtGame(PlayerAtGameEntity playerAtGameEntity)
        {
            this.PlayerAtGameEntity = playerAtGameEntity;
        } 

        public PlayerAtGameEntity PlayerAtGameEntity { get; set; } = default!;

        public Player Player { get; set; } = default!;
    }
}