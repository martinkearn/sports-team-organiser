using Azure.Data.Tables;

namespace STO.Wasm.Interfaces
{
    /// <summary>
    /// Service for working with Azure Storage.
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// Performs a GET request and returns a list of entities of type T
        /// </summary>
        public Task<List<T>> ApiGetAsync<T>() where T : class, ITableEntity;

        /// <summary>
        /// POSTs an entity of type T
        /// </summary>
        public Task ApiPostAsync<T>(T entity) where T : class, ITableEntity;

        /// <summary>
        /// DELETEs an entity base don rowkey
        /// </summary>
        public Task ApiDeleteAsync<T>(string rowKey) where T : class, ITableEntity;
    }
}