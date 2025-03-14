using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Azure.Functions.Worker.Http;

namespace CleanArchitecture.Presentation.FunctionApp8;

public class LoginResponseMiddleware : IFunctionsWorkerMiddleware
{
    private const string SecretKey = "YOUR_SECRET_KEY"; // 🔹 Replace with a secure key!

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var logger = context.GetLogger<LoginResponseMiddleware>();

        // Get HttpRequestData
        var request = await context.GetHttpRequestDataAsync();
        if (request == null)
        {
            logger.LogWarning("Request data is missing.");
            await HandleErrorResponse(context, "Invalid request.");
            return;
        }

        // Only modify response for the /login endpoint
        if (request.Url.AbsolutePath.Equals("/api/login", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Login request detected. Processing...");

            // Execute the function
            await next(context);

            // Modify response after function executes
            var response = context.GetHttpResponseData();
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                // Generate JWT token for the user
                string token = GenerateJwtToken("testuser@example.com"); // Replace with actual user info

                // Add token to response
                await response.WriteStringAsync($"\n{{\"token\": \"{token}\"}}");
                logger.LogInformation("JWT token added to response.");
            }
        }
        else
        {
            // Continue normal execution for other endpoints
            await next(context);
        }
    }

    private string GenerateJwtToken(string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, "User")
            }),
            Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "https://your-app.com",
            Audience = "your-audience"
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task HandleErrorResponse(FunctionContext context, string message)
    {
        var response = context.GetHttpResponseData();
        if (response != null)
        {
            response.StatusCode = HttpStatusCode.BadRequest;
            await response.WriteStringAsync(message);
        }
    }
}
