namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class GameEntityService(ICachedDataService dataService, IRatingEntityService ratingEntityService, IPlayerEntityService playerEntityService, ITransactionEntityService transactionEntityService) : IGameEntityService
	{
		public async Task<List<PlayerAtGame>> CalculateTeamsAsync(List<PlayerAtGame> pags)
		{
            var newPags = new List<PlayerAtGame>();
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
			var ratingsForGame = ratingEntityService.GetRatingEntitiesForGame(rowkey);
			foreach (var re in ratingsForGame)
			{
				await ratingEntityService.DeleteRatingEntityAsync(re.RowKey);
			}

			// Delete PAGs
			var pagsForGame = GetPlayerAtGameEntitiesForGame(rowkey);
			foreach (var pag in pagsForGame)
			{
				await DeletePlayerAtGameEntityAsync(pag.RowKey);
			}

			// Delete game
			await dataService.DeleteEntityAsync<GameEntity>(rowkey);
		}

		public async Task DeletePlayerAtGameEntityAsync(string rowKey)
		{
			await dataService.DeleteEntityAsync<PlayerAtGameEntity>(rowKey);
		}

		public List<GameEntity> GetGameEntities()
		{
			return [.. dataService.GameEntities.OrderByDescending(o => o.Date)];
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
			var playersAtGame = playersAtGameEntities.Select(playersAtGameEntity => new PlayerAtGame(playersAtGameEntity)
			{
				Player = playerEntityService.GetPlayer(playersAtGameEntity.PlayerRowKey), 
				GameEntity = ge
			}).ToList();

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
			var gesOrderByDescending = ges.OrderByDescending(o => o.Date.DateTime);
			return gesOrderByDescending.First();
		}

		public string GetNotesForGame(string rowKey)
		{
			var ge = GetGameEntity(rowKey);
			if (ge is null) return string.Empty;
			var notes = $"For game {ge.Date.Date:dd MMM yyyy}";
			return notes;

		}

		public List<PlayerAtGameEntity> GetPlayerAtGameEntitiesForGame(string gameRowKey)
		{
			var pagsForGame = dataService.PlayerAtGameEntities.Where(o => o.GameRowKey == gameRowKey).ToList();
			return pagsForGame;
		}

		public PlayerAtGameEntity GetPlayerAtGameEntity(string rowKey)
		{
			return dataService.PlayerAtGameEntities.First(o => o.RowKey == rowKey);
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
			var playerEntity = playerEntityService.GetPlayerEntity(pag.PlayerRowKey);

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
				if (transactionEntityService is not null)
                {
                    await transactionEntityService.UpsertTransactionEntityAsync(transaction);
                }
			}
			else
			{
				// Get debit transactions (less than £0) for player and game
				var teForPe = transactionEntityService.GetTransactionEntities().Where(o => o.PlayerRowKey == playerEntity.RowKey);
				var pagDebitTransactionEntities = teForPe.Where(o => o.Amount < 0);
				if (transactionEntityService is not null)
                {
                    foreach (var pagDebitTransactionEntity in pagDebitTransactionEntities)
                    {
                        await transactionEntityService.DeleteTransactionEntityAsync(pagDebitTransactionEntity.RowKey);
                    }
                }
			}

			// Upsert pag
			await UpsertPlayerAtGameEntityAsync(pag);
		}

		public async Task UpsertGameEntityAsync(GameEntity gameEntity)
		{
			await dataService.UpsertEntityAsync(gameEntity);
		}

		public async Task UpsertPlayerAtGameEntityAsync(PlayerAtGameEntity pagEntity)
		{
			await dataService.UpsertEntityAsync(pagEntity);
		}
	}
}
