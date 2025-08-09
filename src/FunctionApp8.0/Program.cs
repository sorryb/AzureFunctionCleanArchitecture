using System.Security.Claims;
using CleanArchitecture.Presentation.FunctionApp8;
using CleanArchitecture8.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

    services.AddScoped<IUser, CurrentUser>();

    services.AddApplicationServices();
    services.AddInfrastructureServices(configuration);
    //services.AddWebServices();

    //// set OpenAPIOptions for Swagger support
    var version = configuration.GetValue<string>("Version") ?? "1.0";
    services.AddSingleton<IOpenApiConfigurationOptions>(config => new HttpOpenApiConfigurationOptions(version));

    services.AddHttpClient();

});

//hostBuilder.ConfigureOpenApi();

var host = hostBuilder.Build();

await host.RunAsync();

/// <summary>
/// Used in Bihaviours
/// </summary>
public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
