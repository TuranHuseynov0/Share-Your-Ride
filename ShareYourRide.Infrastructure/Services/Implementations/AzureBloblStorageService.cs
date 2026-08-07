using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ShareYourRide.Infrastructure.Services.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Services.Implementations
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlobStorage:ConnectionString"]!;
            var containerName = configuration["AzureBlobStorage:ContainerName"]!;

            var serviceClient = new BlobServiceClient(connectionString);
            _containerClient = serviceClient.GetBlobContainerClient(containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file.Length == 0)
                throw new InvalidOperationException("Fayl boşdur.");

            if (file.Length > MaxFileSize)
                throw new InvalidOperationException("Fayl ölçüsü 5 MB-dan çox ola bilməz.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("Yalnız jpg, jpeg, png, webp formatlarına icazə verilir.");

            var blobName = $"{folder}/{Guid.NewGuid()}{extension}";
            var blobClient = _containerClient.GetBlobClient(blobName);

            var contentType = extension switch
            {
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });

            return blobClient.Uri.ToString();
        }
    }
}