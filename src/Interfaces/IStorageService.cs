using STO.Models;

namespace STO.Interfaces
{
    /// <summary>
    /// Service for working with Azure Storage.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Returns configuration data.
        /// </summary>
        /// <returns>StorageConfiguration based on current envirnment.</returns>
        public StorageConfiguration GetConfiguration();
    }
}