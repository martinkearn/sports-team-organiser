using Azure;
using Azure.Data.Tables;

namespace STO.Models
{
    public class Player : ITableEntity
    {
        public string? Name { get; set; }
        public string? Tags { get; set; }
        public NextGameForecast NextGameForecast { get; set; }
        public bool Played { get; set; }
        public double DefaultRate { get; set; } = default!;
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow!;
        public ETag ETag { get; set; } = default!;
    }
}