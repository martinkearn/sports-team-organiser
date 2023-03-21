namespace STO.Models
{
    public class Game
    {
        public Game(GameEntity gameEntity)
        {
            this.GameEntity = gameEntity;
        } 

        public GameEntity GameEntity { get; set; }

        public List<TransactionEntity> TransactionsEntities { get; set; }

        public List<PlayerAtGame> PlayersAtGame { get; set; }

        public List<PlayerAtGame> TeamA { get; set; }

        public List<PlayerAtGame> TeamB { get; set; }
    }
}