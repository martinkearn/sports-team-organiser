namespace STO.Interfaces
{
    /// <summary>
    /// Service for working with Games and their balance.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Converts a list of GameEntities to a full list of Games.
        /// </summary>
        /// <param name="gameEntities">The list of GameEntities to convert.</param>
        /// <returns>List of Games.</returns>
        public List<Game> GetGames(List<GameEntity> gameEntities);

        /// <summary>
        /// Converts all GameEntities to a full list of Games.
        /// </summary>
        /// <returns>List of Games.</returns>
        public List<Game> GetGames();

        /// <summary>
        /// Gets a single Game.
        /// </summary>
        /// <returns>Game.</returns>
        public Game GetGame(string gameRowKey);

        /// <summary>
        /// Deletes the GameEntity and PlayerAtGameEntity associated with a Game.
        /// </summary>
        /// <param name="gameRowkey">The RowKey for the GameEntities to delete.</param>
        public Task DeleteGame(string gameRowkey);

        /// <summary>
        /// Upserts a player at a game, including transactions.
        /// </summary>
        /// <param name="pag">The PlayerAtGame.</param>     
        public Task UpsertPlayerAtGame(PlayerAtGameEntity pag);

        /// <summary>
        /// Deletes a player from a game, including debit trasnactions.
        /// </summary>
        /// <param name="pag">The PlayerAtGame to delete.</param>     
        public Task DeletePlayerAtGame(PlayerAtGameEntity pag);

        /// <summary>
        /// Distributes PAGs into teams
        /// </summary>
        /// <param name="pag">The PlayerAtGame collection to assign teams to.</param> 
        /// <returns>PlayerAtGame with team details.</returns>   
        public List<PlayerAtGame> CalculateTeams(List<PlayerAtGame> pags);
    }
}