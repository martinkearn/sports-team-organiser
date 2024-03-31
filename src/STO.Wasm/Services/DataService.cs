using Azure.Data.Tables;

namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class DataService(IApiService apiService) : IDataService
	{
		private List<PlayerEntity> _playerEntities = [];
		private List<GameEntity> _gameEntities = [];
		private List<TransactionEntity> _transactionEntities = [];
		private List<PlayerAtGameEntity> _playerAtGameEntities = [];
		private List<RatingEntity> _ratingEntities = [];
		private bool _gotData = false;

		private readonly IApiService _apiService = apiService;

		public async Task DeleteEntity<T>(string rowKey) where T : class, ITableEntity
		{
			if (!_gotData) await LoadData();

			// Delete Entity
			await _apiService.ApiDelete<T>(rowKey);

			// Refresh data from storage
			await RefreshEntitiesFromApi<T>();
		}

		public async Task<T> UpsertEntity<T>(T entity) where T : class, ITableEntity
		{
			if (!_gotData) await LoadData();

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
			if (!_gotData) await LoadData();

			var ty = typeof(T);
			if (ty == typeof(PlayerEntity))
			{
				return (List<T>)Convert.ChangeType(_playerEntities, typeof(List<T>));
			}
			else if (ty == typeof(GameEntity))
			{
				return (List<T>)Convert.ChangeType(_gameEntities, typeof(List<T>));
			}
			else if (ty == typeof(TransactionEntity))
			{
				return (List<T>)Convert.ChangeType(_transactionEntities, typeof(List<T>));
			}
			else if (ty == typeof(PlayerAtGameEntity))
			{
				return (List<T>)Convert.ChangeType(_playerAtGameEntities, typeof(List<T>));
			}
			else if (ty == typeof(RatingEntity))
			{
				return (List<T>)Convert.ChangeType(_ratingEntities, typeof(List<T>));
			}
			else
			{
				return [];
			}
		}

		public async Task LoadData()
		{
			// Refresh caches
			var playerTask = RefreshEntitiesFromApi<PlayerEntity>();
			var gameTask = RefreshEntitiesFromApi<GameEntity>();
			var transactionTask = RefreshEntitiesFromApi<TransactionEntity>();
			var playerAtGameTask = RefreshEntitiesFromApi<PlayerAtGameEntity>();
			var ratingTask = RefreshEntitiesFromApi<RatingEntity>();

			await Task.WhenAll(playerTask, gameTask, transactionTask, playerAtGameTask, ratingTask);

			_gotData = true;
		}

		private async Task RefreshEntitiesFromApi<T>() where T : class, ITableEntity
		{
			var ty = typeof(T);

			if (ty == typeof(PlayerEntity))
			{
				_playerEntities = await _apiService.ApiGet<PlayerEntity>();
			}

			if (ty == typeof(GameEntity))
			{
				_gameEntities = await _apiService.ApiGet<GameEntity>();
			}

			if (ty == typeof(TransactionEntity))
			{
				_transactionEntities = await _apiService.ApiGet<TransactionEntity>();
			}

			if (ty == typeof(PlayerAtGameEntity))
			{
				_playerAtGameEntities = await _apiService.ApiGet<PlayerAtGameEntity>();
			}

			if (ty == typeof(RatingEntity))
			{
				_ratingEntities = await _apiService.ApiGet<RatingEntity>();
			}
		}

	}
}