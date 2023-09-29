namespace STO.Interfaces
{
    /// <summary>
    /// Service for working with Transactions.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Converts a list of TransactionEntities to a full list of Transactions.
        /// </summary>
        /// <param name="transactionEntities">The list of TransactionEntities to convert.</param>
        /// <returns>List of Transactions.</returns>
        public Task<List<Transaction>> GetTransactions(List<TransactionEntity> transactionEntities);

        /// <summary>
        /// Converts all TransactionEntities to a full list of Transactions.
        /// </summary>
        /// <returns>List of Transactions.</returns>
        public Task<List<Transaction>> GetTransactions();

        /// <summary>
        /// Gets a single Transaction by row key.
        /// </summary>
        /// <param name="rowKey">The RowKey for the TransactionEntity to get.</param>
        /// <returns>A Transaction.</returns>
        public Task<Transaction> GetTransaction(string rowKey);

        /// <summary>
        /// Deletes a TransactionEntity.
        /// </summary>
        /// <param name="rowKey">The RowKey for the TransactionEntity to delete.</param>
        public Task DeleteTransactionEntity(string rowKey);

        /// <summary>
        /// Deletes all TransactionEntity for a player.
        /// </summary>
        /// <param name="rowKey">The RowKey for the PlayerEntity to delete TransactionEntity for.</param>
        public Task DeleteTransactionEntiesForPlayer(string playerRowKey);

        /// <summary>
        /// Adds a new TransactionEntity.
        /// </summary>
        /// <param name="transactionEntity">The TransactionEntity to upsert.</param>
        public Task UpsertTransactionEntity(TransactionEntity transactionEntity);
        
        /// <summary>
        /// A string which can be used as notes when mapping a transaction to a game
        /// </summary>
        /// <param name="gameTitle"></param>
        /// <returns></returns>
        public string GetNotesForGame(string gameTitle);
    }
}