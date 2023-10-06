
namespace STO.Services
{
    /// <inheritdoc/>
    public class RatingService : IRatingService
    {
        private readonly IStorageService _storageService;
        private readonly IPlayerService _playerService;
        private readonly IGameService _gameService;
        public RatingService(IStorageService storageService, IPlayerService playerService, IGameService gameService)
        {
            _storageService = storageService;
            _playerService = playerService;
            _gameService = gameService;
        }

        public async Task<List<Rating>> GetRatings()
        {
            var entities = _storageService.QueryEntities<RatingEntity>().ToList();
            var ratings = await RatingEntitiesToRatings(entities);
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

        public async Task DeleteRatingEntity(string rowKey)
        {
            await _storageService.DeleteEntity<RatingEntity>(rowKey);
        }

        public async Task UpsertRatingEntity(RatingEntity ratingEntity)
        {
            _ = await _storageService.UpsertEntity<RatingEntity>(ratingEntity);
        }

        private async Task<List<Rating>> RatingEntitiesToRatings(List<RatingEntity> ratingEntities)
        {
            var ratings = new List<Rating>();
            foreach (var re in ratingEntities)
            {
                var player = _playerService.GetPlayer(re.PlayerRowKey);
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
    }
}