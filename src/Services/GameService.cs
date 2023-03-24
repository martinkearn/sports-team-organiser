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
            var game = GetGame(gameRowkey);

            // Delete PAGs
            foreach(var pag in game.PlayersAtGame)
            {
                await DeletePlayerAtGameEntity(pag.PlayerAtGameEntity);
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
            var gameEntities = _storageService.QueryEntities<GameEntity>()
                .OrderByDescending(g => g.Date)
                .ToList();
            return GameEntitiesToGames(gameEntities);
        }

        public Game GetGame(string gameRowKey)
        {
            var gameEntities = _storageService.QueryEntities<GameEntity>().Where(g => g.RowKey == gameRowKey).ToList();
            var matchingGame = GetGames(gameEntities).FirstOrDefault();
            return matchingGame;
        }

        public async Task UpsertGameEntity(GameEntity gameEntity)
        {
            await _storageService.UpsertEntity<GameEntity>(gameEntity);
        }

        public PlayerAtGame GetPlayerAtGame(string pagRowKey)
        {
            var pagEntity = _storageService.QueryEntities<PlayerAtGameEntity>().FirstOrDefault(o => o.RowKey == pagRowKey);
            var pag = new PlayerAtGame(pagEntity);
            pag.Player = _playerService.GetPlayer(pagEntity.PlayerRowKey);
            pag.GameEntity = GetGame(pagEntity.GameRowKey).GameEntity;
            return pag;
        }

        public async Task UpsertPlayerAtGameEntity(PlayerAtGameEntity pag)
        {
            // Update PAG itself
            await _storageService.UpsertEntity<PlayerAtGameEntity>(pag);

            // Get game
            var game = GetGame(pag.GameRowKey);
        
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
                    Notes = $"Debited for game {game.GameEntity.Date.Date.ToShortDateString()}"
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

        public async Task DeletePlayerAtGameEntity(PlayerAtGameEntity pag)
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

        public List<PlayerAtGame> CalculateTeams(List<PlayerAtGame> pags)
        {
            var newPags = new List<PlayerAtGame>();
            var nextTeamToGetPag = "A";

            foreach (var position in Enum.GetNames(typeof(PlayerPosition)))
            {
                var pagsInPosition = pags.Where(p => p.Player.PlayerEntity.Position.ToString() == position.ToString());
                foreach (var pagInPosition in pagsInPosition)
                {
                    pagInPosition.PlayerAtGameEntity.Team = nextTeamToGetPag;
                    newPags.Add(pagInPosition);
                    nextTeamToGetPag = (nextTeamToGetPag == "A") ? "B": "A";
                }
            }

            newPags.OrderBy(o => o.Player.PlayerEntity.Name);

            return newPags;
        }    

        private List<Game> GameEntitiesToGames(List<GameEntity> gameEntities)
        {
            var games = new List<Game>();
            foreach (var ge in gameEntities)
            {
                // Get entities from storage
                var gamesTransactionEntities = _storageService.QueryEntities<TransactionEntity>()
                    .Where(t => t.GameRowKey == ge.RowKey)
                    .OrderByDescending(o => o.Date)
                    .ToList();
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
                var playersAtGameWithTeams = CalculateTeams(playersAtGame);
                var teamA = playersAtGameWithTeams.Where(pag => pag.PlayerAtGameEntity.Team == "A").ToList();
                var teamB = playersAtGameWithTeams.Where(pag => pag.PlayerAtGameEntity.Team == "B").ToList();

                // Construct Game
                var Game = new Game(ge)
                {
                    TransactionsEntities = gamesTransactionEntities,
                    PlayersAtGame = playersAtGameWithTeams,
                    TeamA = teamA,
                    TeamB = teamB
                };
                games.Add(Game);
            }
            return games;
        }
    }
}