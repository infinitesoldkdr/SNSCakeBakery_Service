using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.Services.Address;
using SNSCakeBakery_Service.Services.Helpers;
using SNSCakeBakery_Service.Services.Implementations;
using SNSCakeBakery_Service.Services.Interfaces;
using SNSCakeBakery_Service.Services.Middleware;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// 1. FIX: Prevent .NET from renaming "sub" to long XML schemas
// This ensures User.FindFirstValue(ClaimTypes.NameIdentifier) or "sub" works correctly.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Add services
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// DI Registrations
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// 2. UPDATED AUTHENTICATION: Use values from appsettings.json
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero 
    };

    // --- ADD THIS SECTION TO SEE ERRORS IN CONSOLE ---
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("\n--- AUTHENTICATION FAILED ---");
            Console.WriteLine($"Error: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("\n--- TOKEN VALIDATED SUCCESSFULLY ---");
            return Task.CompletedTask;
        }
    };
}); 
builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3. CORRECT MIDDLEWARE ORDER
// Authentication MUST come before Authorization and your custom Middleware
app.UseAuthentication(); 

// Use custom middleware AFTER authentication so the User is already identified
app.UseMiddleware<JwtMiddleware>(); 

app.UseAuthorization(); 

app.MapControllers();
app.Run();