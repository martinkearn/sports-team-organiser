namespace STO.Wasm.Services
{
    /// <inheritdoc/>
    public class RatingService(IDataService dataService, IPlayerService playerService, IGameService gameService) : IRatingService
    {
        private readonly IDataService _dataService = dataService;
        private readonly IPlayerService _playerService = playerService;
        private readonly IGameService _gameService = gameService;

        public async Task<List<Rating>> GetRatings()
        {
            var entitiesResult = await _dataService.QueryEntities<RatingEntity>();
            var ratings = await RatingEntitiesToRatings(entitiesResult);
            return ratings;
        }

        public async Task<Rating> GetRating(string rowKey)
        {
            var ratings = await GetRatings();
            var rating = ratings.Single(o => o.RatingEntity.RowKey == rowKey);
            return rating;
        }

        public async Task<List<Rating>> GetRatingsForPlayer(string playerRowKey)
        {
            var ratings = await GetRatings();
            var ratingsForPlayer = ratings.Where(o => o.Player.PlayerEntity.RowKey == playerRowKey).ToList();
            return ratingsForPlayer;
        }

        public async Task<List<Rating>> GetRatingsForGame(string gameRowKey)
        {
            var ratings = await GetRatings();
            var ratingsForGame = ratings.Where(o => o.Game.GameEntity.RowKey == gameRowKey).ToList();
            return ratingsForGame;
        }

        public async Task DeleteRatingEntity(string rowKey)
        {
            await _dataService.DeleteEntity<RatingEntity>(rowKey);
        }

        public async Task UpsertRatingEntity(RatingEntity ratingEntity)
        {
            _ = await _dataService.UpsertEntity<RatingEntity>(ratingEntity);
        }

        private async Task<List<Rating>> RatingEntitiesToRatings(List<RatingEntity> ratingEntities)
        {
            var ratings = new List<Rating>();
            foreach (var re in ratingEntities)
            {
                var player = await _playerService.GetPlayer(re.PlayerRowKey);
                var game = await _gameService.GetGame(re.GameRowKey);

                var rating = new Rating(re)
                {
                    Player = player,
                    Game = game
                };
                ratings.Add(rating);
            }
            return ratings.OrderByDescending(o => o.RatingEntity.Timestamp).ToList();
        }

        public string FormatRatingTime(RatingEntity ratingEntity)
        {
            if (ratingEntity is not null)
            {
                var timestamp = ratingEntity.Timestamp;
                if (timestamp is not null)
                {
                    return timestamp.Value.ToString("dd MMM yyyy HH:mm") ?? "N/A";
                }
            }

            return string.Empty;
        }
    }
}