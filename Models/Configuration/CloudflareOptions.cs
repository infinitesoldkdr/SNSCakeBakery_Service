namespace SNSCakeBakery_Service.Configuration
{
    public class CloudflareOptions
    {
        public const string SectionName = "CloudflareR2";

        public string AccountId { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string PublicBaseUrl { get; set; } = string.Empty;
    }
}