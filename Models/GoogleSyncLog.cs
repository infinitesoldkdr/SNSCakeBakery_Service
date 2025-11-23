using System;

namespace SNSCakeBakery_Service.Models
{
    public class GoogleSyncLog
    {
        public int Id { get; set; }
        public DateTime LastSynced { get; set; } = DateTime.UtcNow;
        public int TotalImported { get; set; }
        public string Status { get; set; } // e.g., "Success", "Failed"
    }
}
