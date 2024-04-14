namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class RatingEntityService(ICachedDataService dataService) : IRatingEntityService
	{
		private readonly ICachedDataService _dataService = dataService;

		public async Task DeleteRatingEntityAsync(string rowKey)
		{
			await _dataService.DeleteEntity<RatingEntity>(rowKey);
		}

		public string FormatRatingTime(string rowKey)
		{
			var re = GetRatingEntity(rowKey);
			if (re is not null)
			{
				var timestamp = re.Timestamp;
				if (timestamp is not null)
				{
					return timestamp.Value.ToString("dd MMM yyyy HH:mm") ?? "N/A";
				}
			}

			return string.Empty;
		}

		public List<RatingEntity> GetRatingEntities()
		{
			return _dataService.RatingEntities;
		}

		public List<RatingEntity> GetRatingEntitiesForGame(string gameRowKey)
		{
			var res = GetRatingEntities();
			return res.Where(o => o.GameRowKey == gameRowKey).ToList();
		}

		public List<RatingEntity> GetRatingEntitiesForPlayer(string playerRowKey)
		{
			var res = GetRatingEntities();
			return res.Where(o => o.PlayerRowKey == playerRowKey).ToList();
		}

		public RatingEntity GetRatingEntity(string rowKey)
		{
			var res = GetRatingEntities();
			return res.First(o => o.RowKey == rowKey);
		}

		public async Task UpsertRatingEntityAsync(RatingEntity ratingEntity)
		{
			await _dataService.UpsertEntity(ratingEntity);
		}
	}
}