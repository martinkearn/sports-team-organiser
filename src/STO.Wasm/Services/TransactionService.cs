namespace STO.Wasm.Services
{
    /// <inheritdoc/>
    public class TransactionService(IDataService dataService, IPlayerEntityService playerEntityService) : ITransactionService
	{
		private readonly IDataService _dataService = dataService;
		private readonly IPlayerEntityService _playerEntityService = playerEntityService;


		public async Task<List<Transaction>> GetTransactions(List<TransactionEntity> transactionEntities)
        {
            return await TransactionEntitiesToTransactions(transactionEntities);
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            var TransactionEntitiesResult = await _dataService.QueryEntities<TransactionEntity>();
            return await TransactionEntitiesToTransactions(TransactionEntitiesResult);
        }

        public async Task DeleteTransactionEntity(string rowKey)
        {
            await _dataService.DeleteEntity<TransactionEntity>(rowKey);
        }

        public async Task DeleteTransactionEntiesForPlayer(string playerRowKey)
        {
            List<Transaction> allTransactions = await GetTransactions();
            var playerTransactions = allTransactions.Where(o => o.Player.PlayerEntity.RowKey == playerRowKey).ToList();
            var playerTransactionRowkeys = playerTransactions.Select(e => e.TransactionEntity.RowKey).ToList();
            foreach (var t in playerTransactionRowkeys)
            {
                await DeleteTransactionEntity(t);
            }
        }

        public async Task<Transaction> GetTransaction(string rowKey)
        {
            var transactionEntitiesResult = await _dataService.QueryEntities<TransactionEntity>();
            var transactionEntities = transactionEntitiesResult.Where(o => o.RowKey == rowKey).ToList();
            var transactionsResult = await TransactionEntitiesToTransactions(transactionEntities);
            var matchingTransaction = transactionsResult.FirstOrDefault();

            if (matchingTransaction is not null)
            {
                return matchingTransaction;
            }

            // Create a null transaction to return
            Transaction nullTransaction = new(new TransactionEntity());
            return nullTransaction;
        }

        public async Task UpsertTransactionEntity(TransactionEntity transactionEntity)
        {
            await _dataService.UpsertEntity<TransactionEntity>(transactionEntity);
        }

        private async Task<List<Transaction>> TransactionEntitiesToTransactions(List<TransactionEntity> transactionEntities)
        {
            var transactions = new List<Transaction>();
            foreach (var te in transactionEntities)
            {
                var transactionPlayer = _playerEntityService.GetPlayer(te.PlayerRowKey);

				var Transaction = new Transaction(te)
                {
                    Player = transactionPlayer
                };
                transactions.Add(Transaction);
            }
            return [.. transactions.OrderByDescending(o => o.TransactionEntity.Date)];
        }
    }
}