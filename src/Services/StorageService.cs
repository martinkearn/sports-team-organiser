using Azure.Data.Tables;
using Microsoft.Extensions.Options;

namespace STO.Services
{
    public class StorageService : IStorageService
    {
        private readonly StorageConfiguration _options;
        private readonly TableClient _tableClient;

        public StorageService(IOptions<StorageConfiguration> storageConfigurationOptions)
        { 
            _options = storageConfigurationOptions.Value;

            _tableClient = new TableClient(_options.ConnectionString, _options.DataTable);
            _tableClient.CreateIfNotExists();
        }

        public async Task DeleteEntity(string partitionKey, string rowKey)
        {
            await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        public StorageConfiguration GetConfiguration()
        {
            return _options;
        }

        public List<Player> QueryEntities(string partitionKey, string? filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                filter = $"PartitionKey eq '{partitionKey}'";
            }

            var players = _tableClient.Query<Player>(filter).ToList();

            return players;
        }

        public async Task<Player> UpsertPlayer(Player player)
        {
            // Complete required values
            if (player.RowKey == default) player.RowKey = Guid.NewGuid().ToString();
            if (player.PartitionKey == default) player.PartitionKey = Constants.PlayerPartitionKey;

            // Upsert player
            await _tableClient.UpsertEntityAsync<Player>(player, TableUpdateMode.Replace);

            // Get player from storage
            var upsertedPlayer = this.QueryEntities(Constants.PlayerPartitionKey, $"RowKey eq '{player.RowKey}'").First();

            // Return
            return upsertedPlayer;
        }
    }
}