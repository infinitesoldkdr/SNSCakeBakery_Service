using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Oracle.ManagedDataAccess.Client;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IdentityModel.Tokens.Jwt;

// Project Namespaces
using SNSCakeBakery_Service.Data;
using SNSCakeBakery_Service.Configuration;
using SNSCakeBakery_Service.Services.Interfaces;
using SNSCakeBakery_Service.Services.Implementations;
using SNSCakeBakery_Service.Services.Address;
using SNSCakeBakery_Service.Services.Helpers;
using SNSCakeBakery_Service.Services.Middleware;
using SNSCakeBakery_Service.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. INFRASTRUCTURE CONFIGURATION (Oracle & Firebase)
// ==========================================
var firebaseProjectId = builder.Configuration["Firebase:ProjectID"];
var walletPath = builder.Configuration["WalletPath"];

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(builder.Configuration["Firebase:APIKeyPath"])
});

OracleConfiguration.TnsAdmin = walletPath;
OracleConfiguration.WalletLocation = walletPath;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================
// 2. OPTIONS & STRONGLY-TYPED SETTINGS
// ==========================================
builder.Services.Configure<CloudflareOptions>(
    builder.Configuration.GetSection(CloudflareOptions.SectionName));

// ==========================================
// 3. CORE SERVICES (Dependency Injection)
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application Services
builder.Services.AddScoped<IImageService, CloudflareR2Service>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Helpers
builder.Services.AddSingleton<JwtTokenGenerator>();

// ==========================================
// 4. IDENTITY & SECURITY (Authentication & CORS)
// ==========================================
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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
    });

const string DevPolicy = "DevPolicy";
const string ProdPolicy = "ProdPolicy";

builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy(DevPolicy, policy => 
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    }
    else
    {
        var origins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        options.AddPolicy(ProdPolicy, policy => 
            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
    }
});

// ==========================================
// 5. REQUEST PIPELINE (Middleware)
// ==========================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(DevPolicy);
}
else
{
    app.UseCors(ProdPolicy);
}

// Order matters here! Auth -> Middleware -> Authorization
app.UseAuthentication(); 
app.UseMiddleware<JwtMiddleware>(); 
app.UseAuthorization(); 

app.MapControllers();

app.Run();