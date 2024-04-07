namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class GameEntityService(ICachedDataService dataService, IRatingEntityService ratingEntityService, IPlayerEntityService playerEntityService, ITransactionEntityService transactionEntityService) : IGameEntityService
	{
		private readonly ICachedDataService _dataService = dataService;
		private readonly IRatingEntityService _ratingEntityService = ratingEntityService;
		private readonly IPlayerEntityService _playerEntityService = playerEntityService;
		private readonly ITransactionEntityService _transactionEntityService = transactionEntityService;

		public List<PlayerAtGameEntity> CalculateTeams(List<PlayerAtGameEntity> pags)
		{
			throw new NotImplementedException();
		}

		public void DeleteGameEntity(string rowkey)
		{
			// Delete Ratings
			var ratingsForGame = _ratingEntityService.GetRatingEntitiesForGame(rowkey);
			foreach (var re in ratingsForGame)
			{
				_ratingEntityService.DeleteRatingEntity(re.RowKey);
			}

			// Delete PAGs
			var pagsForGame = GetPlayerAtGameEntitiesForGame(rowkey);
			foreach (var pag in pagsForGame)
			{
				DeletePlayerAtGameEntity(pag.RowKey);
			}

			// Delete game
			_dataService.DeleteEntity<GameEntity>(rowkey);
		}

		public void DeletePlayerAtGameEntity(string rowKey)
		{
			_dataService.DeleteEntity<PlayerAtGameEntity>(rowKey);
		}

		public List<GameEntity> GetGameEntities()
		{
			return _dataService.GameEntities;
		}

		public GameEntity GetGameEntity(string rowKey)
		{
			var ges = GetGameEntities();
			return ges.First(o => o.RowKey == rowKey);
		}

		public GameEntity GetNextGameEntity()
		{
			var ges = GetGameEntities();
			ges.OrderByDescending(o => o.Date.DateTime);
			return ges.First();
		}

		public string GetNotesForGame(string rowKey)
		{
			var ge = GetGameEntity(rowKey);
			if (ge is not null)
			{
				var notes = $"For game {ge.Date.Date:dd MMM yyyy}";
				return notes;
			}

			return string.Empty;
		}

		public List<PlayerAtGameEntity> GetPlayerAtGameEntitiesForGame(string gameRowKey)
		{
			var pagsForGame = _dataService.PlayerAtGameEntities.Where(o => o.GameRowKey == gameRowKey).ToList();
			return pagsForGame;
		}

		public PlayerAtGameEntity GetPlayerAtGameEntity(string rowKey)
		{
			return _dataService.PlayerAtGameEntities.First(o => o.RowKey == rowKey);
		}

		public void MarkAllPlayed(string gameRowkey, bool played)
		{
			var pags = GetPlayerAtGameEntitiesForGame(gameRowkey);
			foreach (var pag in pags)
			{
				TogglePlayerAtGamePlayed(pag, played);
			}
		}

		public void TogglePlayerAtGamePlayed(PlayerAtGameEntity pag, bool? played)
		{
			// Get player for pag
			var playerEntity = _playerEntityService.GetPlayerEntity(pag.PlayerRowKey);

			if (played != null)
			{
				// Set the pag value to what played is
				pag.Played = (bool)played;
			}
			else
			{
				// Just toggle the pag value
				pag.Played = !pag.Played;
			}

			// Add / remove transactions if played / not played
			if (pag.Played)
			{
				var transaction = new TransactionEntity()
				{
					PlayerRowKey = pag.PlayerRowKey,
					Amount = -playerEntity.DefaultRate,
					Date = DateTimeOffset.UtcNow,
					Notes = GetNotesForGame(pag.GameRowKey)
				};
				_transactionEntityService?.UpsertTransactionEntity(transaction);
			}
			else
			{
				// Get debit transactions (less than £0) for player and game
				var teForPe = _transactionEntityService.GetTransactionEntities().Where(o => o.PlayerRowKey == playerEntity.RowKey);
				var pagDebitTransactionEntities = teForPe.Where(o => o.Amount < 0);
				foreach (var pagDebitTransactionEntity in pagDebitTransactionEntities)
				{
					_transactionEntityService?.DeleteTransactionEntity(pagDebitTransactionEntity.RowKey);
				}
			}

			// Upsert pag
			UpsertPlayerAtGameEntity(pag);
		}

		public void UpsertGameEntity(GameEntity gameEntity)
		{
			_dataService.UpsertEntity(gameEntity);
		}

		public void UpsertPlayerAtGameEntity(PlayerAtGameEntity pagEntity)
		{
			_dataService.UpsertEntity(pagEntity);
		}
	}
}
