using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Functions.Repositories
{
    public class TableRepository<T> : ITableRepository<T> where T : TableEntity, new()
    {
        private readonly CloudTable _cloudTable;

        public TableRepository(CloudTableClient tableClient)
        {
            if (tableClient == null)
                throw new ArgumentNullException(nameof(tableClient));

            var tableName = typeof(T).Name;

            _cloudTable = tableClient.GetTableReference(tableName)
                ?? throw new NullReferenceException($"Reference to table '{tableName}' cannot be null!");
        }

        public Task CreateIfNotExistsAsync(CancellationToken cancellationToken)
        {
            return _cloudTable.CreateIfNotExistsAsync(cancellationToken);
        }

        public Task<IList<T>> GetPartitionAsync(string partitionKey, CancellationToken cancellationToken)
        {
            var query = new TableQuery<T>();
            query = query.Where(
                    TableQuery.GenerateFilterCondition(
                        nameof(TableEntity.PartitionKey),
                        QueryComparisons.Equal,
                        partitionKey));
            return ExecuteQueryAsync(query, cancellationToken);
        }

        public Task<IList<T>> GetPartitionsAsync(IEnumerable<string> partitionKeys, CancellationToken cancellationToken)
        {
            var query = new TableQuery<T>();

            var combined = string.Empty;
            foreach (var pk in partitionKeys)
            {
                var predicate = TableQuery.GenerateFilterCondition(
                        nameof(TableEntity.PartitionKey),
                        QueryComparisons.Equal,
                        pk);
                combined = !string.IsNullOrEmpty(combined)
                    ? TableQuery.CombineFilters(
                        combined,
                        TableOperators.Or,
                        predicate)
                    : predicate;
            }

            query = query.Where(combined);
            return ExecuteQueryAsync(query, cancellationToken);
        }

        public async Task<T> GetEntityAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
        {
            var retrieveOp = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var result = await _cloudTable.ExecuteAsync(retrieveOp, cancellationToken)
                .ConfigureAwait(false);
            return result.Result as T;
        }

        public Task<IList<T>> GetAllEntitiesAsync(CancellationToken cancellationToken)
        {
            var query = new TableQuery<T>();
            return ExecuteQueryAsync(query, cancellationToken);
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken)
        {
            var deleteOp = TableOperation.Delete(entity);
            return _cloudTable.ExecuteAsync(deleteOp, cancellationToken);
        }

        public Task InsertOrReplaceAsync(T entity, CancellationToken cancellationToken)
        {
            var insertOrReplaceOp = TableOperation.InsertOrReplace(entity);
            return _cloudTable.ExecuteAsync(insertOrReplaceOp, cancellationToken);
        }

        public Task DeleteBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
            => BatchOpsAsync(entities, (bo, e) => bo.Delete(e), cancellationToken);

        public Task InsertOrReplaceBulkAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
            => BatchOpsAsync(entities, (bo, e) => bo.InsertOrReplace(e), cancellationToken);

        public async Task<IList<T>> ExecuteQueryAsync(TableQuery<T> query, CancellationToken cancellationToken)
        {
            var results = new List<T>();
            // Initialize continuation token to start from the beginning of the table.
            var continuationToken = default(TableContinuationToken);

            do
            {
                // Retrieve a segment (1000 entities)
                var tableQueryResult = await _cloudTable.ExecuteQuerySegmentedAsync(query, continuationToken)
                    .ConfigureAwait(false);
                // Assign the new continuation token to tell the service where to
                // continue on the next iteration (or null if it has reached the end)
                continuationToken = tableQueryResult.ContinuationToken;
                results.AddRange(tableQueryResult.Results);
            } while (continuationToken != null && (cancellationToken == default || !cancellationToken.IsCancellationRequested));

            return results;
        }

        private async Task BatchOpsAsync(IEnumerable<T> entities, Action<TableBatchOperation, T> batchOpAction, CancellationToken cancellationToken)
        {
            var batchOps = new List<TableBatchOperation>();

            foreach (var g in entities.GroupBy(a => a.PartitionKey))
            {
                var batchOp = new TableBatchOperation();
                foreach (var e in g)
                {
                    batchOpAction.Invoke(batchOp, e);

                    // Maximum operations in a batch
                    if (batchOp.Count == 100)
                    {
                        batchOps.Add(batchOp);
                        batchOp = new TableBatchOperation();
                    }
                }

                // Batch can only contain operations in the same partition
                if (batchOp.Count > 0)
                {
                    batchOps.Add(batchOp);
                }
            }

            foreach (var bo in batchOps)
            {
                var result = await _cloudTable.ExecuteBatchAsync(bo, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
