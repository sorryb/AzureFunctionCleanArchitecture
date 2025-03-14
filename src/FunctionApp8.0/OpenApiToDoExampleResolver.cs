using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core;
using Microsoft.OpenApi.Any;
using Newtonsoft.Json;

namespace CleanArchitecture.Presentation.FunctionApp8;

[ExcludeFromCodeCoverage]
public static class OpenApiToDoExampleResolver
    {
        private static readonly JsonSerializerSettings settings = new()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy.MM.dd",
        };
 
        public static KeyValuePair<string, OpenApiExample> Resolve<T>(string name, T instance, NamingStrategy? namingStrategy = null)
        {
            name.ThrowIfNullOrWhiteSpace();
            ArgumentNullException.ThrowIfNull(instance);
 
            DefaultContractResolver contractResolver = new()
            {
                NamingStrategy = namingStrategy ?? new DefaultNamingStrategy()
            };
            settings.ContractResolver = contractResolver;
            IOpenApiAny value = OpenApiExampleFactory.CreateInstance(instance, settings);
            OpenApiExample secondValue = new OpenApiExample
            {
                Value = value
            };
            return new KeyValuePair<string, OpenApiExample>(name, secondValue);
        }
    }


