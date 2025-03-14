using CleanArchitecture8.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mime;

namespace CleanArchitecture.Presentation.FunctionApp8;

public class UserFunctions
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UserFunctions(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [OpenApiOperation(operationId: "RegisterUser", tags: RouteSectionName.Users, Summary = "Register a new User", Description = "This shows a welcome message.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity(Permissions.OpenApiSecuritySettings.BearerAuthenticationTitle, SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = Permissions.OpenApiSecuritySettings.JsonWebToken)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
    // Add these three attribute classes above
    [Function("RegisterUser")]
    public async Task<HttpResponseData> RegisterUser([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var requestBody = await System.Text.Json.JsonSerializer.DeserializeAsync<Dictionary<string, string>>(req.Body) ?? new Dictionary<string, string> { };
        var user = new ApplicationUser { UserName = requestBody["email"], Email = requestBody["email"], NormalizedUserName = requestBody["fullName"] };

        var result = await _userManager.CreateAsync(user, requestBody["password"]);
        var response = req.CreateResponse(result.Succeeded ? HttpStatusCode.OK : HttpStatusCode.BadRequest);

        await response.WriteAsJsonAsync(result);
        return response;
    }

    [OpenApiOperation(operationId: "SignIn", tags: RouteSectionName.Users, Summary = "User SignIn. This should returns a token.", Description = "Signs in a user with the provided credentials.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(SignInUser), Example = typeof(SignInUserExample), Description = "Details of the user for which we want an login.")]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(SignInResult), Summary = "SignIn successful", Description = "Returns the result of the sign-in process.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json", bodyType: typeof(SignInResult), Summary = "SignIn failed", Description = "Returns the result of the sign-in process.")]
    [Function("SignIn")]
    public async Task<HttpResponseData> SignIn([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        //administrator@localhost   Administrator1!
            //var result1 = await _signInManager.PasswordSignInAsync(requestBody["email"], requestBody["password"], false, false);

        using var reader = new StreamReader(req.Body);
        var requestBody = await reader.ReadToEndAsync();
        var requestUser = System.Text.Json.JsonSerializer.Deserialize<SignInUser>(requestBody, new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new SignInUser() { Email = "", Password = "" };


        var user = await _userManager.FindByNameAsync(requestUser.Email);
        SignInResult? result = user == null
            ? SignInResult.Failed
            : await _userManager.CheckPasswordAsync(user, requestUser.Password) ? SignInResult.Success : SignInResult.Failed;

        var response = req.CreateResponse(result.Succeeded ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);

            if (result.Succeeded)
            {
                //var token = _tokenService.GenerateToken(user.Id);
                var token = "ad a jwt token generator here";
                await response.WriteAsJsonAsync(new { Token = token });
            }
            else
            {
                await response.WriteAsJsonAsync(result);
            }

            return response;
    }

    [OpenApiOperation(operationId: "GetUser", tags: new[] { "Get User" }, Summary = "Get a  User", Description = "This shows a welcome message.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
    // Add these three attribute classes above
    [Function("GetUser")]
    public async Task<HttpResponseData> GetUser([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var response = req.CreateResponse(user != null ? HttpStatusCode.OK : HttpStatusCode.NotFound);

        await response.WriteAsJsonAsync(user);
        return response;
    }

    [OpenApiOperation(operationId: "UpdateUser", tags: new[] { "Update User" }, Summary = "Update a  User", Description = "This shows a welcome message.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
    // Add these three attribute classes above
    [Function("UpdateUser")]
    public async Task<HttpResponseData> UpdateUser([HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestData req)
    {
        var requestBody = await System.Text.Json.JsonSerializer.DeserializeAsync<Dictionary<string, string>>(req.Body) ?? new Dictionary<string, string> { };
        var user = await _userManager.FindByEmailAsync(requestBody["email"]);

        if (user == null)
            return req.CreateResponse(HttpStatusCode.NotFound);

        user.NormalizedUserName = requestBody["fullName"];
        var result = await _userManager.UpdateAsync(user);

        var response = req.CreateResponse(result.Succeeded ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        await response.WriteAsJsonAsync(result);
        return response;
    }

    [OpenApiOperation(operationId: "DeleteUser", tags: new[] { "Delete User" }, Summary = "Delete a  User", Description = "This shows a welcome message.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
    // Add these three attribute classes above
    [Function("DeleteUser")]
    public async Task<HttpResponseData> DeleteUser([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequestData req, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return req.CreateResponse(HttpStatusCode.NotFound);

        var result = await _userManager.DeleteAsync(user);
        var response = req.CreateResponse(result.Succeeded ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        await response.WriteAsJsonAsync(result);
        return response;
    }
}

    public class SignInUser
    {
        [JsonProperty("email")]
        public string Email { get; set; } = default!;

       [JsonProperty("password")]
        public string Password { get; set; }= default!;
    }

   public static class Permissions
    {
        /// <summary>
        ///API roles defined on a Azure App Registration which is the "Identity" of this Azure Function Application.
        /// </summary>
        public static class Roles
        {
            public const string ReadOnly = "Roles.Read.All";
            public const string ReadOnlyUI = "Roles.Read.UIAll";
            public const string ReadWrite = "Roles.ReadWrite.All";
            public const string ReadWriteDelete = "Roles.ReadWriteDelete.All";
        }
 
        /// <summary>
        /// OpenAPI settings for Authorisation.
        /// </summary>
        public static class OpenApiSecuritySettings
        {
            public const string BearerAuthenticationTitle = "Bearer_auth";
            public const string JsonWebToken = "JWT";
        }
    }
