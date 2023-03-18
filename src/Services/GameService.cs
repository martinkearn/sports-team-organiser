namespace STO.Services
{
    /// <inheritdoc/>
    public class GameService : IGameService
    {
        private readonly IStorageService _storageService;
        public GameService(IStorageService storageService)
        {
            _storageService = storageService;
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