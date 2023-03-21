namespace STO.Services
{
    /// <inheritdoc/>
    public class PlayerService : IPlayerService
    {
        private readonly IStorageService _storageService;
        public PlayerService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public List<Player> GetPlayers(List<PlayerEntity> playerEntities)
        {
            return PlayerEntitiesToPlayers(playerEntities);
        }

        public List<Player> GetPlayers()
        {
            var playerEntities = _storageService.QueryEntities<PlayerEntity>()
                .OrderBy(p => p.Name)
                .ToList();
            return PlayerEntitiesToPlayers(playerEntities);
        }

        public async Task DeletePlayer(string playerRowkey)
        {
            // Delete TransactionEntity
            var transactions = _storageService.QueryEntities<TransactionEntity>().Where(t => t.PlayerRowKey == playerRowkey);
            foreach (var transaction in transactions)
            {
                await _storageService.DeleteEntity<TransactionEntity>(transaction.RowKey);
            }

            // Delete PlayerAtGameEntity
            var pags = _storageService.QueryEntities<PlayerAtGameEntity>().Where(pag => pag.PlayerRowKey == playerRowkey);
            foreach (var pag in pags)
            {
                await _storageService.DeleteEntity<PlayerAtGameEntity>(pag.RowKey);
            }

            // Delete PlayerEntity
            await _storageService.DeleteEntity<PlayerEntity>(playerRowkey);
        }

        public Player GetPlayer(string rowKey)
        {
            var playerEntities = _storageService.QueryEntities<PlayerEntity>().Where(p => p.RowKey == rowKey).ToList();
            var matchingPlayer = PlayerEntitiesToPlayers(playerEntities).FirstOrDefault();
            return matchingPlayer;
        }

        public async Task UpsertPlayerEntity(PlayerEntity playerEntity)
        {
            await _storageService.UpsertEntity<PlayerEntity>(playerEntity);
        }

        private List<Player> PlayerEntitiesToPlayers(List<PlayerEntity> playerEntities)
        {
            var players = new List<Player>();
            foreach (var pe in playerEntities)
            {
                var playersTransactions = _storageService.QueryEntities<TransactionEntity>().Where(t => t.PlayerRowKey == pe.RowKey).ToList();
                var playerBalance = playersTransactions.Sum(t => t.Amount);
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