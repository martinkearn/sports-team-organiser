namespace STO.Models
{
    public class Player
    {
        public Player(PlayerEntity playerEntity)
        {
            this.PlayerEntity = playerEntity;
        } 
        public PlayerEntity PlayerEntity { get; set; } = default!;
        public double Balance { get; set; } = default!;
        public List<TransactionEntity> Transactions { get; set; }
    }
}