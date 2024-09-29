namespace STO.Models
{
    [ExcludeFromCodeCoverage]
    public class RatingEntity : ITableEntity
    {
        [Required]
        public string PlayerRowKey { get; set; } = default!;
        [Required]
        public double Rating { get; set; } = 0;
        public string GameRowKey { get; set; } = default!;
        public string Notes { get; set; } = default!;
        public string UrlSegment { get; set; }
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow!;
        public ETag ETag { get; set; } = default!;
    }
}