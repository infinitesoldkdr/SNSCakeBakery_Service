using System;
using System.Collections.Generic;
using SNSCakeBakery_Service.DTOs.Orders;

namespace SNSCakeBakery_Service.DTOs.Users
{
    // ---------------------------------------------
    // Create User DTO
    // ---------------------------------------------
    public class CreateUserDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
    }

// ---------------------------------------------
// Update User DTO
// ---------------------------------------------
public class UpdateUserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; } // Optional, only update if provided
    }

    // ---------------------------------------------
    // User Response DTO (sent back to client)
    // ---------------------------------------------
    public class UserResponseDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; }

        // Orders associated with the user
        public ICollection<OrderResponseDto> Orders { get; set; }
    }

}
