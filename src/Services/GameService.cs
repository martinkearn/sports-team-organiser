namespace STO.Services
{
    /// <inheritdoc/>
    public class GameService : IGameService
    {
        private readonly IStorageService _storageService;
        private readonly IPlayerService _playerService;
        public GameService(IStorageService storageService, IPlayerService playerService)
        {
            _storageService = storageService;
            _playerService = playerService;
        }

        public async Task DeleteGame(string gameRowkey)
        {
            // Get and delete PAGs
            var pags = _storageService.QueryEntities<PlayerAtGameEntity>($"GameRowKey eq '{gameRowkey}'");
            foreach (var pag in pags)
            {
                await _storageService.DeleteEntity<PlayerAtGameEntity>(pag.RowKey);
            }

            // Delete game
            await _storageService.DeleteEntity<GameEntity>(gameRowkey);
        }

        public List<Game> GetGames(List<GameEntity> gameEntities)
        {
            return GameEntitiesToGames(gameEntities);
        }

        public List<Game> GetGames()
        {
            var gameEntities = _storageService.QueryEntities<GameEntity>(default)
                .OrderByDescending(g => g.Date)
                .ToList();
            return GameEntitiesToGames(gameEntities);
        }

        public async Task UpsertPlayerAtGame(PlayerAtGameEntity pag)
        {
            // Update PAG itself
            await _storageService.UpsertEntity<PlayerAtGameEntity>(pag);
        
            // Add / remove transactions
            var player = _playerService.GetPlayer(pag.PlayerRowKey);
            if (pag.Played)
            {
                var transaction = new TransactionEntity()
                {
                    PlayerRowKey = pag.PlayerRowKey,
                    Amount = -player.PlayerEntity.DefaultRate,
                    Date = DateTimeOffset.UtcNow,
                    GameRowKey = pag.GameRowKey,
                    Notes = "Auto debited"
                };
                await _storageService.UpsertEntity<TransactionEntity>(transaction);
            }
            else
            {
                // Get debit transactions (less than £0) for player and game
                var playerGameDebits = player.Transactions.Where(t => t.GameRowKey == pag.GameRowKey).Where(t => t.Amount < 0);
                foreach (var playerGameDebit in playerGameDebits)
                {
                    await _storageService.DeleteEntity<TransactionEntity>(playerGameDebit.RowKey);
                }
            }
        }

        public async Task DeletePlayerAtGame(PlayerAtGameEntity pag)
        {
            // Delete transactions for PAG less than £0 (debits)
            var player = _playerService.GetPlayer(pag.PlayerRowKey);
            var playerGameDebits = player.Transactions.Where(t => t.GameRowKey == pag.GameRowKey).Where(t => t.Amount < 0);
            foreach (var playerGameDebit in playerGameDebits)
            {
                await _storageService.DeleteEntity<TransactionEntity>(playerGameDebit.RowKey);
            }

            // Delete PAG itself
            await _storageService.DeleteEntity<PlayerAtGameEntity>(pag.RowKey);
        }       

        private List<Game> GameEntitiesToGames(List<GameEntity> gameEntities)
        {
            var players = _storageService.QueryEntities<PlayerEntity>(default);
            var games = new List<Game>();
            foreach (var ge in gameEntities)
            {
                var gamesTransactions = _storageService.QueryEntities<TransactionEntity>($"GameRowKey eq '{ge.RowKey}'");
                var playersAtGame = _storageService.QueryEntities<PlayerAtGameEntity>($"GameRowKey eq '{ge.RowKey}'");
                var Game = new Game(ge)
                {
                    Transactions = gamesTransactions,
                    PlayersAtGame = playersAtGame,
                };
                games.Add(Game);
            }
            return games;
        }
    }
}