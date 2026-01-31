using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.Services.Address;
using SNSCakeBakery_Service.Services.Helpers;
using SNSCakeBakery_Service.Services.Implementations;
using SNSCakeBakery_Service.Services.Interfaces;
using SNSCakeBakery_Service.Services.Middleware;
using System.IdentityModel.Tokens.Jwt;
using Oracle.ManagedDataAccess.Client;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(builder.Configuration["Firebase:APIKeyPath"])
});
var firebaseProjectId = builder.Configuration["Firebase:ProjectID"];
var walletPath =  builder.Configuration["WalletPath"];
//@"/Users/delantedawkins/Projects/Wallet_SNSCAKEBAKERY";

OracleConfiguration.TnsAdmin = walletPath;
OracleConfiguration.WalletLocation = walletPath;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// Add services
builder.Services.AddControllers();

// DI Registrations
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true
        };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            // Console.WriteLine("\n--- AUTHENTICATION FAILED ---");
            // Console.WriteLine($"Error: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // Console.WriteLine("\n--- TOKEN VALIDATED SUCCESSFULLY ---");
            return Task.CompletedTask;
        }
    };
}); 

builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();


const string DevPolicy = "DevPolicy";
const string ProdPolicy = "ProdPolicy";


builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy(DevPolicy, policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    }
    else
    {
        var origins = builder.Configuration
            .GetSection("CorsSettings:AllowedOrigins")
            .Get<string[]>();

        options.AddPolicy(ProdPolicy, policy =>
        {
            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); 
        });
    }
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseCors(DevPolicy);
}
else
{
    app.UseCors(ProdPolicy);
}

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); 

app.UseMiddleware<JwtMiddleware>(); 

app.UseAuthorization(); 

app.MapControllers();
app.Run();