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

        public List<Transaction> GetTransactions(List<TransactionEntity> transactionEntities)
        {
            return TransactionEntitiesToTransactions(transactionEntities);
        }

        public List<Transaction> GetTransactions()
        {
            var TransactionEntities = _storageService.QueryEntities<TransactionEntity>()
                .OrderBy(o => o.Date)
                .ToList();
            return TransactionEntitiesToTransactions(TransactionEntities);
        }

        public async Task DeleteTransaction(string rowKey)
        {
            await _storageService.DeleteEntity<TransactionEntity>(rowKey);
        }

        public Transaction GetTransaction(string rowKey)
        {
            var transactionEntities = _storageService.QueryEntities<TransactionEntity>().Where(o => o.RowKey == rowKey).ToList();
            var matchingTransaction = TransactionEntitiesToTransactions(transactionEntities).FirstOrDefault();
            return matchingTransaction;
        }

        private List<Transaction> TransactionEntitiesToTransactions(List<TransactionEntity> transactionEntities)
        {
            var transactions = new List<Transaction>();
            foreach (var te in transactionEntities)
            {
                var transactionPlayer = _playerService.GetPlayer(te.PlayerRowKey);
                var transactionGame = _gameService.GetGame(te.GameRowKey);

                var Transaction = new Transaction(te)
                {
                    Player = transactionPlayer,
                    Game = transactionGame,
                };
                transactions.Add(Transaction);
            }
            return transactions;
        }
    }
}