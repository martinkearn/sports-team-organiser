namespace STO.Wasm.Interfaces
{
	/// <summary>
	/// Service for working with GameEntity and PlayerAtGame.
	/// </summary>
	public interface IGameEntityService
    {
        /// <summary>
        /// Gets all GameEntities.
        /// </summary>
        /// <returns>List of GameEntity.</returns>
        public List<GameEntity> GetGameEntities();

        /// <summary>
        /// Gets a single GameEntity.
        /// </summary>
        /// <returns>GameEntity.</returns>
        public GameEntity GetGameEntity(string rowKey);

		/// <summary>
		/// Gets a single Game.
		/// </summary>
		/// <param name="rowKey">The RowKey of the GameEntity which the Game is based on.</param>
		/// <returns>A Game.</returns>
		public Game GetGame(string rowKey);

        /// <summary>
        /// Gets the next GameEntity in terms of date.
        /// </summary>
        /// <returns>GameEntity.</returns>
        public GameEntity GetNextGameEntity();

        /// <summary>
        /// Deletes the GameEntity and PlayerAtGameEntity associated with a Game.
        /// </summary>
        /// <param name="gameRowkey">The RowKey for the GameEntity to delete.</param>
        public void DeleteGameEntity(string rowkey);

        /// <summary>
        /// Adds a new GameEntity.
        /// </summary>
        /// <param name="playerEntity">The GameEntity to upsert.</param>
        public void UpsertGameEntity(GameEntity gameEntity);

		/// <summary>
		/// Gets a list of PlayerAtGame for a given Game
		/// </summary>
		/// <returns>List of PlayerAtGame.</returns>
		public List<PlayerAtGameEntity> GetPlayerAtGameEntitiesForGame(string gameRowKey);

		/// <summary>
		/// Gets a single Pag (player at game).
		/// </summary>
		/// <returns>PlayerAtGame.</returns>
		public PlayerAtGameEntity GetPlayerAtGameEntity(string rowKey);

		/// <summary>
		/// Upserts a PlayerAtGameEntity.
		/// </summary>
		/// <param name="pagEntity">The PlayerAtGame.</param>     
		public void UpsertPlayerAtGameEntity(PlayerAtGameEntity pagEntity);

		/// <summary>
		/// Deletes a PlayerAtGameEntity from a game.
		/// </summary>
		/// <param name="rowKey">The RowKey for the PlayerAtGame to delete.</param>     
		public void DeletePlayerAtGameEntity(string rowKey);

		/// <summary>
		/// Distributes PlayerAtGames into teams
		/// </summary>
		/// <param name="pag">The PlayerAtGame collection to assign teams to.</param> 
		/// <returns>List of PlayerAtGame with team details.</returns>   
		public List<PlayerAtGame> CalculateTeams(List<PlayerAtGame> pags);

		/// <summary>
		/// Marks all PlayerAtGameEntity in a Game as having played.
		/// </summary>
		/// <param name="gameRowkey">The RowKey for the GameEntities to update PlayerAtGame for.</param>
		/// <param name="played">Inidicates whether all PlayerAtGame should have played set to true or false</param>
		public void MarkAllPlayed(string gameRowkey, bool played);

        /// <summary>
        /// Toggles whether a player has played at a game or not and creates/removes trasnactions.
        /// </summary>
        /// <param name="pag">The PlayerAtGame to toggle the Played property for. Not used if null.</param>  
        public void TogglePlayerAtGamePlayed(PlayerAtGameEntity pag, bool? played);

		/// <summary>
		/// A string which can be used as notes when mapping a transaction to a game
		/// </summary>
		/// <param name="rowKey">The RowKey of teh GameEntity to create notes for.</param>
		/// <returns>A string which can be used as notes for a Game.</returns>
		public string GetNotesForGame(string rowKey);
    }
}