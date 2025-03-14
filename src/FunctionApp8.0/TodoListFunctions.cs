using System.Net;
using CleanArchitecture8.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture8.Application.TodoLists.Commands.DeleteTodoList;
using CleanArchitecture8.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture8.Application.TodoLists.Queries.GetTodos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace CleanArchitecture.Presentation.FunctionApp8
{
    public class TodoListFunctions
    {
        private readonly IHttpRequestProcessor processor;
        private readonly ILogger<TodoListFunctions> logger;
        private readonly ISender _sender;

        public TodoListFunctions(IHttpRequestProcessor processor, ILogger<TodoListFunctions> logger, ISender sender)
        {
            this.logger = logger;
            this.processor = processor;
            _sender = sender;
        }

        [OpenApiOperation(operationId: "ToDO", tags: RouteSectionName.ToDo, Summary = "GetTodos", Description = "This shows a welcome message.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
        // Add these three attribute classes above
        [Function(nameof(GetTodos))]
        public async /*Task<HttpResponseData>*/ Task<TodosVm> GetTodos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todolists")]
            HttpRequestData req,
            FunctionContext functionContext)
        {
            logger.LogInformation("Called GetTodos");

            return await _sender.Send(new GetTodosQuery());
        }

        [OpenApiOperation(operationId: "ToDO", tags: RouteSectionName.ToDo, Summary = "Get Todo List by Id", Description = "This shows a todo message.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
        // Add these three attribute classes above
        [Function(nameof(GetTodosById))]
        public async /*Task<HttpResponseData>*/ Task<TodoListDto> GetTodosById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todolists/{id}")]
            HttpRequestData req,
            int id,
            FunctionContext functionContext)
        {
            logger.LogInformation($"Called GetTodo for {id}");

            return await _sender.Send(new GetTodoQuery() { Id = id});
        }

        [OpenApiOperation(operationId: "CreateTodosList", tags: RouteSectionName.ToDo, Summary = "Create Todos List", Description = "This shows a to do created message.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
        // Add these three attribute classes above
        [Function(nameof(CreateTodosList))]
        public async /*Task<HttpResponseData>*/Task<int> CreateTodosList([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todolists")]
                HttpRequestData req,
            FunctionContext functionContext)
        {
            logger.LogInformation("Called CreateTodosList");

            var todoList = await req.ReadFromJsonAsync<TodoList>();
            var command = new CreateTodoListCommand
            {
                Title = todoList?.Title
            };

            return await _sender.Send(command);
        }

        [OpenApiOperation(operationId: "UpdateTodosList", tags: RouteSectionName.ToDo, Summary = "Update Todos List", Description = "This shows a welcome message.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
        // Add these three attribute classes above
        [Function(nameof(UpdateTodosList))]
        public async Task<HttpResponseData> /*Task<IResult>*/ UpdateTodosList([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todolists/{id}")]
                HttpRequestData req,
            int id,
            FunctionContext functionContext)
        {
            logger.LogInformation("Called UpdateTodosList");

            var todoList = await req.ReadFromJsonAsync<TodoList>();
            var command = new UpdateTodoListCommand
            {
                Id = id,
                Title = todoList?.Title
            };

            //if (id != command.Id) return Results.BadRequest();
            //await _sender.Send(command);
            //return Results.NoContent();

            return await this.processor.ExecuteAsync<UpdateTodoListCommand, Unit>(functionContext,
                                                                req,
                                                                command,
                                                                (r) => req.CreateResponseAsync());
        }

        [OpenApiOperation(operationId: "DeleteTodosList", tags: RouteSectionName.ToDo, Summary = "DeleteTodosList", Description = "This shows a delete message.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
        // Add these three attribute classes above
        [Function(nameof(DeleteTodosList))]
        public async /*Task<HttpResponseData> */ Task<IResult>  DeleteTodosList([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todolists/{id}")]
                HttpRequestData req,
            int id,
            FunctionContext functionContext)
        {
            logger.LogInformation("Called DeleteTodosList");

            var request = new DeleteTodoListCommand(id)
            {
                Id = id
            };

            await _sender.Send(new DeleteTodoListCommand(id));
            return  Results.NoContent();
        }
    }

    public class TodoList
    {
        public string? Title { get; set; }
    }
}
