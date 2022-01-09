using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Functions.Repositories
{
    public interface ITableRepository<T> where T : TableEntity, new()
    {
        Task CreateIfNotExistsAsync(CancellationToken cancellationToken);

        Task<IList<T>> GetPartitionAsync(string partitionKey, CancellationToken cancellationToken);
        Task<IList<T>> GetPartitionsAsync(IEnumerable<string> partitionKeys, CancellationToken cancellationToken);

        Task<T> GetEntityAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);
        Task<IList<T>> GetAllEntitiesAsync(CancellationToken cancellationToken);

        Task DeleteAsync(T entity, CancellationToken cancellationToken);

        Task InsertOrReplaceAsync(T entity, CancellationToken cancellationToken);

        Task DeleteBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

        Task InsertOrReplaceBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken);

        Task<IList<T>> ExecuteQueryAsync(TableQuery<T> query, CancellationToken cancellationToken);
    }
}
