using System;

namespace SaCodeWhite.Functions.Models
{
    public class BlobStorageOptions
    {
        public string AzureWebJobsStorage { get; set; }
        public string BlobContainerName { get; set; }
    }
}
