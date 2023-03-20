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

        public Task DeleteGame(string gameRowkey)
        {
            throw new NotImplementedException();
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

        public Game GetGame(string gameRowKey)
        {
            var gameEntities = _storageService.QueryEntities<GameEntity>($"RowKey eq '{gameRowKey}'");
            var matchingGame = GetGames(gameEntities).FirstOrDefault();
            return matchingGame;
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
                    Notes = $"Auto debited from game {pag.GameRowKey}"
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

        public List<PlayerAtGameEntity> CalculateTeams(List<PlayerAtGameEntity> pags)
        {
            var newPags = pags;
            var teams = new List<Team>();
            teams.Add(new Team() { Name = "A", Count = 0});
            teams.Add(new Team() { Name = "B", Count = 0});

            foreach (var position in Enum.GetNames(typeof(PlayerPosition)))
            {

            }

            return newPags;
        }    

        private List<Game> GameEntitiesToGames(List<GameEntity> gameEntities)
        {
            var games = new List<Game>();
            foreach (var ge in gameEntities)
            {
                var gamesTransactionEntities = _storageService.QueryEntities<TransactionEntity>($"GameRowKey eq '{ge.RowKey}'").OrderBy(t => t.Amount).ToList();
                var playersAtGameEntities = _storageService.QueryEntities<PlayerAtGameEntity>($"GameRowKey eq '{ge.RowKey}'");
                var playersAtGame = new List<PlayerAtGame>();
                foreach (var playersAtGameEntity in playersAtGameEntities)
                {
                    var pag = new PlayerAtGame(playersAtGameEntity);
                    pag.Player = _playerService.GetPlayer(playersAtGameEntity.PlayerRowKey);
                    playersAtGame.Add(pag);
                }
                var Game = new Game(ge)
                {
                    Transactions = gamesTransactionEntities,
                    PlayersAtGame = playersAtGame,
                };
                games.Add(Game);
            }
            return games;
        }
    }
}