using Azure.Data.Tables;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class CachedDataService(IApiService apiService, ILocalStorageService localStorageService) : ICachedDataService
	{

		private readonly IApiService _apiService = apiService;
		private readonly ILocalStorageService _localStorageService = localStorageService;

		public List<PlayerEntity> PlayerEntities { get; set; } = [];
		public List<TransactionEntity> TransactionEntities { get; set; } = [];
		public List<RatingEntity> RatingEntities { get; set; } = [];
		public List<GameEntity> GameEntities { get; set; } = [];
		public List<PlayerAtGameEntity> PlayerAtGameEntities { get; set; } = [];

		public async Task DeleteEntityAsync<T>(string rowKey) where T : class, ITableEntity
		{
			// Delete Entity
			await _apiService.ApiDeleteAsync<T>(rowKey);

			// Refresh data from storage
			await RefreshEntitiesFromApiAsync<T>();
		}

		public async Task<T> UpsertEntityAsync<T>(T entity) where T : class, ITableEntity
		{
			// Complete required values
			if (entity.RowKey == default) entity.RowKey = Guid.NewGuid().ToString();
			if (entity.PartitionKey == default) entity.PartitionKey = typeof(T).ToString();

            // Upsert entity to Api
            await _apiService.ApiPostAsync(entity);

            // Refresh data from storage
            // TO DO ... does this call actually need to happen? Providing the local storage is update with the same data as the Api store, we do not need to refresh the local store from the Api store? Can't we just update the local store directly? Same for Delete
            //await RefreshEntitiesFromApiAsync<T>();

            // Upsert entity to local store
            if (typeof(T) == typeof(PlayerAtGameEntity))
            {
				var convertedEntity = (PlayerAtGameEntity)Convert.ChangeType(entity, typeof(PlayerAtGameEntity));
                int existingEntityIndex = PlayerAtGameEntities.FindIndex(o => o.RowKey == entity.RowKey);
				if (existingEntityIndex != -1)
				{
					// Replace
					PlayerAtGameEntities[existingEntityIndex] = convertedEntity;
				}
				else 
				{
					// Insert
					PlayerAtGameEntities.Add(convertedEntity);
				}
            }
            else
            {
                await RefreshEntitiesFromApiAsync<T>();
            }

            // Return
            return entity;
		}

		public async Task<List<T>> QueryEntitiesAsync<T>() where T : class, ITableEntity
		{
			var data = await _localStorageService.GetItemAsync<List<T>>(typeof(T).Name);
			if (data is not null)
			{
				return data;
			}

			return [];
		}

		public async Task LoadDataAsync(bool forceApi, bool forceLocalOnly)
		{
			if (forceLocalOnly)
			{
				await LoadDataFromLocalStorageAsync();
				return;
			}

			if (!forceApi)
			{
				// Check Data Details, set lists from local storage and exit if they are up to date
				var apiDdes = await _apiService.ApiGetAsync<DataDetailsEntity>();
				var localDdes = await _localStorageService.GetItemAsync<List<DataDetailsEntity>>(nameof(DataDetailsEntity));
				if (apiDdes?.Count > 0 && localDdes?.Count > 0)
				{
					if (apiDdes.First().LastWriteEpoch == localDdes.First()?.LastWriteEpoch)
					{
						await LoadDataFromLocalStorageAsync();
						return;
					}
				}
			}

			// Refresh caches
			var playerTask = RefreshEntitiesFromApiAsync<PlayerEntity>();
			var gameTask = RefreshEntitiesFromApiAsync<GameEntity>();
			var transactionTask = RefreshEntitiesFromApiAsync<TransactionEntity>();
			var playerAtGameTask = RefreshEntitiesFromApiAsync<PlayerAtGameEntity>();
			var ratingTask = RefreshEntitiesFromApiAsync<RatingEntity>();
			var dataDetailsTask = RefreshEntitiesFromApiAsync<DataDetailsEntity>();
			await Task.WhenAll(playerTask, gameTask, transactionTask, playerAtGameTask, ratingTask, dataDetailsTask);
		}

		private async Task LoadDataFromLocalStorageAsync()
		{
			PlayerEntities = await _localStorageService.GetItemAsync<List<PlayerEntity>>(nameof(PlayerEntity)) ?? [];
			TransactionEntities = await _localStorageService.GetItemAsync<List<TransactionEntity>>(nameof(TransactionEntity)) ?? [];
			RatingEntities = await _localStorageService.GetItemAsync<List<RatingEntity>>(nameof(RatingEntity)) ?? [];
			GameEntities = await _localStorageService.GetItemAsync<List<GameEntity>>(nameof(GameEntity)) ?? [];
			PlayerAtGameEntities = await _localStorageService.GetItemAsync<List<PlayerAtGameEntity>>(nameof(PlayerAtGameEntity)) ?? [];
		}

		private async Task RefreshEntitiesFromApiAsync<T>() where T : class, ITableEntity
		{
			var data = await _apiService.ApiGetAsync<T>();
            if (data is not null)
			{
                await _localStorageService.SetItemAsync(typeof(T).Name, data);

				if (typeof(T) == typeof(PlayerEntity))
				{
					PlayerEntities = (List<PlayerEntity>)Convert.ChangeType(data, typeof(List<T>));
				}

				if (typeof(T) == typeof(TransactionEntity))
				{
					TransactionEntities = (List<TransactionEntity>)Convert.ChangeType(data, typeof(List<T>));
				}

				if (typeof(T) == typeof(RatingEntity))
				{
					RatingEntities = (List<RatingEntity>)Convert.ChangeType(data, typeof(List<T>));
				}

				if (typeof(T) == typeof(GameEntity))
				{
					GameEntities = (List<GameEntity>)Convert.ChangeType(data, typeof(List<T>));
				}

				if (typeof(T) == typeof(PlayerAtGameEntity))
				{
					PlayerAtGameEntities = (List<PlayerAtGameEntity>)Convert.ChangeType(data, typeof(List<T>));
				}
			}
		}

	}
}