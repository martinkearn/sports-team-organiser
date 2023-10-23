namespace STO.Services
{
    /// <inheritdoc/>
    public class TransactionService : ITransactionService
    {
        private readonly IStorageService _storageService;
        private readonly IPlayerService _playerService;
        public TransactionService(IStorageService storageService, IPlayerService playerService)
        {
            _storageService = storageService;
            _playerService = playerService;
        }

        public List<Transaction> GetTransactions(List<TransactionEntity> transactionEntities)
        {
            return TransactionEntitiesToTransactions(transactionEntities);
        }

        public List<Transaction> GetTransactions()
        {
            var TransactionEntities = _storageService.QueryEntities<TransactionEntity>().ToList();
            return TransactionEntitiesToTransactions(TransactionEntities);
        }

        public async Task DeleteTransactionEntity(string rowKey)
        {
            await _storageService.DeleteEntity<TransactionEntity>(rowKey);
        }

        public async Task DeleteTransactionEntiesForPlayer(string playerRowKey)
        {
            List<Transaction> allTransactions = GetTransactions();
            var playerTransactions = allTransactions.Where(o => o.Player.PlayerEntity.RowKey == playerRowKey).ToList();
            var playerTransactionRowkeys = playerTransactions.Select(e => e.TransactionEntity.RowKey).ToList();
            foreach (var t in playerTransactionRowkeys)
            {
                await DeleteTransactionEntity(t);
            }
        }

        public Transaction GetTransaction(string rowKey)
        {
            var transactionEntities = _storageService.QueryEntities<TransactionEntity>().Where(o => o.RowKey == rowKey).ToList();
            var transactions = TransactionEntitiesToTransactions(transactionEntities);
            var matchingTransaction = transactions.FirstOrDefault();
            return matchingTransaction;
        }

        public async Task UpsertTransactionEntity(TransactionEntity transactionEntity)
        {
            await _storageService.UpsertEntity<TransactionEntity>(transactionEntity);
        }

        public string GetNotesForGame(string gameRowKey)
        {
            var gameEntity = _storageService.QueryEntities<GameEntity>().Where(o => o.RowKey == gameRowKey).FirstOrDefault();
            var notes = $"For game {gameEntity.Date.Date.ToString("dd MMM yyyy")}";
            return notes;
        }

        private List<Transaction> TransactionEntitiesToTransactions(List<TransactionEntity> transactionEntities)
        {
            var transactions = new List<Transaction>();
            foreach (var te in transactionEntities)
            {
                var transactionPlayer = _playerService.GetPlayer(te.PlayerRowKey);

                var Transaction = new Transaction(te)
                {
                    Player = transactionPlayer
                };
                transactions.Add(Transaction);
            }
            return transactions.OrderByDescending(o => o.TransactionEntity.Date).ToList();
        }
    }
}