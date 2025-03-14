using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Azure.Functions.Worker.Http;

namespace CleanArchitecture.Presentation.FunctionApp8;
public class JwtValidationMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var logger = context.GetLogger<JwtValidationMiddleware>();

        // Extract HttpRequestData
        var request = await context.GetHttpRequestDataAsync();
        if (request == null)
        {
            logger.LogWarning("Request data is missing.");
            await HandleUnauthorizedResponse(context);
            return;
        }

        // Check for Authorization header
        var authHeader = request.Headers.FirstOrDefault(h => h.Key == "Authorization").Value.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            logger.LogWarning("Missing or invalid Authorization header.");
            await HandleUnauthorizedResponse(context);
            return;
        }

        var token = authHeader.Substring("Bearer ".Length);

        try
        {
            var claimsPrincipal = ValidateJwtToken(token);

            // Store validated user in FunctionContext for later use
            context.Items["User"] = claimsPrincipal;
        }
        catch (SecurityTokenException ex)
        {
            logger.LogWarning($"JWT validation failed: {ex.Message}");
            await HandleUnauthorizedResponse(context);
            return;
        }

        // Call the next middleware or function
        await next(context);
    }

    private ClaimsPrincipal ValidateJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("YOUR_SECRET_KEY"); // 🔹 Replace with your signing key

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://your-issuer.com",
            ValidAudience = "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }

    private async Task HandleUnauthorizedResponse(FunctionContext context)
    {
        var response = context.GetHttpResponseData();
        if (response != null)
        {
            response.StatusCode = HttpStatusCode.Unauthorized;
            await response.WriteStringAsync("Unauthorized");
        }
    }
}
