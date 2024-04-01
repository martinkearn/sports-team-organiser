namespace STO.Wasm.Interfaces
{
	/// <summary>
	/// Service for working with Players, PlayerEntities.
	/// </summary>
	public interface IPlayerEntityService
	{
		/// <summary>
		/// Gets all PlayerEntity objects.
		/// </summary>
		/// <returns>List of Players.</returns>
		public List<PlayerEntity> GetPlayerEntities();

		/// <summary>
		/// Gets a PlayerEntity object
		/// </summary>
		/// <returns>A PlayerEntity.</returns>
		public Player GetPlayerEntity(string rowKey);

		/// <summary>
		/// Gets a Player object
		/// </summary>
		/// <returns>A Player.</returns>
		public Player GetPlayer(string rowKey);

		/// <summary>
		/// Deletes the PlayerEntity, TransactionEntity and PlayerAtGameEntity associated with a Player.
		/// </summary>
		/// <param name="playerRowkey">The RowKey for the PlayerEntities to delete.</param>
		public void DeletePlayerEntity(string playerRowkey);

		/// <summary>
		/// Adds a new PlayerEntity.
		/// </summary>
		/// <param name="playerEntity">The PlayerEntity to upsert.</param>
		public void UpsertPlayerEntity(PlayerEntity playerEntity);
	}
}