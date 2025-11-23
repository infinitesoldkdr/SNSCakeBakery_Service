using System;
using System.Collections.Generic;

namespace SNSCakeBakery_Service.Models
{
    public class User
    {
        public int Id { get; set; } // Primary key
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Order> Orders { get; set; }
    }
}
