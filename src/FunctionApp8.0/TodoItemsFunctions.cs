using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Presentation.FunctionApp8
{
    public class TodoItemsFunctions
    {
        private readonly IHttpRequestProcessor processor;
        private readonly ILogger<TodoItemsFunctions> logger;

        public TodoItemsFunctions(IHttpRequestProcessor processor, ILogger<TodoItemsFunctions> logger)
        {
            this.logger = logger;
            this.processor = processor;
        }

        //[Function(nameof(GetTodoItems))]
        //public async Task<HttpResponseData> GetTodoItems([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todolists/{id}")]
        //    HttpRequestData req,
        //    int id,
        //    FunctionContext functionContext)
        //{
        //    logger.LogInformation("Called GetTodoItems");

        //    if(req.Headers.TryGetValues("Content-Type", out IEnumerable<string> values))
        //    {
        //        if(values.Contains("text/csv"))
        //        {
        //            var request = new ExportTodosQuery()
        //            {
        //                ListId = id
        //            };
        //            return await this.processor.ExecuteAsync<ExportTodosQuery, ExportTodosVm>(functionContext,
        //                                                                req,
        //                                                                request,
        //                                                                (r) => req.CreateFileContentResponseAsync(r.Content, r.ContentType, r.FileName));
        //        }
        //    }

        //    var query =  new GetTodoItemsWithPaginationQuery()
        //    {
        //        ListId = id
        //    };

        //    return await this.processor.ExecuteAsync<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemDto>>(functionContext,
        //                                                        req,
        //                                                        query,
        //                                                        (r) => req.CreateObjectResponseAsync(r));
        //}

        //[Function(nameof(CreateTodoItem))]
        //public async Task<HttpResponseData> CreateTodoItem([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todolists/{id}/items")]
        //    HttpRequestData req,
        //    int id,
        //    FunctionContext functionContext)
        //{
        //    logger.LogInformation("Called CreateTodoItems");

        //    var todoList = await req.ReadFromJsonAsync<TodoList>();
        //    var command = new CreateTodoItemCommand
        //    {
        //        ListId = id,
        //        Title = todoList.Title
        //    };

        //    return await this.processor.ExecuteAsync<CreateTodoItemCommand, int>(functionContext,
        //                                                        req,
        //                                                        command,
        //                                                        (r) => req.CreateObjectCreatedResponseAsync($"todolists/{id}/items", r));
        //}
    }
}
