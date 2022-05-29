using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Functions.Repositories
{
    public interface ITableRepository<T> where T : ITableEntity, new()
    {
        Task CreateIfNotExistsAsync(CancellationToken cancellationToken);

        Task<IList<T>> GetPartitionAsync(string partitionKey, CancellationToken cancellationToken);
        Task<IList<T>> GetPartitionsAsync(IList<string> partitionKeys, CancellationToken cancellationToken);

        Task<T> GetEntityAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);
        Task<IList<T>> GetEntitiesAsync(IList<(string partitionKey, string rowKey)> keys, CancellationToken cancellationToken);
        Task<IList<T>> GetAllEntitiesAsync(CancellationToken cancellationToken);

        Task DeleteAsync(T entity, CancellationToken cancellationToken);
        Task InsertOrReplaceAsync(T entity, CancellationToken cancellationToken);

        Task DeleteBulkAsync(IEnumerable<T> entities, ILogger logger, CancellationToken cancellationToken);
        Task InsertOrReplaceBulkAsync(IEnumerable<T> entities, ILogger logger, CancellationToken cancellationToken);

        Task<IList<T>> ExecuteQueryAsync(AsyncPageable<T> query, CancellationToken cancellationToken);
    }
}
