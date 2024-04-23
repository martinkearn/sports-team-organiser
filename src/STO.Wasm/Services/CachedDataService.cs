using Azure.Data.Tables;

namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class CachedDataService(IApiService apiService, ILocalStorageService localStorageService) : ICachedDataService
	{
		public List<PlayerEntity> PlayerEntities { get; set; } = [];
		public List<TransactionEntity> TransactionEntities { get; set; } = [];
		public List<RatingEntity> RatingEntities { get; set; } = [];
		public List<GameEntity> GameEntities { get; set; } = [];
		public List<PlayerAtGameEntity> PlayerAtGameEntities { get; set; } = [];

		public async Task DeleteEntityAsync<T>(string rowKey) where T : class, ITableEntity
		{
			// Delete entity in Api
			await apiService.ApiDeleteAsync<T>(rowKey);

			// Refresh data from storage
			await RefreshEntitiesFromApiAsync<T>();
		}

		public async Task<T> UpsertEntityAsync<T>(T entity) where T : class, ITableEntity
		{
			// Complete required values
			entity.RowKey ??= Guid.NewGuid().ToString();
			entity.PartitionKey ??= typeof(T).ToString();

            // Upsert entity to Api
            await apiService.ApiPostAsync(entity);

            // Upsert entity to local store
            if (typeof(T) == typeof(PlayerEntity))
            {
                var convertedEntity = (PlayerEntity)Convert.ChangeType(entity, typeof(PlayerEntity));
                int existingEntityIndex = PlayerEntities.FindIndex(o => o.RowKey == entity.RowKey);
                if (existingEntityIndex != -1)
                {
                    // Replace
                    PlayerEntities[existingEntityIndex] = convertedEntity;
                }
                else
                {
                    // Insert
                    PlayerEntities.Add(convertedEntity);
                }
            }

            if (typeof(T) == typeof(TransactionEntity))
            {
                var convertedEntity = (TransactionEntity)Convert.ChangeType(entity, typeof(TransactionEntity));
                var existingEntityIndex = TransactionEntities.FindIndex(o => o.RowKey == entity.RowKey);
                if (existingEntityIndex != -1)
                {
                    // Replace
                    TransactionEntities[existingEntityIndex] = convertedEntity;
                }
                else
                {
                    // Insert
                    TransactionEntities.Add(convertedEntity);
                }
            }

            if (typeof(T) == typeof(RatingEntity))
            {
                var convertedEntity = (RatingEntity)Convert.ChangeType(entity, typeof(RatingEntity));
                var existingEntityIndex = RatingEntities.FindIndex(o => o.RowKey == entity.RowKey);
                if (existingEntityIndex != -1)
                {
                    // Replace
                    RatingEntities[existingEntityIndex] = convertedEntity;
                }
                else
                {
                    // Insert
                    RatingEntities.Add(convertedEntity);
                }
            }

            if (typeof(T) == typeof(GameEntity))
            {
				var listCopy = GameEntities;
                var convertedEntity = (GameEntity)Convert.ChangeType(entity, typeof(GameEntity));
                var existingEntityIndex = listCopy.FindIndex(o => o.RowKey == entity.RowKey);
                if (existingEntityIndex != -1)
                {
                    // Replace
                    listCopy[existingEntityIndex] = convertedEntity;
                }
                else
                {
                    // Insert
                    listCopy.Add(convertedEntity);
                }
				GameEntities = listCopy;
            }

            if (typeof(T) == typeof(PlayerAtGameEntity))
            {
				var convertedEntity = (PlayerAtGameEntity)Convert.ChangeType(entity, typeof(PlayerAtGameEntity));
                var existingEntityIndex = PlayerAtGameEntities.FindIndex(o => o.RowKey == entity.RowKey);
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

            // Return
            return entity;
		}

		public async Task<List<T>> QueryEntitiesAsync<T>() where T : class, ITableEntity
		{
			var data = await localStorageService.GetItemAsync<List<T>>(typeof(T).Name);
			return data ?? [];
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
				// Check Data Details, set lists from local storage and exit if they are up-to-date
				var apiDdes = await apiService.ApiGetAsync<DataDetailsEntity>();
				var localDdes = await localStorageService.GetItemAsync<List<DataDetailsEntity>>(nameof(DataDetailsEntity));
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

		public async Task ClearLocalDataAsync()
		{
			await localStorageService.ClearAsync();
		}

		private async Task LoadDataFromLocalStorageAsync()
		{
			PlayerEntities = await localStorageService.GetItemAsync<List<PlayerEntity>>(nameof(PlayerEntity)) ?? [];
			TransactionEntities = await localStorageService.GetItemAsync<List<TransactionEntity>>(nameof(TransactionEntity)) ?? [];
			RatingEntities = await localStorageService.GetItemAsync<List<RatingEntity>>(nameof(RatingEntity)) ?? [];
			GameEntities = await localStorageService.GetItemAsync<List<GameEntity>>(nameof(GameEntity)) ?? [];
			PlayerAtGameEntities = await localStorageService.GetItemAsync<List<PlayerAtGameEntity>>(nameof(PlayerAtGameEntity)) ?? [];
		}

		private async Task RefreshEntitiesFromApiAsync<T>() where T : class, ITableEntity
		{
			var data = await apiService.ApiGetAsync<T>();
            if (data is not null)
			{
                await localStorageService.SetItemAsync(typeof(T).Name, data);

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