public interface IImageService
{
    //Returns the URL of the uploaded image
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteImageAsync(string fileName);
}