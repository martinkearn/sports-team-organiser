using Azure;
using Azure.Data.Tables;

namespace STO.Models
{
    public class PlayerEntity : ITableEntity
    {
        public string Name { get; set; }
        public string Tags { get; set; }
        public Enums.PlayerPosition Position { get; set; }
        public double DefaultRate { get; set; }
        public int AdminRating { get; set; }
        public string UrlSegment { get; set; }
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow!;
        public ETag ETag { get; set; } = default!;
    }
}