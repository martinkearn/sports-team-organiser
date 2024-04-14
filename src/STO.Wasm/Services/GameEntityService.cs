namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class GameEntityService(ICachedDataService dataService, IRatingEntityService ratingEntityService, IPlayerEntityService playerEntityService, ITransactionEntityService transactionEntityService) : IGameEntityService
	{
		private readonly ICachedDataService _dataService = dataService;
		private readonly IRatingEntityService _ratingEntityService = ratingEntityService;
		private readonly IPlayerEntityService _playerEntityService = playerEntityService;
		private readonly ITransactionEntityService _transactionEntityService = transactionEntityService;

		public async Task<List<PlayerAtGame>> CalculateTeamsAsync(List<PlayerAtGame> pags)
		{
            var newPags = new List<PlayerAtGame>();
            var rng = new Random();
            var yesPags = pags
                .Where(o => o.PlayerAtGameEntity.Forecast.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(o => o.Player.PlayerEntity.AdminRating).ToList();
            var nextTeamToGetPag = "A";

            foreach (var position in Enum.GetNames(typeof(Enums.PlayerPosition)))
            {
                // Get pags in this position
                var pagsInPosition = yesPags.Where(o => o.Player.PlayerEntity.Position.ToString() == position.ToString());

                // Distribute pags in this position between teams
                foreach (var pagInPosition in pagsInPosition)
                {
                    // Set team for page
                    pagInPosition.PlayerAtGameEntity.Team = nextTeamToGetPag;
                    newPags.Add(pagInPosition);

                    // Update pag in storage
                    await UpsertPlayerAtGameEntityAsync(pagInPosition.PlayerAtGameEntity);

                    // Set team for next pag
                    nextTeamToGetPag = (nextTeamToGetPag == "A") ? "B" : "A";
                }
            }

            _ = newPags.OrderBy(o => o.Player.PlayerEntity.Name);

            return newPags;

        }

		public async Task DeleteGameEntityAsync(string rowkey)
		{
			// Delete Ratings
			var ratingsForGame = _ratingEntityService.GetRatingEntitiesForGame(rowkey);
			foreach (var re in ratingsForGame)
			{
				await _ratingEntityService.DeleteRatingEntityAsync(re.RowKey);
			}

			// Delete PAGs
			var pagsForGame = GetPlayerAtGameEntitiesForGame(rowkey);
			foreach (var pag in pagsForGame)
			{
				await DeletePlayerAtGameEntityAsync(pag.RowKey);
			}

			// Delete game
			await _dataService.DeleteEntityAsync<GameEntity>(rowkey);
		}

		public async Task DeletePlayerAtGameEntityAsync(string rowKey)
		{
			await _dataService.DeleteEntityAsync<PlayerAtGameEntity>(rowKey);
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

		public Game GetGame(string rowKey)
		{
			// Get GameEntity
			var ge = GetGameEntity(rowKey);

			// Get PlayerAtGame's for GameEntity
			var playersAtGameEntities = GetPlayerAtGameEntitiesForGame(rowKey);

			// Calculate PlayerAtGame
			var playersAtGame = new List<PlayerAtGame>();
			foreach (var playersAtGameEntity in playersAtGameEntities)
			{
				var pag = new PlayerAtGame(playersAtGameEntity)
				{
					Player = _playerEntityService.GetPlayer(playersAtGameEntity.PlayerRowKey),
					GameEntity = ge
				};
				playersAtGame.Add(pag);
			}

			// Add teams to PlayerAtGame
			var teamA = playersAtGame
				.Where(pag => pag.PlayerAtGameEntity.Team == "A")
				.OrderBy(o => o.Player.PlayerEntity.Name)
				.ToList();
			var teamB = playersAtGame
				.Where(pag => pag.PlayerAtGameEntity.Team == "B")
				.OrderBy(o => o.Player.PlayerEntity.Name)
				.ToList();

			// Construct Game
			var game = new Game(ge)
			{
				PlayersAtGame = [.. playersAtGame.OrderBy(o => o.Player.PlayerEntity.Name)],
				TeamA = teamA,
				TeamB = teamB
			};

			return game;
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

		public async Task MarkAllPlayedAsync(string gameRowkey, bool played)
		{
			var pags = GetPlayerAtGameEntitiesForGame(gameRowkey);
			foreach (var pag in pags)
			{
				await TogglePlayerAtGamePlayedAsync(pag, played);
			}
		}

		public async Task TogglePlayerAtGamePlayedAsync(PlayerAtGameEntity pag, bool? played)
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
				await _transactionEntityService?.UpsertTransactionEntityAsync(transaction);
			}
			else
			{
				// Get debit transactions (less than £0) for player and game
				var teForPe = _transactionEntityService.GetTransactionEntities().Where(o => o.PlayerRowKey == playerEntity.RowKey);
				var pagDebitTransactionEntities = teForPe.Where(o => o.Amount < 0);
				foreach (var pagDebitTransactionEntity in pagDebitTransactionEntities)
				{
					await _transactionEntityService?.DeleteTransactionEntityAsync(pagDebitTransactionEntity.RowKey);
				}
			}

			// Upsert pag
			await UpsertPlayerAtGameEntityAsync(pag);
		}

		public async Task UpsertGameEntityAsync(GameEntity gameEntity)
		{
			await _dataService.UpsertEntityAsync(gameEntity);
		}

		public async Task UpsertPlayerAtGameEntityAsync(PlayerAtGameEntity pagEntity)
		{
			await _dataService.UpsertEntityAsync(pagEntity);
		}
	}
}
