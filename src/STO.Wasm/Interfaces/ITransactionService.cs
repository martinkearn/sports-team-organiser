namespace STO.Wasm.Interfaces
{
    /// <summary>
    /// Service for working with TransactionEntities.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Gets all TransactionEntities.
        /// </summary>
        /// <returns>List of TransactionEntity.</returns>
        public List<TransactionEntity> GetTransactionEntities();

        /// <summary>
        /// Gets all TransactionEntities for agiven player by PlayerRoweKey
        /// </summary>
        /// <param name="playerRowKey">The PlayerRowKey to match trasnactions for</param>
        /// <returns>List of TransactionEntity.</returns>
        public List<TransactionEntity> GetTransactionEntitiesForPlayerEntity(string playerRowKey);

        /// <summary>
        /// Gets a specific TransactionEntity based on its RowKey
        /// </summary>
        /// <param name="rowKey">The RowKey for the Transaction to get</param>
        /// <returns>A TransactionEntity</returns>
        public TransactionEntity GetTransactionEntity(string rowKey);
        
        /// <summary>
        /// Gets a specific TransactionEntity based on its UrlSegment
        /// </summary>
        /// <param name="urlSegment">The UrlSegment for the Transaction to get</param>
        /// <returns>A TransactionEntity</returns>
        public TransactionEntity GetTransactionEntityByUrlSegment(string urlSegment);

        /// <summary>
        /// Deletes a TransactionEntity.
        /// </summary>
        /// <param name="rowKey">The RowKey for the TransactionEntity to delete.</param>
        public Task DeleteTransactionEntityAsync(string rowKey);

        /// <summary>
        /// Adds a new TransactionEntity.
        /// </summary>
        /// <param name="transactionEntity">The TransactionEntity to upsert.</param>
        public Task UpsertTransactionEntityAsync(TransactionEntity transactionEntity);
        
    }
}