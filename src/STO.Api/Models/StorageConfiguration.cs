namespace STO.Api.Models
{
    /// <summary>
    /// Used to strongly type the "StorageConfiguration" appsettings section
    /// </summary>
    public class StorageConfiguration
    {
        /// <summary>
        /// The connection string for Azure Storage.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The name of a table in Azure Storage where entities are stored.
        /// </summary>
        public string DataTable { get; set; }
    }
}