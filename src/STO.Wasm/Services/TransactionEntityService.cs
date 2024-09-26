namespace STO.Wasm.Services
{
    /// <inheritdoc/>
    public class TransactionEntityService(IDataService dataService) : ITransactionEntityService
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

        public TransactionEntity GetTransactionEntityByUrlSegment(string urlSegment)
        {
            var tes = GetTransactionEntities();
            return tes.First(o => o.UrlSegment == urlSegment);
        }

        public async Task DeleteTransactionEntityAsync(string rowKey)
        {
            await dataService.DeleteEntityAsync<TransactionEntity>(rowKey);
        }

        public async Task UpsertTransactionEntityAsync(TransactionEntity transactionEntity)
        {
            if (transactionEntity.PlayerRowKey is null) return;
			
            // Set the UrlSegment
            // Cannot do this as setter for UrlSegment because we cannot resolve the PlayerEntity there
            // Cannot use PlayerEntityService due to circular dependency. need to work with data service directly to get player details
            var pes = dataService.PlayerEntities;
            var player = pes.First(o => o.RowKey == transactionEntity.PlayerRowKey);
            transactionEntity.UrlSegment = $"{player.UrlSegment}-{transactionEntity.Date.DateTime:dd-MM-yyyy-HH-mm-ss}";
            
            await dataService.UpsertEntityAsync(transactionEntity);
        }
    }
}