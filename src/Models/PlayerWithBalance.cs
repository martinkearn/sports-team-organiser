namespace STO.Models
{
    public class PlayerWithBalance : PlayerEntity
    {
        public PlayerWithBalance(PlayerEntity player)
        {
            this.Player = player;
        }

        public PlayerEntity Player { get; set; }
        public double Balance { get; set; }
    }
}