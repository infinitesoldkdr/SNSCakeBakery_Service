using System;

namespace SNSCakeBakery_Service.Models
{
    public class Order
    {
        public string Id { get; set; } // Primary key
        public string UserId { get; set; } // Foreign key to User
        public string CakeType { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public string Source { get; set; } // "Manual" or "Google"
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime DeliveryDate { get; set; } = DateTime.UtcNow;
        public int DeliveryAddressID { get; set; }
        public bool DeliveryRequired { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        // Navigation property
        public User User { get; set; }
    }
}
