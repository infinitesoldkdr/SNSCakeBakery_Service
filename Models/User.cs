using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SNSCakeBakery_Service.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } 

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(128)] 
        public string FirebaseUid { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}