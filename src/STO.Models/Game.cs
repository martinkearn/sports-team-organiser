namespace STO.Models
{
    public class Game
    {
        public Game(GameEntity gameEntity)
        {
            this.GameEntity = gameEntity;
        } 

        public GameEntity GameEntity { get; set; }

        public List<PlayerAtGameEntity> PlayersAtGame { get; set; }

        public List<PlayerAtGameEntity> TeamA { get; set; }

        public List<PlayerAtGameEntity> TeamB { get; set; }
    }
}