using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using SaCodeWhite.Functions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Functions.Repositories
{
    public class TableRepository<T> : ITableRepository<T> where T : class, ITableEntity, new()
    {
        private const int MaxBatchCount = 100;

        private readonly TableClient _tableClient;

        public TableRepository(TableStorageOptions options)
        {
            if (string.IsNullOrEmpty(options?.AzureWebJobsStorage))
                throw new ArgumentException(nameof(TableStorageOptions.AzureWebJobsStorage));

            _tableClient = new TableClient(options.AzureWebJobsStorage, typeof(T).Name);
        }

        public Task CreateIfNotExistsAsync(CancellationToken cancellationToken)
        {
            return _tableClient.CreateIfNotExistsAsync(cancellationToken);
        }

        public Task<IList<T>> GetPartitionAsync(string partitionKey, CancellationToken cancellationToken)
        {
            var query = _tableClient.QueryAsync<T>(i => i.PartitionKey == partitionKey);
            return ExecuteQueryAsync(query, cancellationToken);
        }

        public Task<IList<T>> GetPartitionsAsync(IList<string> partitionKeys, CancellationToken cancellationToken)
        {
            if (!partitionKeys.Any())
                return Task.FromResult<IList<T>>(Array.Empty<T>());

            var sb = new StringBuilder();
            for (var i = 0; i < partitionKeys.Count; i++)
            {
                var pk = partitionKeys[i];

                sb.AppendFormat("(PartitionKey eq '{0}')", pk);

                if (i < partitionKeys.Count - 1)
                    sb.AppendFormat(" or ");
            }

            var query = _tableClient.QueryAsync<T>(filter: sb.ToString());
            return ExecuteQueryAsync(query, cancellationToken);
        }

        public async Task<T> GetEntityAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            var resp = await _tableClient.GetEntityAsync<T>(partitionKey, rowKey).ConfigureAwait(false);
            return resp.Value;
        }

        public Task<IList<T>> GetEntitiesAsync(IList<(string partitionKey, string rowKey)> keys, CancellationToken cancellationToken)
        {
            if (!keys.Any())
                return Task.FromResult<IList<T>>(Array.Empty<T>());

            var sb = new StringBuilder();
            for (var i = 0; i < keys.Count; i++)
            {
                var k = keys[i];

                sb.Append("(");
                sb.AppendFormat("(PartitionKey eq '{0}')", k.partitionKey);
                sb.Append(" and ");
                sb.AppendFormat("(RowKey eq '{0}')", k.rowKey);
                sb.Append(")");

                if (i < keys.Count - 1)
                    sb.AppendFormat(" or ");
            }

            var query = _tableClient.QueryAsync<T>(filter: sb.ToString());
            return ExecuteQueryAsync(query, cancellationToken);
        }

        public Task<IList<T>> GetAllEntitiesAsync(CancellationToken cancellationToken)
        {
            var query = _tableClient.QueryAsync<T>();
            return ExecuteQueryAsync(query, cancellationToken);
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            return _tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey, cancellationToken: cancellationToken);
        }

        public Task InsertOrReplaceAsync(T entity, CancellationToken cancellationToken)
        {
            return _tableClient.UpsertEntityAsync(entity, cancellationToken: cancellationToken);
        }

        public Task DeleteBulkAsync(IEnumerable<T> entities, ILogger logger, CancellationToken cancellationToken)
            => BatchActionAsync(entities, TableTransactionActionType.Delete, logger, cancellationToken);

        public Task InsertOrReplaceBulkAsync(IEnumerable<T> entities, ILogger logger, CancellationToken cancellationToken)
            => BatchActionAsync(entities, TableTransactionActionType.UpsertReplace, logger, cancellationToken);

        public async Task<IList<T>> ExecuteQueryAsync(AsyncPageable<T> asyncQuery, CancellationToken cancellationToken)
        {
            var results = new List<T>();

            await foreach (var page in asyncQuery.AsPages().ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                results.AddRange(page.Values);
            }

            return results;
        }

        private async Task BatchActionAsync(IEnumerable<T> entities, TableTransactionActionType actionType, ILogger logger, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Creating batch actions for '{_tableClient.Name}'");

            var batchActions = entities
                .GroupBy(a => a.PartitionKey)
                .ToDictionary(g => g.Key, g => g.Select(i => new TableTransactionAction(actionType, i)).ToList());

            var batchCount = 1;
            foreach (var ba in batchActions)
            {
                logger.LogInformation($"Batch {batchCount++} of {batchActions.Count} for '{_tableClient.Name}'");
                for (var i = 0; i < ba.Value.Count; i += MaxBatchCount)
                {
                    var batch = ba.Value.Skip(i).Take(MaxBatchCount);
                    var result = await _tableClient.SubmitTransactionAsync(batch, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
