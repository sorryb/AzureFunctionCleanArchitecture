using System.Net;
using System.Net.Mime;
using CleanArchitecture8.Application.Common.Models;
using CleanArchitecture8.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture8.Application.TodoItems.Commands.DeleteTodoItem;
using CleanArchitecture8.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture8.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;

namespace CleanArchitecture.Presentation.FunctionApp8;
    public class TodoItemsFunctions
    {
        private readonly ILogger<TodoItemsFunctions> _logger;

        private readonly ISender _sender;

        public TodoItemsFunctions(ILogger<TodoItemsFunctions> logger, ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        [OpenApiOperation(operationId: "GetTodoItems", tags: RouteSectionName.Items, Summary = "Get Todo Items", Description = "Retrieve a list of todo items with pagination.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Example = typeof(ItemIdExample), Summary = "The ID of the todo list", Description = "The ID of the todo list to retrieve items from")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PaginatedList<TodoItemBriefDto>), Summary = "The response", Description = "This returns the list of todo items")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json", bodyType: typeof(SignInResult), Summary = "Unauthorized", Description = "Returns the result of the unauthorized access")]
        [Function(nameof(GetTodoItems))]
        public async Task<PaginatedList<TodoItemBriefDto>> GetTodoItems([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todolists/items/{id}")]
           HttpRequestData req,
           int id,
           FunctionContext functionContext)
        {
           _logger.LogInformation("Called Get Todo Items");


           var query =  new GetTodoItemsWithPaginationQuery()
           {
               ListId = id
           };

            return await _sender.Send(query);
        }

        [OpenApiOperation(operationId: "CreateTodoItem", tags: RouteSectionName.Items, Summary = "Create Todo Item", Description = "Create a new todo item.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(TodoList), Example = typeof(TodoListExample), Description = "Details of the todo item to be created.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(int), Summary = "The response", Description = "This returns the ID of the created todo item")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json", bodyType: typeof(SignInResult), Summary = "Unauthorized", Description = "Returns the result of the unauthorized access")]
        [Function(nameof(CreateTodoItem))]
        public async Task<int> CreateTodoItem([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todolists/{id}/items")]
           HttpRequestData req,
           int id,
           FunctionContext functionContext)
        {
           _logger.LogInformation("Called CreateTodoItems");

           var todoList = await req.ReadFromJsonAsync<TodoList>();

           var command = new CreateTodoItemCommand
           {
               ListId = id,
               Title = todoList?.Title
           };

            return await _sender.Send(command);
        }

        [OpenApiOperation(operationId: "DeleteTodoItem", tags: RouteSectionName.Items, Summary = "Delete Todo Item", Description = "Delete an existing todo item.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiParameter(name: "itemId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "The ID of the todo item to delete", Description = "The ID of the todo item to delete")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(void), Summary = "The response", Description = "This returns no content")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json", bodyType: typeof(SignInResult), Summary = "Unauthorized", Description = "Returns the result of the unauthorized access")]
        [Function(nameof(DeleteTodoItem))]
        public async Task<IResult>  DeleteTodoItem([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todolists/items/{itemId}")] HttpRequestData req, int itemId)
        {
            await _sender.Send(new DeleteTodoItemCommand(itemId));
            return Results.NoContent();
        }

        [OpenApiOperation(operationId: "UpdateTodoItem", tags: RouteSectionName.Items, Summary = "Update Todo Item", Description = "Update an existing todo item.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: MediaTypeNames.Application.Json, bodyType: typeof(UpdateTodoItemCommand), Example = typeof(UpdateTodoItemCommandExample), Description = "Details of the todo item to be updated.")]
        [OpenApiParameter(name: "itemId", In = ParameterLocation.Path, Required = true, Type = typeof(int), Summary = "The ID of the todo item to update", Description = "The ID of the todo item to update")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(void), Summary = "The response", Description = "This returns no content")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Unauthorized, contentType: "application/json", bodyType: typeof(SignInResult), Summary = "Unauthorized", Description = "Returns the result of the unauthorized access")]
        [Function(nameof(UpdateTodoItem))]
        public async Task<IResult>  UpdateTodoItem([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todolists/items/{itemId}")] HttpRequestData req, int itemId)
        {
            var command = await req.ReadFromJsonAsync<UpdateTodoItemCommand>();

            if (itemId != command?.Id) return Results.BadRequest();

            await _sender.Send(command);
            return Results.NoContent();
        }
    }

    public class UpdateTodoItemCommandExample : OpenApiExample<UpdateTodoItemCommand>
    {
        public override IOpenApiExample<UpdateTodoItemCommand> Build(NamingStrategy? namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Update Todo Item Command Example", new UpdateTodoItemCommand
            {
                Id = 1,
                Title = "Updated Todo Item",
                Done = true
            }, namingStrategy));

            Examples.Add(OpenApiExampleResolver.Resolve("Simple Update Todo Item Command Example", new UpdateTodoItemCommand
            {
                Id = 2,
                Title = "Another Updated Todo Item",
                Done = false
            }, namingStrategy));

            return this;
        }
    }

    public class TodoListExample : OpenApiExample<TodoList>
    {
        public override IOpenApiExample<TodoList> Build(NamingStrategy? namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Todo List Example", new TodoList
            {
                Title = "Sample Todo List"
            }, namingStrategy));

            Examples.Add(OpenApiExampleResolver.Resolve("Simple Todo List Example", new TodoList
            {
                Title = "Another Sample Todo List"
            }, namingStrategy));

            return this;
        }
    }

    public class ItemIdExample : OpenApiExample<int>
    {
        public override IOpenApiExample<int> Build(NamingStrategy? namingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Item ID Example", 1, namingStrategy));

            Examples.Add(OpenApiExampleResolver.Resolve("Simple Item ID Example", 2, namingStrategy));

            return this;
        }
    }

