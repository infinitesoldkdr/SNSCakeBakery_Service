using Microsoft.AspNetCore.Http; 

public interface IImageService
{
    Task<string> UploadImageAsync(IFormFile file, string folder);
    Task DeleteImageAsync(string storageKey);
}