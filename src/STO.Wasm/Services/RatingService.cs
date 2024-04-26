namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class RatingService(IDataService dataService) : IRatingService
	{
		public async Task DeleteRatingEntityAsync(string rowKey)
		{
			await dataService.DeleteEntityAsync<RatingEntity>(rowKey);
		}

		public string FormatRatingTime(string rowKey)
		{
			var re = GetRatingEntity(rowKey);
			if (re is null) return string.Empty;
			var timestamp = re.Timestamp;
			if (timestamp is not null)
			{
				return timestamp.Value.ToString("dd MMM yyyy HH:mm") ?? "N/A";
			}

			return string.Empty;
		}

		public List<RatingEntity> GetRatingEntities()
		{
			return [.. dataService.RatingEntities.OrderByDescending(o => o.Timestamp)];
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
			await dataService.UpsertEntityAsync(ratingEntity);
		}
	}
}