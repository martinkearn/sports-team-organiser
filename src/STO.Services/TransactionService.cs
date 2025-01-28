using STO.Models;
using STO.Models.Interfaces;

namespace STO.Services;

public class TransactionService(IDataService dataService) : ITransactionService
{
    
    private IEnumerable<TransactionEntity> GetTransactionEntities()
    {
        return [.. dataService.TransactionEntities.OrderByDescending(o => o.Date.DateTime)];
    }

    private Transaction ConstructTransaction(string transactionId)
    {
        // Get TransactionEntity
        var te = GetTransactionEntities().SingleOrDefault(te => te.RowKey == transactionId);
        
        if (te == null)
        {
            throw new KeyNotFoundException();
        }

        // Get PlayerEntity
        var pe = dataService.PlayerEntities.Single(p => p.RowKey == te.PlayerRowKey);

        // Get GameEntity
        var ge = dataService.GameEntities.SingleOrDefault(g => g.RowKey == te.GameRowKey);

        // Calculate label
        var label = te.Amount < 0 ? 
            $"Charge of £{Math.Abs(te.Amount)} to {pe.Name} on {te.Date.DateTime:dd MMM}" : 
            $"Payment of £{te.Amount} from {pe.Name} on {te.Date.DateTime:dd MMM}";

        // Construct Transaction
        var t = new Transaction()
        {
            Id = te.RowKey,
            Amount = te.Amount,
            Notes = te.Notes,
            DateTime = te.Date.DateTime,
            UrlSegment = te.UrlSegment,
            Label = label,
            PlayerId = te.PlayerRowKey,
            PlayerName = pe.Name,
            PlayerUrlSegment = pe.UrlSegment,
            GameId = te.GameRowKey,
            LastUpdated = te.Timestamp!.Value.DateTime,
        };
        if (ge != null)
        {
            t.GameLabel = ge.Title;
        }

        return t;
    }
    
    private static TransactionEntity DeconstructTransaction(Transaction transaction)
    {
        var te = new TransactionEntity()
        {
            PlayerRowKey = transaction.PlayerId,
            Amount = transaction.Amount,
            Date = new DateTimeOffset(transaction.DateTime),
            Notes = transaction.Notes,
            UrlSegment = transaction.UrlSegment,
            GameRowKey = transaction.GameId,
            RowKey = transaction.Id,
        };
        return te;
    }

    public List<Transaction> GetTransactions(int? skip, int? take)
    {
        // Get all TransactionEntity
        var tes = GetTransactionEntities();
        
        // Apply Skip and Take only if values are provided
        tes = skip.HasValue ? tes.Skip(skip.Value) : tes;
        tes = take.HasValue ? tes.Take(take.Value) : tes;

        var ts = tes.Select(te => ConstructTransaction(te.RowKey)).ToList();

        return ts;
    }
    
    public List<Transaction> GetTransactions(int? skip, int? take, string playerId)
    {
        // If playerId not defined, do not filter by player
        if (string.IsNullOrEmpty(playerId))
        {
            return GetTransactions(skip, take);
        }

        // Get player's TransactionEntity
        var tes = GetTransactionEntities().Where(te => te.PlayerRowKey == playerId);
        
        // Apply Skip and Take only if values are provided
        tes = skip.HasValue ? tes.Skip(skip.Value) : tes;
        tes = take.HasValue ? tes.Take(take.Value) : tes;

        var ts = tes.Select(te => ConstructTransaction(te.RowKey)).ToList();

        return ts;
    }

    public List<Transaction> GetTransactions(string playerId)
    {
        throw new NotImplementedException();
    }

    public Transaction GetTransaction(string id)
    {
        return ConstructTransaction(id);
    }

    public Transaction GetTransactionByUrlSegment(string urlSegment)
    {
        // Get TransactionEntity for this UrlSegment
        var te = GetTransactionEntities().FirstOrDefault(te => te.UrlSegment == urlSegment);

        if (te == null)
        {
            throw new KeyNotFoundException();
        }

        return ConstructTransaction(te.RowKey);
    }

    public async Task DeleteTransactionAsync(string id)
    {
        await dataService.DeleteEntityAsync<TransactionEntity>(id);
    }

    public async Task UpsertTransactionAsync(Transaction transaction)
    {
        // Calc UrlSegment
        transaction.UrlSegment = $"{transaction.PlayerUrlSegment}-{transaction.Amount}-{transaction.DateTime:dd-MM-yyyy-HH-mm-ss}";

        // Deconstruct to TransactionEntity
        var te = DeconstructTransaction(transaction);
        await dataService.UpsertEntityAsync(te);
    }
}