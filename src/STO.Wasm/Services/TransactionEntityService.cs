namespace STO.Wasm.Services
{
    /// <inheritdoc/>
    public class TransactionEntityService(ICachedDataService dataService) : ITransactionEntityService
    {
        private readonly ICachedDataService _dataService = dataService;

        public List<TransactionEntity> GetTransactionEntities()
        {
            return _dataService.TransactionEntities;
        }

        public TransactionEntity GetTransactionEntity(string rowKey)
        {
            var tes = GetTransactionEntities();
            try
            {
                return tes.First(o => o.RowKey == rowKey);
            }
            catch (Exception ex)
            {
                var m = ex.Message;
                return new TransactionEntity();
            }
        }

        public async Task DeleteTransactionEntityAsync(string rowKey)
        {
            await _dataService.DeleteEntityAsync<TransactionEntity>(rowKey);
        }

        public async Task UpsertTransactionEntityAsync(TransactionEntity transactionEntity)
        {
            await _dataService.UpsertEntityAsync(transactionEntity);
        }
    }
}