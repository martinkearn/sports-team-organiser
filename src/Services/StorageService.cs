using Azure.Data.Tables;
using Microsoft.Extensions.Options;

namespace STO.Services
{
    public class StorageService : IStorageService
    {
        private readonly StorageConfiguration _options;
        private readonly TableClient _playersTableClient;

        private string _playersPartitionKey = "player";

        public StorageService(IOptions<StorageConfiguration> storageConfigurationOptions)
        { 
            _options = storageConfigurationOptions.Value;

            _playersTableClient = new TableClient(_options.ConnectionString, _options.PlayersTable);
            _playersTableClient.CreateIfNotExists();
        }

        public void DeletePlayer(string rowKey)
        {
            throw new NotImplementedException();
        }

        public StorageConfiguration GetConfiguration()
        {
            return _options;
        }

        public List<Player> QueryPlayers(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                filter = $"PartitionKey eq '{_playersPartitionKey}'";
            }

            var players = _playersTableClient.Query<Player>(filter).ToList();

            return players;
        }

        public async Task<Player> UpsertPlayer(Player player)
        {
            // Complete required values
            if (player.RowKey == default) player.RowKey = Guid.NewGuid().ToString();
            if (player.PartitionKey == default) player.PartitionKey = _playersPartitionKey;

            // Upsert player
            await _playersTableClient.UpsertEntityAsync<Player>(player, TableUpdateMode.Replace);

            // Get player from storage
            var upsertedPlayer = this.QueryPlayers($"RowKey eq '{player.RowKey}'").First();

            // Return
            return upsertedPlayer;
        }
    }
}