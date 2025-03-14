using System.Security.Claims;
using CleanArchitecture.Presentation.FunctionApp8;
using CleanArchitecture8.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var hostBuilder = new HostBuilder()
     .ConfigureFunctionsWorkerDefaults(
    worker =>
    {
        worker.UseNewtonsoftJson();
        worker.UseMiddleware<ExceptionHandlingMiddleware>();
    }
    );

    // remove this block
        //.ConfigureFunctionsWebApplication((IFunctionsWorkerApplicationBuilder builder) =>
        //{
        //     //builder.UseNewtonsoftJson();
        //}
        //);
    // and this from csproj in order to use the ExceptionHandlingMiddleware middleware   <!--<PackageReference Include="Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore" />-->

hostBuilder.ConfigureServices((context, services) =>
{
    var configuration = context.Configuration;

    services.AddSingleton<IHttpRequestProcessor, HttpRequestProcessor>();
    services.AddScoped<IUser, CurrentUser>();

    services.AddApplicationServices();
    services.AddInfrastructureServices(configuration);
    services.AddWebServices();

    //// set OpenAPIOptions for Swagger support
    var version = configuration.GetValue<string>("Version") ?? "1.0";
    services.AddSingleton<IOpenApiConfigurationOptions>(config => new HttpOpenApiConfigurationOptions(version));

    services.AddHttpClient();

});

//hostBuilder.ConfigureOpenApi();

var host = hostBuilder.Build();

await host.RunAsync();

//internal class CurrentUserService : ICurrentUserService
public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
public class HttpOpenApiConfigurationOptions : OpenApiConfigurationOptions
{
    private readonly string _version;

    public HttpOpenApiConfigurationOptions(string version)
    {
        _version = version;
    }
        /// <summary>
    /// Include host name.
    /// </summary>
    public override bool IncludeRequestingHostName { get { return false; } }
 
    /// <summary>
    /// OpenAPI version.
    /// </summary>
    public override OpenApiVersionType OpenApiVersion { get { return OpenApiVersionType.V3; } }

    public override OpenApiInfo Info { get; set; } = new OpenApiInfo
    {
        Title = "My Azure Function API",
        Version = "1.0.0",  // Default value (overridden later)
        Description = "API documentation for the Azure Functions project with Clean Architecture."
    };
}
