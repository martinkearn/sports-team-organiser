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
			try
			{
				return pes.First(o => o.RowKey == rowKey);
			}
			catch (Exception ex)
			{
				var m = ex.Message;
				return new PlayerEntity();
			}
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

		public async Task DeletePlayerEntity(string playerRowkey)
		{
			// Delete Ratings
			var allRatingEntities = await _dataService.QueryEntities<RatingEntity>();
			var ratingsForPlayer = allRatingEntities.Where(o => o.PlayerRowKey == playerRowkey).ToList();
			foreach (var rating in ratingsForPlayer)
			{
				await _dataService.DeleteEntity<RatingEntity>(rating.RowKey);
			}

			// Delete TransactionEntity
			var transactionsResult = await _dataService.QueryEntities<TransactionEntity>();
			var transactions = transactionsResult.Where(t => t.PlayerRowKey == playerRowkey);
			foreach (var transaction in transactions)
			{
				await _dataService.DeleteEntity<TransactionEntity>(transaction.RowKey);
			}

			// Delete PlayerAtGameEntity
			var pagsResult = await _dataService.QueryEntities<PlayerAtGameEntity>();
			var pags = pagsResult.Where(pag => pag.PlayerRowKey == playerRowkey);
			foreach (var pag in pags)
			{
				await _dataService.DeleteEntity<PlayerAtGameEntity>(pag.RowKey);
			}

			// Delete PlayerEntity
			await _dataService.DeleteEntity<PlayerEntity>(playerRowkey);
		}

		public async Task UpsertPlayerEntity(PlayerEntity playerEntity)
		{
			await _dataService.UpsertEntity(playerEntity);
		}

	}
}