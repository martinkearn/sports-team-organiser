namespace STO.Models.Interfaces;

/// <summary>
/// Service for working with Transactions.
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Gets all Transactions.
    /// </summary>
    /// <returns>List of Transaction.</returns>
    public List<Transaction> GetTransactions();
    
    /// <summary>
    /// Gets all Transactions for a specified Player Id.
    /// </summary>
    /// <returns>List of Transaction.</returns>
    public List<Transaction> GetTransactions(string playerId);

    /// <summary>
    /// Gets a specific Transaction based on the Id
    /// </summary>
    /// <param name="id">The Id for the Transaction to get</param>
    /// <returns>A Transaction</returns>
    public Transaction GetTransaction(string id);

    /// <summary>
    /// Deletes a Transaction.
    /// </summary>
    /// <param name="id">The id for the Transaction to delete.</param>
    public Task DeleteTransactionAsync(string id);

    /// <summary>
    /// Adds a new Transaction.
    /// </summary>
    /// <param name="transaction">The Transaction to upsert.</param>
    public Task UpsertTransactionAsync(Transaction transaction);
}