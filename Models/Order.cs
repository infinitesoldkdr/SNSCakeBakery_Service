using System;

namespace SNSCakeBakery_Service.Models
{
    public class Order
    {
        public int Id { get; set; } // Primary key
        public int UserId { get; set; } // Foreign key to User
        public string CakeType { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public string Source { get; set; } // "Manual" or "Google"
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User User { get; set; }
    }
}
