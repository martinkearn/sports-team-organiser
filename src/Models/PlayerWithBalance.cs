namespace STO.Models
{
    public class PlayerWithBalance : Player
    {
        public PlayerWithBalance(Player player)
        {
            this.Player = player;
        }

        public Player Player { get; set; }
        public double Balance { get; set; }
    }
}