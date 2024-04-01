namespace STO.Wasm.Services
{
	/// <inheritdoc/>
	public class PlayerEntityService(ICachedDataService dataService) : IPlayerEntityService
	{
		private readonly ICachedDataService _dataService = dataService;

		public List<PlayerEntity> GetPlayerEntities()
		{
			return _dataService.PlayerEntities;
		}

		public Player GetPlayerEntity(string rowKey)
		{
			throw new NotImplementedException();
		}

		public Player GetPlayer(string rowKey)
		{
			throw new NotImplementedException();
		}

		public void DeletePlayerEntity(string playerRowkey)
		{
			throw new NotImplementedException();
		}

		public void UpsertPlayerEntity(PlayerEntity playerEntity)
		{
			throw new NotImplementedException();
		}

	}
}