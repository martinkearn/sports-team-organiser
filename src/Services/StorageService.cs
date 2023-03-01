using Microsoft.Extensions.Options;

namespace STO.Services
{
    public class StorageService : IStorageService
    {
        private readonly StorageConfiguration _options;

        public StorageService(IOptions<StorageConfiguration> storageConfigurationOptions)
        { 
            _options = storageConfigurationOptions.Value;
        }

        public StorageConfiguration GetConfiguration()
        {
            return _options;
        }
    }
}