namespace STO.Wasm.Services
{
    /// <inheritdoc/>
    public class TransactionService(IDataService dataService) : ITransactionService
    {
        public List<TransactionEntity> GetTransactionEntities()
        {
            return [.. dataService.TransactionEntities.OrderByDescending(o => o.Date)];
        }

        public List<TransactionEntity> GetTransactionEntitiesForPlayerEntity(string playerRowKey)
        {
            var tes = GetTransactionEntities();
            return tes.Where(o => o.PlayerRowKey == playerRowKey).ToList();
        }

        public TransactionEntity GetTransactionEntity(string rowKey)
        {
            var tes = GetTransactionEntities();

            return tes.First(o => o.RowKey == rowKey);
        }

        public async Task DeleteTransactionEntityAsync(string rowKey)
        {
            await dataService.DeleteEntityAsync<TransactionEntity>(rowKey);
        }

        public async Task UpsertTransactionEntityAsync(TransactionEntity transactionEntity)
        {
            await dataService.UpsertEntityAsync(transactionEntity);
        }
    }
}