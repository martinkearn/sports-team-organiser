namespace STO.Wasm.Services
{
    /// <inheritdoc/>
    public class GameService(IApiService storageService, IPlayerService playerService, ITransactionService transactionService) : IGameService
    {
        private readonly IApiService _storageService = storageService;
        private readonly IPlayerService _playerService = playerService;

        private readonly ITransactionService _transactionService = transactionService;

        public async Task DeleteGame(string gameRowKey)
        {
            var game = await GetGame(gameRowKey);

            // Delete Ratings - Cannot use RatingService due to circular dependecy so must use StorageService directly
            var allRatingEntities = await _storageService.QueryEntities<RatingEntity>();
            var ratingsForGame = allRatingEntities.Where(o => o.GameRowKey == gameRowKey).ToList();
            foreach (var rating in ratingsForGame)
            {
                await _storageService.DeleteEntity<RatingEntity>(rating.RowKey);
            }

            // Delete PAGs
            foreach (var pag in game.PlayersAtGame)
            {
                await DeletePlayerAtGameEntity(pag.PlayerAtGameEntity);
            }

            // Delete game
            await _storageService.DeleteEntity<GameEntity>(gameRowKey);
        }

        public async Task<List<Game>> GetGames(List<GameEntity> gameEntities)
        {
            return await GameEntitiesToGames(gameEntities);
        }

        public async Task<List<Game>> GetGames()
        {
            var gameEntitiesResult = await _storageService.QueryEntities<GameEntity>();
            var gameEntities = gameEntitiesResult.OrderByDescending(g => g.Date).ToList();
            return await GameEntitiesToGames(gameEntities);
        }

        public async Task<Game> GetGame(string gameRowKey)
        {
            var gameEntitiesResult = await _storageService.QueryEntities<GameEntity>();
            var gameEntities = gameEntitiesResult.Where(g => g.RowKey == gameRowKey).ToList();
            var games = await GetGames(gameEntities);
            var matchingGame = games.FirstOrDefault();
            if (matchingGame is not null)
            {
                matchingGame.TeamA = [.. matchingGame.TeamA.OrderBy(o => o.Player.PlayerEntity.Position)];
                matchingGame.TeamB = [.. matchingGame.TeamB.OrderBy(o => o.Player.PlayerEntity.Position)];
                return matchingGame;
            }

            // Create a null game to return
            Game nullGame = new(new GameEntity());
            return nullGame;
        }

        public async Task<Game> GetNextGame()
        {
            var gameEntitiesResult = await _storageService.QueryEntities<GameEntity>();
            var gameEntities = gameEntitiesResult.Where(g => g.Date.Date > DateTime.UtcNow.Date).ToList();
            var games = await GetGames(gameEntities);
            var nextGame = games.FirstOrDefault();
            if (nextGame is not null)
            {
                return nextGame;
            }

            // Create a null game to return
            Game game = new(new GameEntity());
            return game;
        }

        public async Task UpsertGameEntity(GameEntity gameEntity)
        {
            _ = await _storageService.UpsertEntity<GameEntity>(gameEntity);
        }

        public async Task<PlayerAtGame> GetPlayerAtGame(string pagRowKey)
        {
            var pagEntityResult = await _storageService.QueryEntities<PlayerAtGameEntity>();
            var pagEntity = pagEntityResult.FirstOrDefault(o => o.RowKey == pagRowKey);
            if (pagEntity is not null)
            {
                var game = await GetGame(pagEntity.GameRowKey);
                var pag = new PlayerAtGame(pagEntity)
                {
                    Player = await _playerService.GetPlayer(pagEntity.PlayerRowKey),
                    GameEntity = game.GameEntity
                };
                return pag;
            }

            // Create a null pag to return
            PlayerAtGame emptyPag = new(new PlayerAtGameEntity());
            return emptyPag;
        }

        public async Task UpsertPlayerAtGameEntity(PlayerAtGameEntity pag)
        {
            var game = await GetGame(pag.GameRowKey);
            var existingPag = game.PlayersAtGame.FirstOrDefault(p => p.Player.PlayerEntity.RowKey == pag.PlayerRowKey);
            if (existingPag == default)
            {
                _ = await _storageService.UpsertEntity<PlayerAtGameEntity>(pag);
            }
            else
            {
                pag.RowKey = existingPag.PlayerAtGameEntity.RowKey;
                pag.PartitionKey = existingPag.PlayerAtGameEntity.PartitionKey;
                _ = await _storageService.UpsertEntity<PlayerAtGameEntity>(pag);
            }
        }

        public async Task DeletePlayerAtGameEntity(PlayerAtGameEntity pag)
        {
            await _storageService.DeleteEntity<PlayerAtGameEntity>(pag.RowKey);
        }

        public async Task<List<PlayerAtGame>> CalculateTeams(List<PlayerAtGame> pags)
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
                    await UpsertPlayerAtGameEntity(pagInPosition.PlayerAtGameEntity);

                    // Set team for next pag
                    nextTeamToGetPag = (nextTeamToGetPag == "A") ? "B" : "A";
                }
            }

            _ = newPags.OrderBy(o => o.Player.PlayerEntity.Name);

            return newPags;
        }

        public async Task MarkAllPlayed(string gameRowkey, bool played)
        {
            var game = await GetGame(gameRowkey);

            var togglePlayedTasks = new List<Task>();
            foreach (var pag in game.PlayersAtGame)
            {
                togglePlayedTasks.Add(TogglePlayerAtGamePlayed(pag.PlayerAtGameEntity, played));
            }
            await Task.WhenAll(togglePlayedTasks);
        }

        public async Task TogglePlayerAtGamePlayed(PlayerAtGameEntity pag, bool? played)
        {
            // Get player for pag
            var player = await _playerService.GetPlayer(pag.PlayerRowKey);

            if (played != null)
            {
                // Set the pag value to what played is
                pag.Played = (bool)played;
            }
            else
            {
                // Just toggle te pag value
                pag.Played = !pag.Played;
            }

            // Add / remove transactions if played / not played
            if (pag.Played)
            {
                var transaction = new TransactionEntity()
                {
                    PlayerRowKey = pag.PlayerRowKey,
                    Amount = -player.PlayerEntity.DefaultRate,
                    Date = DateTimeOffset.UtcNow,
                    Notes = await _transactionService.GetNotesForGame(pag.GameRowKey)
                };
                await _transactionService.UpsertTransactionEntity(transaction);
            }
            else
            {
                // Get debit transactions (less than £0) for player and game
                var pagDebitTransactionEntities = player.Transactions
                    .Where(o => o.Amount < 0);
                foreach (var pagDebitTransactionEntity in pagDebitTransactionEntities)
                {
                    await _transactionService.DeleteTransactionEntity(pagDebitTransactionEntity.RowKey);
                }
            }

            // Upsert pag
            await UpsertPlayerAtGameEntity(pag);
        }

        private async Task<List<Game>> GameEntitiesToGames(List<GameEntity> gameEntities)
        {
            var games = new List<Game>();
            foreach (var ge in gameEntities)
            {
                // Get entities from storage
                var playersAtGameEntitiesResult = await _storageService.QueryEntities<PlayerAtGameEntity>();
                var playersAtGameEntities = playersAtGameEntitiesResult.Where(pag => pag.GameRowKey == ge.RowKey);

                // Calculate PlayerAtGame
                var playersAtGame = new List<PlayerAtGame>();
                foreach (var playersAtGameEntity in playersAtGameEntities)
                {
                    var pag = new PlayerAtGame(playersAtGameEntity)
                    {
                        Player = await _playerService.GetPlayer(playersAtGameEntity.PlayerRowKey),
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
                var Game = new Game(ge)
                {
                    PlayersAtGame = playersAtGame.OrderBy(o => o.Player.PlayerEntity.Name).ToList(),
                    TeamA = teamA,
                    TeamB = teamB
                };
                games.Add(Game);
            }
            return games;
        }

    }
}