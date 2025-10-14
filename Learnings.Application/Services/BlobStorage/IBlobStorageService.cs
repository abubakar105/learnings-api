
using Microsoft.AspNetCore.Http;

namespace Learnings.Application.Services.Interface
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string? containerName = null);
        Task<string> UploadFileAsync(IFormFile file, string fileName, string? containerName = null);
        Task<bool> DeleteFileAsync(string fileUrl, string? containerName = null);
        Task<Stream> GetFileAsync(string fileName, string? containerName = null);
    }
}
