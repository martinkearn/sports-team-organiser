namespace STO.Wasm.Services
{
    /// <inheritdoc/>
    public class PlayerService(IApiService storageService) : IPlayerService
    {
        private readonly IApiService _storageService = storageService;

        public async Task<List<Player>> GetPlayers(List<PlayerEntity> playerEntities)
        {
            return await PlayerEntitiesToPlayers(playerEntities);
        }

        public async Task<List<Player>> GetPlayers()
        {
            var playerEntitiesResult = await _storageService.QueryEntities<PlayerEntity>();
            var playerEntities = playerEntitiesResult
                .OrderBy(p => p.Name)
                .ToList();
            return await PlayerEntitiesToPlayers(playerEntities);
        }

        public async Task DeletePlayer(string playerRowkey)
        {
            // Delete Ratings - Cannot use RatingService due to circular dependecy so must use StorageService directly
            var allRatingEntities = await _storageService.QueryEntities<RatingEntity>();
            var ratingsForPlayer = allRatingEntities.Where(o => o.PlayerRowKey == playerRowkey).ToList();
            foreach (var rating in ratingsForPlayer)
            {
                await _storageService.DeleteEntity<RatingEntity>(rating.RowKey);
            }

            // Delete TransactionEntity
            var transactionsResult = await _storageService.QueryEntities<TransactionEntity>();
            var transactions = transactionsResult.Where(t => t.PlayerRowKey == playerRowkey);
            foreach (var transaction in transactions)
            {
                await _storageService.DeleteEntity<TransactionEntity>(transaction.RowKey);
            }

            // Delete PlayerAtGameEntity
            var pagsResult = await _storageService.QueryEntities<PlayerAtGameEntity>();
            var pags = pagsResult.Where(pag => pag.PlayerRowKey == playerRowkey);
            foreach (var pag in pags)
            {
                await _storageService.DeleteEntity<PlayerAtGameEntity>(pag.RowKey);
            }

            // Delete PlayerEntity
            await _storageService.DeleteEntity<PlayerEntity>(playerRowkey);
        }

        public async Task<Player> GetPlayer(string rowKey)
        {
            var playerEntitiesResult = await _storageService.QueryEntities<PlayerEntity>();
            var playerEntities = playerEntitiesResult.Where(p => p.RowKey == rowKey).ToList();
            var matchingPlayerResult = await PlayerEntitiesToPlayers(playerEntities);
            var matchingPlayer = matchingPlayerResult.FirstOrDefault();
            if (matchingPlayer is not null)
            {
                return matchingPlayer;
            }

            // Create a null player to return
            Player nullPlayer = new(new PlayerEntity());
            return nullPlayer;
        }

        public async Task UpsertPlayerEntity(PlayerEntity playerEntity)
        {
            _ = await _storageService.UpsertEntity<PlayerEntity>(playerEntity);
        }

        private async Task<List<Player>> PlayerEntitiesToPlayers(List<PlayerEntity> playerEntities)
        {
            var players = new List<Player>();
            foreach (var pe in playerEntities)
            {
                var playersTransactionsResult = await _storageService.QueryEntities<TransactionEntity>();
                var playersTransactions = playersTransactionsResult
                    .Where(o => o.PlayerRowKey == pe.RowKey)
                    .OrderByDescending(o => o.Date)
                    .ToList();
                var playerBalance = playersTransactions.Sum(o => o.Amount);
                var player = new Player(pe)
                {
                    Transactions = playersTransactions,
                    Balance = playerBalance
                };
                players.Add(player);
            }
            return players;
        }

    }
}