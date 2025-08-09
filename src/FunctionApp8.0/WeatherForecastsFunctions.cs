using System.Net;
using CleanArchitecture8.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace CleanArchitecture.Presentation.FunctionApp8
{
    public class WeatherForecastsFunctions
    {
        private readonly ISender _sender;

        public WeatherForecastsFunctions( ISender sender)
        {
            _sender = sender;
        }

        [OpenApiOperation(operationId: "Weather", tags: RouteSectionName.Wheater, Summary = "Get weather", Description = "This shows a weather message.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
        // Add these three attribute classes above
        [Function(nameof(GetWeatherForecasts))]
        public async /*Task<HttpResponseData>*/Task<IEnumerable<WeatherForecast>> GetWeatherForecasts([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "weather/forecasts")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger<WeatherForecastsFunctions>();
            logger.LogInformation("Called GetWeatherForecasts");

            return await _sender.Send(new GetWeatherForecastsQuery());

        }

        [OpenApiOperation(operationId: "Weather", tags: RouteSectionName.Wheater, Summary = "Get weather", Description = "This shows a weather message.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
        // Add these three attribute classes above
        [Function(nameof(GetWeatherForecastsWithRequestProcessor) )]
        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsWithRequestProcessor([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "weather/forecasts2")] HttpRequestData req,
    FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger<WeatherForecastsFunctions>();
            logger.LogInformation("Called GetWeatherForecasts");

            return await _sender.Send(new GetWeatherForecastsQuery());

        }
    }
}
