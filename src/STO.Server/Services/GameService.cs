using Azure;

namespace STO.Server.Services
{
    /// <inheritdoc/>
    public class GameService : IGameService
    {
        private readonly IStorageService _storageService;
        private readonly IPlayerService _playerService;

        private readonly ITransactionService _transactionService;

        public GameService(IStorageService storageService, IPlayerService playerService, ITransactionService transactionService)
        {
            _storageService = storageService;
            _playerService = playerService;
            _transactionService = transactionService;
        }

        public async Task DeleteGame(string gameRowkey)
        {
            var game = await GetGame(gameRowkey);

            // Delete PAGs
            foreach(var pag in game.PlayersAtGame)
            {
                await DeletePlayerAtGameEntity(pag.PlayerAtGameEntity);
            }

            // Delete game
            await _storageService.DeleteEntity<GameEntity>(gameRowkey);
        }

        public async Task<List<Game>> GetGames(List<GameEntity> gameEntities)
        {
            return await GameEntitiesToGames(gameEntities);
        }

        public async Task<List<Game>> GetGames()
        {
            var gameEntities = _storageService.QueryEntities<GameEntity>()
                .OrderByDescending(g => g.Date)
                .ToList();
            return await GameEntitiesToGames(gameEntities);
        }

        public async Task<Game> GetGame(string gameRowKey)
        {
            var gameEntities = _storageService.QueryEntities<GameEntity>().Where(g => g.RowKey == gameRowKey).ToList();
            var games = await GetGames(gameEntities);
            var matchingGame = games.FirstOrDefault();
            matchingGame.TeamA = matchingGame.TeamA.OrderBy(o => o.Player.PlayerEntity.Position).ToList();
            matchingGame.TeamB = matchingGame.TeamB.OrderBy(o => o.Player.PlayerEntity.Position).ToList();
            return matchingGame;
        }

        public async Task<Game> GetNextGame()
        {
            var gameEntities = _storageService.QueryEntities<GameEntity>().Where(g => g.Date.Date > DateTime.UtcNow.Date).ToList();
            var games = await GetGames(gameEntities);
            var nextGame = games.FirstOrDefault();
            return nextGame;
        }

        public async Task UpsertGameEntity(GameEntity gameEntity)
        {
            await _storageService.UpsertEntity<GameEntity>(gameEntity);
        }

        public async Task<PlayerAtGame> GetPlayerAtGame(string pagRowKey)
        {
            var pagEntity = _storageService.QueryEntities<PlayerAtGameEntity>().FirstOrDefault(o => o.RowKey == pagRowKey);
            var game = await GetGame(pagEntity.GameRowKey);
            var pag = new PlayerAtGame(pagEntity);
            pag.Player = _playerService.GetPlayer(pagEntity.PlayerRowKey);
            pag.GameEntity = game.GameEntity;
            return pag;
        }

        public async Task UpsertPlayerAtGameEntity(PlayerAtGameEntity pag)
        {
            var game = await GetGame(pag.GameRowKey);
            var existingPag = game.PlayersAtGame.FirstOrDefault(p => p.Player.PlayerEntity.RowKey == pag.PlayerRowKey);
            if (existingPag == default)
            {
                await _storageService.UpsertEntity<PlayerAtGameEntity>(pag);
            }
            else
            {
                pag.RowKey = existingPag.PlayerAtGameEntity.RowKey;
                pag.PartitionKey = existingPag.PlayerAtGameEntity.PartitionKey;
                await _storageService.UpsertEntity<PlayerAtGameEntity>(pag);
            } 
        }

        public async Task DeletePlayerAtGameEntity(PlayerAtGameEntity pag)
        {
            // Delete transactions for PAG for game
            var player = _playerService.GetPlayer(pag.PlayerRowKey);
            var transactionNotes = _transactionService.GetNotesForGame(pag.GameRowKey);
            var playerGameDebits = player.Transactions.Where(t => t.Notes == transactionNotes);
            foreach (var playerGameDebit in playerGameDebits)
            {
                await _storageService.DeleteEntity<TransactionEntity>(playerGameDebit.RowKey);
            }

            // Delete PAG itself
            await _storageService.DeleteEntity<PlayerAtGameEntity>(pag.RowKey);
        }   

        public async Task<List<PlayerAtGame>> CalculateTeams(List<PlayerAtGame> pags)
        {
            var newPags = new List<PlayerAtGame>();
            var rng = new Random();
            var yesPags = pags
                .Where(o => o.PlayerAtGameEntity.Forecast.ToLowerInvariant() == "yes")
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
                    nextTeamToGetPag = (nextTeamToGetPag == "A") ? "B": "A";
                }
            }

            newPags.OrderBy(o => o.Player.PlayerEntity.Name);

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
            var player = _playerService.GetPlayer(pag.PlayerRowKey);

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
                    Notes = _transactionService.GetNotesForGame(pag.GameRowKey)
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

        private Task<List<Game>> GameEntitiesToGames(List<GameEntity> gameEntities)
        {
            var games = new List<Game>();
            foreach (var ge in gameEntities)
            {
                // Get entities from storage
                var playersAtGameEntities = _storageService.QueryEntities<PlayerAtGameEntity>()
                    .Where(pag => pag.GameRowKey == ge.RowKey);

                // Calculate PlayerAtGame
                var playersAtGame = new List<PlayerAtGame>();
                foreach (var playersAtGameEntity in playersAtGameEntities)
                {
                    var pag = new PlayerAtGame(playersAtGameEntity);
                    pag.Player = _playerService.GetPlayer(playersAtGameEntity.PlayerRowKey);
                    pag.GameEntity = ge;
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
            return Task.FromResult(games);
        }

    }
}