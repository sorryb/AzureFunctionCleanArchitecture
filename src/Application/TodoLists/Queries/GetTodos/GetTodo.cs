using CleanArchitecture8.Application.Common.Interfaces;
using CleanArchitecture8.Application.Common.Models;
using CleanArchitecture8.Application.Common.Security;
using CleanArchitecture8.Domain.Entities;
using CleanArchitecture8.Domain.Enums;

namespace CleanArchitecture8.Application.TodoLists.Queries.GetTodos;

//[Authorize]
public record GetTodoQuery : IRequest<TodoListDto>
{
    public int Id { get; init; }

    public string? Title { get; init; }
}

public class GetTodoQueryHandler : IRequestHandler<GetTodoQuery, TodoListDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TodoListDto?> Handle(GetTodoQuery request, CancellationToken cancellationToken)
    {
        var listTodo = await _context.TodoLists.FirstOrDefaultAsync(x => x.Id == request.Id);
        var res = _mapper.Map<TodoList?, TodoListDto>(listTodo);
        return res;
    }
}
