namespace STO.Wasm.Interfaces
{
	/// <summary>
	/// Service for working with GameEntity and PlayerAtGame.
	/// </summary>
	public interface IGameService
    {
        /// <summary>
        /// Gets all GameEntities.
        /// </summary>
        /// <returns>List of GameEntity.</returns>
        public List<GameEntity> GetGameEntities();

        /// <summary>
        /// Gets a single GameEntity.
        /// </summary>
        /// <param name="rowKey">The RowKey for the GameEntity to delete.</param>
        /// <returns>GameEntity.</returns>
        public GameEntity GetGameEntity(string rowKey);
        
        /// <summary>
        /// Gets the first GameEntity on a given date.
        /// </summary>
        /// <param name="date">The dd-mm-yyyy date string to match the game entity to.</param>
        /// <returns>GameEntity.</returns>
        public GameEntity GetGameEntityByGameDateLabel(string date);

        /// <summary>
        /// Gets the next GameEntity in terms of date.
        /// </summary>
        /// <returns>GameEntity.</returns>
        public GameEntity GetNextGameEntity();

        /// <summary>
        /// Deletes the GameEntity and PlayerAtGameEntity associated with a Game.
        /// </summary>
        /// <param name="rowKey">The RowKey for the GameEntity to delete.</param>
        public Task DeleteGameEntityAsync(string rowKey);

        /// <summary>
        /// Adds a new GameEntity.
        /// </summary>
        /// <param name="gameEntity">The GameEntity to upsert.</param>
        public Task UpsertGameEntityAsync(GameEntity gameEntity);

		/// <summary>
		/// Gets a list of PlayerAtGame for a given Game
		/// </summary>
		/// <param name="gameRowKey">The RowKey for the GameEntity to delete.</param>
		/// <returns>List of PlayerAtGame.</returns>
		public List<PlayerAtGameEntity> GetPlayerAtGameEntitiesForGame(string gameRowKey);

		/// <summary>
		/// Gets a single Pag (player at game).
		/// </summary>
		/// <param name="rowKey">The RowKey for the GameEntity to delete.</param>
		/// <returns>PlayerAtGame.</returns>
		public PlayerAtGameEntity GetPlayerAtGameEntity(string rowKey);

		/// <summary>
		/// Upserts a PlayerAtGameEntity.
		/// </summary>
		/// <param name="pagEntity">The PlayerAtGame.</param>     
		public Task UpsertPlayerAtGameEntityAsync(PlayerAtGameEntity pagEntity);

		/// <summary>
		/// Deletes a PlayerAtGameEntity from a game.
		/// </summary>
		/// <param name="rowKey">The RowKey for the PlayerAtGame to delete.</param>     
		public Task DeletePlayerAtGameEntityAsync(string rowKey);

		/// <summary>
		/// Distributes PlayerAtGames into teams
		/// </summary>
		/// <param name="pags">The PlayerAtGame collection to assign teams to.</param> 
		/// <returns>List of PlayerAtGame with team details.</returns>   
		public Task<List<PlayerAtGameEntity>> CalculateTeamsAsync(List<PlayerAtGameEntity> pags);

		/// <summary>
		/// Removes the team assignment for all PlayerAtGame in a Game
		/// </summary>
		/// <param name="gameRowKey">The RowKey for the GameEntity to remove team asisgnments for</param>
		/// <returns>Nothing</returns>
		public Task ResetTeamsAsync(string gameRowKey);

		/// <summary>
		/// Marks all PlayerAtGameEntity in a Game as having played.
		/// </summary>
		/// <param name="gameRowKey">The RowKey for the GameEntities to update PlayerAtGame for.</param>
		/// <param name="played">Indicates whether all PlayerAtGame should have played set to true or false</param>
		public Task MarkAllPlayedAsync(string gameRowKey, bool played);

		/// <summary>
		/// Toggles whether a player has played at a game or not and creates/removes trasnactions.
		/// </summary>
		/// <param name="pag">The PlayerAtGame to toggle the Played property for. Not used if null.</param>
		/// <param name="played">Indicates whether to mark the PlayerAtGameEntity as Played or not</param>  
		public Task TogglePlayerAtGamePlayedAsync(PlayerAtGameEntity pag, bool? played);

		/// <summary>
		/// A string which can be used as notes when mapping a transaction to a game
		/// </summary>
		/// <param name="rowKey">The RowKey of the GameEntity to create notes for.</param>
		/// <returns>A string which can be used as notes for a Game.</returns>
		public string GetNotesForGame(string rowKey);

		/// <summary>
		/// Returns a title for the GameEntity based on the game's date, title and note
		/// </summary>
		/// <param name="rowKey">The RowKey of the GameEntity to create a title for.</param>
		/// <returns>A string which represents the label for the GameEntity.</returns>
		public string GetGameLabel(string rowKey);

		/// <summary>
		/// Gets a DateTime object from a dd-mm-yyyy game date string (GameDateLabel)
		/// </summary>
		/// <param name="gameDateLabel">A DateTime in dd-mm-yyyy format 01-01-2024</param>
		/// <returns>A DateTime object</returns>
		public DateTime GetDateFromGameDateLabel(string gameDateLabel);

		/// <summary>
		/// Converts a DateTime into a game date dd-mm-yyyy string
		/// </summary>
		/// <param name="gameDate">A DateTime</param>
		/// <returns>DateTime as dd-mm-yyyy format</returns>
		public string GetGameDateLabelFromDate(DateTime gameDate);
    }
}