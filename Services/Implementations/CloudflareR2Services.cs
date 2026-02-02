using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using SNSCakeBakery_Service.Configuration;

namespace SNSCakeBakery_Service.Services
{
    public class CloudflareR2Service : IImageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly CloudflareOptions _options;

        public CloudflareR2Service(IOptions<CloudflareOptions> options)
        {
            // The .Value property contains our strongly-typed settings
            _options = options.Value;

            var s3Config = new AmazonS3Config
            {
                ServiceURL = $"https://{_options.AccountId}.r2.cloudflarestorage.com",
                AuthenticationRegion = "auto"
            };

            _s3Client = new AmazonS3Client(_options.AccessKey, _options.SecretKey, s3Config);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            // Generate a unique key for the file to prevent overwriting
            var fileKey = $"{folder}/{Guid.NewGuid()}_{file.FileName}";

            using (var stream = file.OpenReadStream())
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = fileKey,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    
                    // CRITICAL: This bypasses the STREAMING-AWS4-HMAC-SHA256 error 
                    // by forcing a standard header-based signature.
                    DisablePayloadSigning = true,
                    
                    // Prevents R2 from rejecting the request due to modern S3 checksum trailers
                    DisableDefaultChecksumValidation = true
                };

                await _s3Client.PutObjectAsync(putRequest);
            }

            return fileKey;
        }

        public async Task DeleteImageAsync(string storageKey)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = storageKey
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }
    }
}