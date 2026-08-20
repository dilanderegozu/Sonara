using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Sonara.CoreLayer;
using Azure.Storage.Blobs.Models;
namespace Sonara.WebApi.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;

        public BlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException("Azure Storage connection string not found.");
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName, string? contentType = null)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";
            var blobClient = containerClient.GetBlobClient(uniqueFileName);

            var headers = new BlobHttpHeaders
            {
                ContentType = contentType ?? "application/octet-stream"
            };

            await blobClient.UploadAsync(fileStream, new BlobUploadOptions { HttpHeaders = headers });

            return blobClient.Uri.ToString();
        }
    }
}