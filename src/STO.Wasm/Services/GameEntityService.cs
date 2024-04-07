
namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class GameEntityService(ICachedDataService dataService) : IGameEntityService
	{
		private readonly ICachedDataService _dataService = dataService;
		public List<PlayerAtGame> CalculateTeams(List<PlayerAtGame> pags)
		{
			throw new NotImplementedException();
		}

		public void DeleteGame(string gameRowkey)
		{
			throw new NotImplementedException();
		}

		public void DeletePlayerAtGameEntity(PlayerAtGameEntity pag)
		{
			throw new NotImplementedException();
		}

		public GameEntity GetGame(string gameRowKey)
		{
			throw new NotImplementedException();
		}

		public List<GameEntity> GetGames()
		{
			throw new NotImplementedException();
		}

		public GameEntity GetNextGame()
		{
			throw new NotImplementedException();
		}

		public string GetNotesForGame(string gameTitle)
		{
			throw new NotImplementedException();
		}

		public PlayerAtGame GetPlayerAtGame(string pagRowKey)
		{
			throw new NotImplementedException();
		}

		public void MarkAllPlayed(string gameRowkey, bool played)
		{
			throw new NotImplementedException();
		}

		public void TogglePlayerAtGamePlayed(PlayerAtGameEntity pag, bool? played)
		{
			throw new NotImplementedException();
		}

		public void UpsertGameEntity(GameEntity gameEntity)
		{
			throw new NotImplementedException();
		}

		public void UpsertPlayerAtGameEntity(PlayerAtGameEntity pag)
		{
			throw new NotImplementedException();
		}
	}
}
