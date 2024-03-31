using Azure.Data.Tables;

namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class CachedDataService(IApiService apiService, ILocalStorageService localStorageService) : ICachedDataService
	{
		private readonly IApiService _apiService = apiService;
		private readonly ILocalStorageService _localStorageService = localStorageService;

		public async Task DeleteEntity<T>(string rowKey) where T : class, ITableEntity
		{
			// Delete Entity
			await _apiService.ApiDelete<T>(rowKey);

			// Refresh data from storage
			await RefreshEntitiesFromApi<T>();
		}

		public async Task<T> UpsertEntity<T>(T entity) where T : class, ITableEntity
		{
			// Complete required values
			if (entity.RowKey == default) entity.RowKey = Guid.NewGuid().ToString();
			if (entity.PartitionKey == default) entity.PartitionKey = typeof(T).ToString();

			// Upsert entity
			await _apiService.ApiPost<T>(entity);

			// Refresh data from storage
			await RefreshEntitiesFromApi<T>();

			// Return
			return entity;
		}

		public async Task<List<T>> QueryEntities<T>() where T : class, ITableEntity
		{
			var data = await _localStorageService.GetItemAsync<List<T>>(typeof(T).Name);
			if (data is not null)
			{
				return data;
			}

			return [];
		}

		public async Task LoadData()
		{
			// Refresh caches
			var playerTask = RefreshEntitiesFromApi<PlayerEntity>();
			var gameTask = RefreshEntitiesFromApi<GameEntity>();
			var transactionTask = RefreshEntitiesFromApi<TransactionEntity>();
			var playerAtGameTask = RefreshEntitiesFromApi<PlayerAtGameEntity>();
			var ratingTask = RefreshEntitiesFromApi<RatingEntity>();
			var dataDetailsTask = RefreshEntitiesFromApi<DataDetailsEntity>();

			await Task.WhenAll(playerTask, gameTask, transactionTask, playerAtGameTask, ratingTask, dataDetailsTask);
		}

		private async Task RefreshEntitiesFromApi<T>() where T : class, ITableEntity
		{
			var data = await _apiService.ApiGet<T>();
            if (data is not null)
			{
                await _localStorageService.SetItemAsync(typeof(T).Name, data);
            }
		}

	}
}