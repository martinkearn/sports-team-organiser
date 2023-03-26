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
            return matchingGame;
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
            await _storageService.UpsertEntity<PlayerAtGameEntity>(pag);
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

        public async Task<List<PlayerAtGame>> CalculateTeams(List<PlayerAtGame> pags)
        {
            var newPags = new List<PlayerAtGame>();
            var nextTeamToGetPag = "A";

            foreach (var position in Enum.GetNames(typeof(PlayerPosition)))
            {
                // Get pags in this position
                var pagsInPosition = pags.Where(o => o.Player.PlayerEntity.Position.ToString() == position.ToString());

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

        private Task<List<Game>> GameEntitiesToGames(List<GameEntity> gameEntities)
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
                    TransactionsEntities = gamesTransactionEntities,
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