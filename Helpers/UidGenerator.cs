namespace SNSCakeBakery_Service.Helpers
{
    public static class UidGenerator
    {
        public static string GenerateUniqueId(string prefix)
        {
            string timestamp = DateTime.UtcNow.ToString("yyMMddmmss");
            return prefix + timestamp;
        }
    }
}
