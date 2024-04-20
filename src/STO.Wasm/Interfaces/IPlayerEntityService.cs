namespace STO.Wasm.Interfaces
{
	public class InClassName
	{
		public InClassName(string rowKey)
		{
			RowKey = rowKey;
		}

		public string RowKey { get; private set; }
	}

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
		public PlayerEntity GetPlayerEntity(string rowKey);

		/// <summary>
		/// Returns the default rate for the PlayerEntity associated with the rowKey
		/// </summary>
		/// <param name="rowKey">The Rowkey for the PlayerEntity to get the default rate for</param>
		/// <returns>A double representing the default rate.</returns>
		public double GetDefaultRateForPlayerEntity(string rowKey);
		
		/// <summary>
		/// Returns the balance of all TransactionEntities for the PlayerEntity associated with the rowKey
		/// </summary>
		/// <param name="rowKey">The Rowkey for the PlayerEntity to get the balance for</param>
		/// <returns>A double representing the balance.</returns>
		public double GetBalanceForPlayerEntity(string rowKey);

		/// <summary>
		/// Returns the Rating for the PlayerEntity associated with the rowKey
		/// </summary>
		/// <param name="rowKey">The Rowkey for the PlayerEntity to get the rating for</param>
		/// <returns>A double representing the player rating.</returns>
		public double GetRatingForPlayerEntity(string rowKey);

		/// <summary>
		/// Deletes the PlayerEntity, TransactionEntity and PlayerAtGameEntity associated with a Player.
		/// </summary>
		/// <param name="playerRowkey">The RowKey for the PlayerEntities to delete.</param>
		public Task DeletePlayerEntityAsync(string playerRowkey);

		/// <summary>
		/// Adds a new PlayerEntity.
		/// </summary>
		/// <param name="playerEntity">The PlayerEntity to upsert.</param>
		public Task UpsertPlayerEntityAsync(PlayerEntity playerEntity);
	}
}