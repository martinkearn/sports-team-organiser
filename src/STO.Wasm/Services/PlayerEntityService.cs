using STO.Models;

namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class PlayerEntityService(ICachedDataService dataService) : IPlayerEntityService
	{
		private readonly ICachedDataService _dataService = dataService;

		public List<PlayerEntity> GetPlayerEntities()
		{
			return _dataService.PlayerEntities;
		}

		public PlayerEntity GetPlayerEntity(string rowKey)
		{
			var pes = _dataService.PlayerEntities;
			return pes.First(o => o.RowKey == rowKey);
		}

		public Player GetPlayer(string rowKey)
		{
			// Get PlayerEntity
			var playerEntity = GetPlayerEntity(rowKey);

			// Get Transactions
			var playersTransactions = _dataService.TransactionEntities.Where(o => o.PlayerRowKey == rowKey)
				.OrderByDescending(o => o.Date)
				.ToList();

			// Get Balance
			var playerBalance = playersTransactions.Sum(o => o.Amount);

			// Construct Player
			var player = new Player(playerEntity)
			{
				Transactions = playersTransactions,
				Balance = playerBalance
			};

			return player;
		}

		public void DeletePlayerEntity(string playerRowkey)
		{
			throw new NotImplementedException();
		}

		public void UpsertPlayerEntity(PlayerEntity playerEntity)
		{
			throw new NotImplementedException();
		}

	}
}