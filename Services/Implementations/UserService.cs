using Microsoft.EntityFrameworkCore;
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.DTO.Login;
using SNSCakeBakery_Service.DTO.Register;
using SNSCakeBakery_Service.DTO.Service;
using SNSCakeBakery_Service.DTO.User;
using SNSCakeBakery_Service.Helpers;
using SNSCakeBakery_Service.Models;
using SNSCakeBakery_Service.Services.Interfaces;
using SNSCakeBakery_Service.Services.Helpers;
using SNSCakeBakery_Service.DTOs.Users;
using FirebaseAdmin.Auth; // Requires FirebaseAdmin NuGet package

namespace SNSCakeBakery_Service.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        private readonly JwtTokenGenerator _jwt;

        public UserService(AppDbContext db, IConfiguration config, JwtTokenGenerator jwt)
        {
            _db = db;
            _config = config;
            _jwt = jwt;
        }

        // -------------------------------------------------------
        // Register
        // -------------------------------------------------------
        public async Task<ServiceResponse> RegisterAsync(RegisterRequestDto request)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);

            if (exists)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Email already registered."
                };
            }

            //var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = UidGenerator.GenerateUniqueId("U").ToString(),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                //PasswordHash = hashedPassword
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new ServiceResponse
            {
                Success = true,
                Message = "User registered successfully."
            };
        }

        // -------------------------------------------------------
        // Login
        // -------------------------------------------------------
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            //TODO: review this logic
            if (user == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid credentials."
                };
            }

            var token = _jwt.GenerateToken(user);

            return new LoginResponseDto
            {
                Success = true,
                Token = token,
                Email = user.Email,
                UserId = user.Id
            };
        }

        // -------------------------------------------------------
        // Get Authenticated User Profile
        // -------------------------------------------------------
        public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            return new UserProfileDto
            {
                UserId = user.Id,
                Email = user.Email
            };
        }

        public async Task<ServiceResponse> RegisterUser(CreateUserDto dto)
{
    // 1. Create the user in Firebase first
    var fbArgs = new UserRecordArgs
    {
        Email = dto.Email,
        Password = dto.Password,
        DisplayName = $"{dto.FirstName} {dto.LastName}",
        EmailVerified = false
    };

    UserRecord fbRecord;
    try {
        fbRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(fbArgs);
    }
    catch (Exception ex) {
        return new ServiceResponse { Success = false, Message = $"Firebase Error: {ex.Message}" };
    }

    // 2. Now create the user in Oracle using the UID we just got
    var newUser = new User
    {
        Id = UidGenerator.GenerateUniqueId("U").ToString(),
        FirebaseUid = fbRecord.Uid, // This satisfies your DB requirement
        Email = dto.Email,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        CreatedDate = DateTime.UtcNow
    };

    try {
        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();
        return new ServiceResponse { Success = true, Message = "User registered successfully in both systems." };
    }
    catch (Exception ex) {
        // Principal Tip: COMPENSATING TRANSACTION
        // If Oracle fails, we must delete the Firebase user we just created 
        // otherwise the systems get out of sync.
        await FirebaseAuth.DefaultInstance.DeleteUserAsync(fbRecord.Uid);
        return new ServiceResponse { Success = false, Message = "Database error. Registration rolled back." };
    }
}
        /// <summary>
        /// Allows User to sign in via Google
        /// </summary>
        /// <param name="firebaseUid"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ServiceResponse> SyncFirebaseUserAsync(string firebaseUid, UserSyncDto dto)
        {
            // 1. Check if the user already exists in Oracle by FirebaseUid
            // Note: You must add the 'FirebaseUid' property to your User model first!
            var user = await _db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

            if (user == null)
            {
                // 2. New User: Create record in Oracle
                user = new User
                {
                    // Keep your existing custom ID generation
                    Id = UidGenerator.GenerateUniqueId("U").ToString(),
                    FirebaseUid = firebaseUid,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    //PasswordHash = "EXTERNAL_FIREBASE_AUTH"
                };
                _db.Users.Add(user);
            }
            else
            {
                // 3. Existing User: Update profile if names changed
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                _db.Users.Update(user);
            }

            try
            {
                await _db.SaveChangesAsync();
                return new ServiceResponse { Success = true, Message = "Profile synced." };
            }
            catch (Exception ex)
            {
                return new ServiceResponse { Success = false, Message = $"Database Sync Failed: {ex.Message}" };
            }
        }
        
    }
}
