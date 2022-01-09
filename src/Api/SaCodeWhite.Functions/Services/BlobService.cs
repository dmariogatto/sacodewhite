using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Newtonsoft.Json;
using SaCodeWhite.Functions.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaCodeWhite.Functions.Services
{
    public class BlobService : IBlobService
    {
        private const string ContainerName = "sacodewhite";

        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<IList<BlobFile>> GetBlobFilesAsync(CancellationToken cancellationToken)
        {
            var result = new List<BlobFile>();

            var container = await GetBlobContainerAsync(cancellationToken).ConfigureAwait(false);

            // Call the listing operation and return pages of the specified size.
            var resultSegment = container.GetBlobsAsync().AsPages();

            // Enumerate the blobs returned for each page.
            await foreach (Azure.Page<BlobItem> blobPage in resultSegment)
            {
                foreach (var blob in blobPage.Values.Where(i => !i.Deleted && i.Properties.LastModified.HasValue))
                {
                    result.Add(new BlobFile()
                    {
                        Name = blob.Name,
                        ModifiedUtc = blob.Properties.LastModified.Value.UtcDateTime
                    });
                }
            }

            return result;
        }

        public async Task SerialiseAsync<T>(T data, string localFilePath, CancellationToken cancellationToken)
        {
            var container = await GetBlobContainerAsync(cancellationToken).ConfigureAwait(false);
            var blob = container.GetBlockBlobClient(localFilePath);

            using var writer = new StreamWriter(await blob.OpenWriteAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false));
            using var jw = new JsonTextWriter(writer);
            JsonSerializer.CreateDefault().Serialize(jw, data);
        }

        public async Task<T> DeserialiseAsync<T>(string localFilePath, CancellationToken cancellationToken)
        {
            var result = default(T);

            var container = await GetBlobContainerAsync(cancellationToken).ConfigureAwait(false);
            var blob = container.GetBlobClient(localFilePath);

            if (await blob.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                using var reader = new StreamReader(await blob.OpenReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false));
                using var jr = new JsonTextReader(reader);
                result = JsonSerializer.CreateDefault().Deserialize<T>(jr);
            }

            return result;
        }

        public async Task<Stream> OpenReadAsync(string localFilePath, CancellationToken cancellationToken)
        {
            var container = await GetBlobContainerAsync(cancellationToken).ConfigureAwait(false);
            var blob = container.GetBlobClient(localFilePath);

            if (await blob.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                return await blob.OpenReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            return Stream.Null;
        }

        public async Task<Stream> OpenWriteAsync(string localFilePath, CancellationToken cancellationToken)
        {
            var container = await GetBlobContainerAsync(cancellationToken).ConfigureAwait(false);
            var blob = container.GetBlockBlobClient(localFilePath);
            return await blob.OpenWriteAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task WriteAllTextAsync(string data, string localFilePath, CancellationToken cancellationToken)
        {
            var container = await GetBlobContainerAsync(cancellationToken).ConfigureAwait(false);
            var blob = container.GetBlockBlobClient(localFilePath);

            using var writer = new StreamWriter(await blob.OpenWriteAsync(true, cancellationToken: cancellationToken).ConfigureAwait(false));
            await writer.WriteAsync(data).ConfigureAwait(false);
        }

        public async Task<string> ReadAllTextAsync(string localFilePath, CancellationToken cancellationToken)
        {
            var result = string.Empty;

            var container = await GetBlobContainerAsync(cancellationToken).ConfigureAwait(false);
            var blob = container.GetBlobClient(localFilePath);

            if (await blob.ExistsAsync(cancellationToken).ConfigureAwait(false))
            {
                using var reader = new StreamReader(await blob.OpenReadAsync(cancellationToken: cancellationToken).ConfigureAwait(false));
                result = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            return result;
        }

        public async Task<bool> ExistsAsync(string localFilePath, CancellationToken cancellationToken)
        {
            var container = await GetBlobContainerAsync(cancellationToken).ConfigureAwait(false);
            var blob = container.GetBlobClient(localFilePath);
            return await blob.ExistsAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string localFilePath, CancellationToken cancellationToken)
        {
            var container = await GetBlobContainerAsync(cancellationToken).ConfigureAwait(false);
            var blob = container.GetBlobClient(localFilePath);
            await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private async Task<BlobContainerClient> GetBlobContainerAsync(CancellationToken cancellationToken = default)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            return containerClient;
        }
    }
}
