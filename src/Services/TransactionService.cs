namespace STO.Services
{
    /// <inheritdoc/>
    public class TransactionService : ITransactionService
    {
        private readonly IStorageService _storageService;
        private readonly IPlayerService _playerService;
        private readonly IGameService _gameService;
        public TransactionService(IStorageService storageService, IPlayerService playerService, IGameService gameService)
        {
            _storageService = storageService;
            _playerService = playerService;
            _gameService = gameService;
        }

        public async Task<List<Transaction>> GetTransactions(List<TransactionEntity> transactionEntities)
        {
            return await TransactionEntitiesToTransactions(transactionEntities);
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            var TransactionEntities = _storageService.QueryEntities<TransactionEntity>().ToList();
            return await TransactionEntitiesToTransactions(TransactionEntities);
        }

        public async Task DeleteTransactionEntity(string rowKey)
        {
            await _storageService.DeleteEntity<TransactionEntity>(rowKey);
        }

        public async Task<Transaction> GetTransaction(string rowKey)
        {
            var transactionEntities = _storageService.QueryEntities<TransactionEntity>().Where(o => o.RowKey == rowKey).ToList();
            var transactions = await TransactionEntitiesToTransactions(transactionEntities);
            var matchingTransaction = transactions.FirstOrDefault();
            return matchingTransaction;
        }

        public async Task UpsertTransactionEntity(TransactionEntity transactionEntity)
        {
            await _storageService.UpsertEntity<TransactionEntity>(transactionEntity);
        }

        private async Task<List<Transaction>> TransactionEntitiesToTransactions(List<TransactionEntity> transactionEntities)
        {
            var transactions = new List<Transaction>();
            foreach (var te in transactionEntities)
            {
                var transactionPlayer = _playerService.GetPlayer(te.PlayerRowKey);
                var transactionGame = await _gameService.GetGame(te.GameRowKey);

                var Transaction = new Transaction(te)
                {
                    Player = transactionPlayer,
                    Game = transactionGame,
                };
                transactions.Add(Transaction);
            }
            return transactions.OrderByDescending(o => o.TransactionEntity.Date).ToList();
        }
    }
}