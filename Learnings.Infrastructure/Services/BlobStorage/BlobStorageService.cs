
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Learnings.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Learnings.Infrastructure.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _defaultContainerName;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(
            IConfiguration configuration,
            ILogger<BlobStorageService> logger)
        {
            var connectionString = configuration.GetConnectionString("BlobStorage")
                ?? configuration["BlobStorage:ConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Blob Storage connection string not configured");
            }

            _blobServiceClient = new BlobServiceClient(connectionString);
            _defaultContainerName = configuration["BlobStorage:ContainerName"] ?? "product-images";
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string? containerName = null)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null");
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            return await UploadFileAsync(file, fileName, containerName);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName, string? containerName = null)
        {
            try
            {
                _logger.LogInformation("Uploading file {FileName} to blob storage", fileName);

                ValidateFile(file);

                var container = containerName ?? _defaultContainerName;
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);

                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobClient = containerClient.GetBlobClient(fileName);

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = GetContentType(file.FileName)
                };

                // Upload file
                await using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });

                _logger.LogInformation("File {FileName} uploaded successfully. URL: {Url}",
                    fileName, blobClient.Uri.AbsoluteUri);

                // Return public URL
                return blobClient.Uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName}", fileName);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string fileUrl, string? containerName = null)
        {
            try
            {
                _logger.LogInformation("Deleting file from URL: {FileUrl}", fileUrl);

                var uri = new Uri(fileUrl);
                var fileName = Path.GetFileName(uri.LocalPath);

                var container = containerName ?? _defaultContainerName;
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);

                var blobClient = containerClient.GetBlobClient(fileName);

                var response = await blobClient.DeleteIfExistsAsync();

                _logger.LogInformation("File {FileName} deleted: {Success}", fileName, response.Value);

                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from URL: {FileUrl}", fileUrl);
                return false;
            }
        }

        public async Task<Stream> GetFileAsync(string fileName, string? containerName = null)
        {
            try
            {
                var container = containerName ?? _defaultContainerName;
                var containerClient = _blobServiceClient.GetBlobContainerClient(container);
                var blobClient = containerClient.GetBlobClient(fileName);

                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file {FileName}", fileName);
                throw;
            }
        }

        #region Private Helper Methods

        private void ValidateFile(IFormFile file)
        {
            // Max file size: 10MB
            const long maxFileSize = 10 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                throw new ArgumentException($"File size exceeds maximum allowed size of {maxFileSize / 1024 / 1024}MB");
            }

            // Allowed extensions
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".doc", ".docx" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"File type {extension} is not allowed");
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }

        #endregion
    }
}
