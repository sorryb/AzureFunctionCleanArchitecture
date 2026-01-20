using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

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
