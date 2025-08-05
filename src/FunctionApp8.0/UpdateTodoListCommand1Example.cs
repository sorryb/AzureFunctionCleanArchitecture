using System.Diagnostics.CodeAnalysis;
using CleanArchitecture8.Application.TodoLists.Commands.DeleteTodoList;
using CleanArchitecture8.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture8.Infrastructure.Identity;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Newtonsoft.Json.Serialization;

namespace CleanArchitecture.Presentation.FunctionApp8;

[ExcludeFromCodeCoverage]
public class UpdateTodoListCommand1Example : OpenApiExample<UpdateTodoListCommand>
{
    public override IOpenApiExample<UpdateTodoListCommand> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(OpenApiToDoExampleResolver.Resolve("UpdateTodoListCommand1Example", new UpdateTodoListCommand
        {
            Id = 1,
            Title = "Updated Todo List Title1"
        }, namingStrategy));

        Examples.Add(OpenApiToDoExampleResolver.Resolve("UpdateTodoListCommand2Example", new UpdateTodoListCommand
        {
            Id = 2,
            Title = "Updated Todo List Title2"
        }, namingStrategy));

        return this;
    }
}


