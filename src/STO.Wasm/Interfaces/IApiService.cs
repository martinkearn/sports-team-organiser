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
        public Task<List<T>> ApiGet<T>() where T : class, ITableEntity;

        /// <summary>
        /// POSTs an entity of type T
        /// </summary>
        public Task ApiPost<T>(T entity) where T : class, ITableEntity;

        /// <summary>
        /// DELETEs an entity base don rowkey
        /// </summary>
        public Task ApiDelete<T>(string rowKey) where T : class, ITableEntity;
    }
}