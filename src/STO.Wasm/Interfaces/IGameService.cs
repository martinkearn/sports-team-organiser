namespace STO.Wasm.Interfaces
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
        public Task<List<Game>> GetGames(List<GameEntity> gameEntities);

        /// <summary>
        /// Converts all GameEntities to a full list of Games.
        /// </summary>
        /// <returns>List of Games.</returns>
        public Task<List<Game>> GetGames();

        /// <summary>
        /// Gets a single Game.
        /// </summary>
        /// <returns>Game.</returns>
        public Task<Game> GetGame(string gameRowKey);

        /// <summary>
        /// Gets the next Game.
        /// </summary>
        /// <returns>Game.</returns>
        public Task<Game> GetNextGame();

        /// <summary>
        /// Deletes the GameEntity and PlayerAtGameEntity associated with a Game.
        /// </summary>
        /// <param name="gameRowkey">The RowKey for the GameEntities to delete.</param>
        public Task DeleteGame(string gameRowkey);

        /// <summary>
        /// Adds a new GameEntity.
        /// </summary>
        /// <param name="playerEntity">The GameEntity to upsert.</param>
        public Task UpsertGameEntity(GameEntity gameEntity);

        /// <summary>
        /// Gets a single Pag (player at game).
        /// </summary>
        /// <returns>PlayerAtGame.</returns>
        public Task<PlayerAtGame> GetPlayerAtGame(string pagRowKey);

        /// <summary>
        /// Upserts a player at a game, including transactions.
        /// </summary>
        /// <param name="pag">The PlayerAtGame.</param>     
        public Task UpsertPlayerAtGameEntity(PlayerAtGameEntity pag);

        /// <summary>
        /// Deletes a player from a game, including debit trasnactions.
        /// </summary>
        /// <param name="pag">The PlayerAtGame to delete.</param>     
        public Task DeletePlayerAtGameEntity(PlayerAtGameEntity pag);

        /// <summary>
        /// Distributes PAGs into teams
        /// </summary>
        /// <param name="pag">The PlayerAtGame collection to assign teams to.</param> 
        /// <returns>PlayerAtGame with team details.</returns>   
        public Task<List<PlayerAtGame>> CalculateTeams(List<PlayerAtGame> pags);

        /// <summary>
        /// Marks all players in a game as having played.
        /// </summary>
        /// <param name="gameRowkey">The RowKey for the GameEntities to update PlayerAtGame for.</param>
        /// <param name="played">Inidicates whether all PlayerAtGame should have played set to true or false</param>
        public Task MarkAllPlayed(string gameRowkey, bool played);

        /// <summary>
        /// Toggles whether a player has played at a game or not and creates/removes trasnactions.
        /// </summary>
        /// <param name="pag">The PlayerAtGame to toggle the Played property for. Not used if null.</param>  
        public Task TogglePlayerAtGamePlayed(PlayerAtGameEntity pag, bool? played);

        /// <summary>
        /// A string which can be used as notes when mapping a transaction to a game
        /// </summary>
        /// <param name="gameTitle"></param>
        /// <returns></returns>
        public Task<string> GetNotesForGame(string gameTitle);
    }
}