namespace STO.Models
{
    [ExcludeFromCodeCoverage]
    public class DataDetailsEntity : ITableEntity
    {
        [Required]
        public long LastWriteEpoch { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds()!;

        [Required]
        public DateTimeOffset LastWriteDate { get; set; } = DateTimeOffset.UtcNow!;

        public string PartitionKey { get; set; } = typeof(DataDetailsEntity).ToString();
        public string RowKey { get; set; } = typeof(DataDetailsEntity).ToString();
        public DateTimeOffset? Timestamp { get; set; } = DateTimeOffset.UtcNow!;
        public ETag ETag { get; set; } = default!;
    }
}