using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace STO.Wasm.Services
{
    /// <inheritdoc/>
    public class DataService(IOptions<ApiConfiguration> storageConfigurationOptions, ILocalStorageService localStorageService, IApiService apiService) : IDataService
    {
        private readonly ApiConfiguration _options = storageConfigurationOptions.Value;

        private readonly ILocalStorageService _localStore = localStorageService;

        private readonly IApiService _apiService = apiService;

        public async Task DeleteEntity<T>(string rowKey) where T : class, ITableEntity
        {
            // Delete entity in browser storage
            var data = await _localStore.GetItemAsync<List<T>>(typeof(T).Name);

            if (data is not null)
            {
                // Remove entity from list
                data.RemoveAll(o => o.RowKey == rowKey);
                
                // Save new List to browser storage
                await _localStore.SetItemAsync<List<T>>(typeof(T).Name, data);
            }

            // Delete Entity in Api
            await Task.Run(() => _apiService.ApiDelete<T>(rowKey));
        } 

        public async Task<T> UpsertEntity<T>(T entity) where T : class, ITableEntity
        {
            // Complete required values
            if (entity.RowKey == default) entity.RowKey = Guid.NewGuid().ToString();
            if (entity.PartitionKey == default) entity.PartitionKey = typeof(T).ToString();

            // Upsert entity in browser storage
            var data = await _localStore.GetItemAsync<List<T>>(typeof(T).Name);
            if (data is not null)
            {
                // Remove entity from list
                data.RemoveAll(o => o.RowKey == entity.RowKey);

                // Add new entity to list
                data.Add(entity);
                
                // Save new List to browser storage
                await _localStore.SetItemAsync<List<T>>(typeof(T).Name, data);
            }

            // Upsert entity in Api
            await Task.Run(() => _apiService.ApiPost<T>(entity));
            
            // Return
            return entity;
        } 

        public async Task<List<T>> QueryEntities<T>() where T : class, ITableEntity
        {
            var data = await _localStore.GetItemAsync<List<T>>(typeof(T).Name);
            if (data is not null)
            {
                return (List<T>)Convert.ChangeType(data, typeof(List<T>));
            }
            else
            {
                return [];
            }
        }   

        public async Task LoadData()
        {
            // TO DO need to check that cache is fresh and re-cache if not. By default, cache will load from Api when blazor app loads initially
            var playerTask = GetDataFromApi<PlayerEntity>();
            var gameTask =  GetDataFromApi<GameEntity>();
            var transactionTask =  GetDataFromApi<TransactionEntity>();
            var playerAtGameTask =  GetDataFromApi<PlayerAtGameEntity>();
            var ratingTask =  GetDataFromApi<RatingEntity>();
            var dataDetailsTask =  GetDataFromApi<DataDetailsEntity>();

            await Task.WhenAll(playerTask, gameTask, transactionTask, playerAtGameTask, ratingTask, dataDetailsTask);
        }

        private async Task GetDataFromApi<T>() where T : class, ITableEntity
        {
            var data = await _apiService.ApiGet<T>();
            if (data is not null)
            {
                await _localStore.SetItemAsync<List<T>>(typeof(T).Name, data);
            }
        } 
    }
}