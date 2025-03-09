using CleanArchitecture8.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Presentation.FunctionApp8;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddScoped<IUser, CurrentUser>();

        services.AddHttpContextAccessor();

        //services.AddHealthChecks()
        //    .AddDbContextCheck<ApplicationDbContext>();

        //services.AddExceptionHandler<CustomExceptionHandler>(); // this works only for Web API; use middleware for Azure Functions

        //services.AddRazorPages();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();

        //services.AddOpenApiDocument((configure, sp) =>
        //{
        //    configure.Title = "CleanArchitecture8 API";

        //    // Add JWT
        //    configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
        //    {
        //        Type = OpenApiSecuritySchemeType.ApiKey,
        //        Name = "Authorization",
        //        In = OpenApiSecurityApiKeyLocation.Header,
        //        Description = "Type into the textbox: Bearer {your JWT token}."
        //    });

        //    configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        //});

        return services;
    }

    //public static IServiceCollection AddKeyVaultIfConfigured(this IServiceCollection services, ConfigurationManager configuration)
    //{
    //    var keyVaultUri = configuration["AZURE_KEY_VAULT_ENDPOINT"];
    //    if (!string.IsNullOrWhiteSpace(keyVaultUri))
    //    {
    //        configuration.AddAzureKeyVault(
    //            new Uri(keyVaultUri),
    //            new DefaultAzureCredential());
    //    }

    //    return services;
    //}
}
