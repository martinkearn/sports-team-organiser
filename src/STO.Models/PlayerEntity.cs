namespace STO.Models
{
    public class PlayerEntity : ITableEntity
    {
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                UrlSegment = value.Replace(" ", "-").ToLowerInvariant();
            }
        }

        private string _name;
        
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