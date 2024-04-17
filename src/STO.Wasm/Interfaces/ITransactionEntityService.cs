namespace STO.Wasm.Interfaces
{
    /// <summary>
    /// Service for working with TransactionEntities.
    /// </summary>
    public interface ITransactionEntityService
    {
        /// <summary>
        /// Gets all TransactionEntities.
        /// </summary>
        /// <returns>List of TransactionEntities.</returns>
        public List<TransactionEntity> GetTransactionEntities();

        /// <summary>
        /// Gets a specific TransactionEntity based on its RowKey
        /// </summary>
        /// <param name="rowKey"></param>
        /// <returns>a TransactionEntity</returns>
        public TransactionEntity GetTransactionEntity(string rowKey);

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