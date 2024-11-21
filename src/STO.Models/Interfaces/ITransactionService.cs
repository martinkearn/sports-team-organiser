namespace STO.Models.Interfaces;

/// <summary>
/// Service for working with Transactions.
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Gets all Transactions.
    /// </summary>
    /// <param name="skip">How many items to skip before taking. For example, if set to 30, the return will start from the 31st item. If null, no items will be skipped</param>
    /// <param name="take">How many items to take For example, if set to 20, 20 items will be returned. If null, all items will be taken from the skip</param>
    /// <returns>List of Transaction.</returns>
    public List<Transaction> GetTransactions(int? skip, int? take);
    
    /// <summary>
    /// Gets all Transactions.
    /// </summary>
    /// <param name="skip">How many items to skip before taking. For example, if set to 30, the return will start from the 31st item. If null, no items will be skipped</param>
    /// <param name="take">How many items to take For example, if set to 20, 20 items will be returned. If null, all items will be taken from the skip</param>
    /// <param name="playerId">Optional player to filter on</param>
    /// <returns>List of Transaction.</returns>
    public List<Transaction> GetTransactions(int? skip, int? take, string playerId);

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