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
        
        // Complete base Transaction properties
        var t = new Transaction()
        {
            Id = te.RowKey,
            Amount = te.Amount,
            DateTime = te.Date.DateTime,
            Notes = te.Notes,
            PlayerId = te.PlayerRowKey,
            GameId = te.GameRowKey,
            LastUpdated = te.Timestamp!.Value.DateTime,
        };
        
        // Fill Transaction
        t = FillTransaction(t);

        return t;
    }

    private Transaction FillTransaction(Transaction t)
    {
        // Get PlayerEntity
        var pe = dataService.PlayerEntities.Single(p => p.RowKey == t.PlayerId);
        
        // Calculate label
        var label = t.Amount < 0 ? 
            $"Charge of £{Math.Abs(t.Amount)} to {pe.Name} on {t.DateTime:dd MMM}" : 
            $"Payment of £{t.Amount} from {pe.Name} on {t.DateTime:dd MMM}";
        
        // Calculate UrlSegment
        var urlSegment = $"{pe.UrlSegment}-{t.DateTime:dd-MM-yyyy-HH-mm-ss}";

        // Get GameEntity
        var ge = dataService.GameEntities.SingleOrDefault(g => g.RowKey == t.GameId);
        
        // Construct Transaction
        t.PlayerName = pe.Name;
        t.PlayerUrlSegment = pe.UrlSegment;
        t.Label = label;
        t.UrlSegment = urlSegment;
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
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        // We only need to store transactions with a positive or negative amount
        if (transaction.Amount != 0)
        {
            // Fill Transaction - to populate UrlSegment
            transaction = FillTransaction(transaction);
        
            // Deconstruct to TransactionEntity
            var te = DeconstructTransaction(transaction);
            await dataService.UpsertEntityAsync(te);
        }
    }
}