using Microsoft.IdentityModel.Tokens;
using SNSCakeBakery_Service.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SNSCakeBakery_Service.Services.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public JwtMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContext(context, userService, token);

            await _next(context);
        }

        private async Task AttachUserToContext(
            HttpContext context,
            IUserService userService,
            string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

                // Attach user to HttpContext
                context.Items["User"] = await userService.GetUserProfileAsync(userId);
            }
            catch
            {
                // If JWT validation fails, do nothing.
                // User will NOT be attached to HttpContext.
            }
        }
    }
}
