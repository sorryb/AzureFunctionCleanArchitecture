using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanArchitecture8.Functions
{
    public class TokenService
    {
        private readonly IConfiguration? _configuration;

        public TokenService(IConfiguration? configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration?["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT Key is not configured.");
            var key = Encoding.UTF8.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userId)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

// how to use it

    // public static class LoginFunction
    // {
    //     [FunctionName("Login")]
    //     public static async Task<IActionResult> Run(
    //         [HttpTrigger(AuthorizationLevel.Function, "post", Route = "login")] HttpRequest req,
    //         ILogger log,
    //         [Inject] TokenService tokenService)
    //     {
    //         string email = req.Form["email"];
    //         string password = req.Form["password"];

    //         // Validate user credentials (this is just an example, replace with your actual validation logic)
    //         if (email == "user@example.com" && password == "password")
    //         {
    //             string userId = "123"; // Replace with actual user ID
    //             var token = tokenService.GenerateToken(userId);
    //             return new OkObjectResult(new { Token = token });
    //         }

    //         return new UnauthorizedResult();
    //     }
    // }

// Update your local.settings.json to include JWT settings:
//     {
//   "IsEncrypted": false,
//   "Values": {
//     "AzureWebJobsStorage": "UseDevelopmentStorage=true",
//     "FUNCTIONS_WORKER_RUNTIME": "dotnet"
//   },
//   "Jwt": {
//     "Key": "your-secret-key",
//     "Issuer": "your-issuer",
//     "Audience": "your-audience"
//   }
// }