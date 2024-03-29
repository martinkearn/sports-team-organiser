using Azure.Data.Tables;

namespace STO.Wasm.Services
{
    /// <inheritdoc/>
    public class DataService(ILocalStorageService localStorageService, IApiService apiService) : IDataService
    {
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
                await _localStore.SetItemAsync(typeof(T).Name, data);
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
                await _localStore.SetItemAsync(typeof(T).Name, data);
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
            if (_apiService is null)
            {
                throw new Exception();
            }

            if (_localStore is null)
            {
                throw new Exception();
            }

			// Look for reasons not to load data from API
			var apiDataDetailsEntities = await _apiService.ApiGet<DataDetailsEntity>();
			if (apiDataDetailsEntities?.Count > 0)
			{
				var apiDataDetailsEntity = apiDataDetailsEntities.First();
				var localStorageDataDetailsEntities = await _localStore.GetItemAsync<List<DataDetailsEntity>>(typeof(DataDetailsEntity).Name);
				if (localStorageDataDetailsEntities?.Count > 0)
				{
					var localStorageDataDetailsEntity = localStorageDataDetailsEntities.First();
					if (apiDataDetailsEntity.LastWriteEpoch == localStorageDataDetailsEntity?.LastWriteEpoch)
					{
						return;
					}
				}
			}


			// Load all data
			var playerTask = GetDataFromApi<PlayerEntity>();
            var gameTask =  GetDataFromApi<GameEntity>();
            var transactionTask =  GetDataFromApi<TransactionEntity>();
            var playerAtGameTask =  GetDataFromApi<PlayerAtGameEntity>();
            var ratingTask =  GetDataFromApi<RatingEntity>();
            var dataDetailsTask = GetDataFromApi<DataDetailsEntity>();

            await Task.WhenAll(playerTask, gameTask, transactionTask, playerAtGameTask, ratingTask, dataDetailsTask);
        }

        private async Task GetDataFromApi<T>() where T : class, ITableEntity
        {
            var data = await _apiService.ApiGet<T>();
            if (data is not null)
            {
                await _localStore.SetItemAsync(typeof(T).Name, data);
            }
        } 
    }
}