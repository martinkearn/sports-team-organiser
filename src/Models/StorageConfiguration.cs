namespace STO.Models
{
    /// <summary>
    /// Used to strongly type the "StorageConfiguration" appsettings section
    /// </summary>
    public class StorageConfiguration
    {
        /// <summary>
        /// The connection string for Azure Storage.
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// The name of a table in Azure Storage where Player entities are stored.
        /// </summary>
        public string? PlayersTable { get; set; }

        /// <summary>
        /// The name of a table in Azure Storage where Game entities are stored.
        /// </summary>
        public string? GamesTable { get; set; }   
   
    }
}