namespace STO.Models
{
    [ExcludeFromCodeCoverage]
    public class PlayerAtGameEntity : ITableEntity
    {
        [Required]
        public string PlayerRowKey { get; set; } = default!;
        [Required]
        public string GameRowKey { get; set; } = default!;
        [Required]
        public Enums.PlayingStatus Forecast { get; set; }
        [Required]
        public bool Played { get; set;} = false;
        public string Team { get; set; } = default!;
        public string PartitionKey { get; set; } = default!;

        public string UrlSegment { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow!;
        public ETag ETag { get; set; } = default!;
    }
}