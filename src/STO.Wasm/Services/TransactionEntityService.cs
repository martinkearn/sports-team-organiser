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

        public async Task DeleteTransactionEntity(string rowKey)
        {
            await _dataService.DeleteEntity<TransactionEntity>(rowKey);
        }

        public async Task UpsertTransactionEntity(TransactionEntity transactionEntity)
        {
            await _dataService.UpsertEntity(transactionEntity);
        }
    }
}