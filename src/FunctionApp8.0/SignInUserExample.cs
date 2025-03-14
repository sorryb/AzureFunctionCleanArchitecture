using CleanArchitecture8.Infrastructure.Identity;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Newtonsoft.Json.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace CleanArchitecture.Presentation.FunctionApp8;

[ExcludeFromCodeCoverage]
public class SignInUserExample : OpenApiExample<SignInUser>
{
    public override IOpenApiExample<SignInUser> Build(NamingStrategy? namingStrategy = null )
    {
        Examples.Add(OpenApiToDoExampleResolver.Resolve("SignInUserExample", new SignInUser
        {
            Email = "administrator@localhost",
            Password = "Administrator1!"
        }, namingStrategy));

        // Examples.Add(OpenApiToDoExampleResolver.Resolve("SignInUserExample2", new SignInUser
        // {
        //     Email = "administrator@localhost",
        //     Password = "Administrator1!"
        // }, namingStrategy));

        return this;
    }
}


