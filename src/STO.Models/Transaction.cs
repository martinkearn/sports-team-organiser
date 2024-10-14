namespace STO.Models;

public class Transaction
{
    public string Id { get; set; }
    
    public string PlayerRowKey { get; set; } = default!;
    
    public double Amount { get; set; }
    
    public DateTime DateTime { get; set; } = DateTime.UtcNow!;
    
    public string Notes { get; set; } = default!;
    
    public string UrlSegment { get; set; }
    
    public string PlayerLabel { get; set; }
    
    public string GameRowKey { get; set; } = default!;
    
    public string GameLabel { get; set; } = default!;
    
    public DateTime LastUpdated { get; set; }
    
    public string Label { get; set; }
}