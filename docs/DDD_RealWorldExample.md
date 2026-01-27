# Domain-Driven Design — Real World Example 

This document is an original summary and explanation of the key ideas and examples from the referenced article on Domain-Driven Design (DDD). It captures the main concepts, patterns, and a concrete example (order placement) while rephrasing the original content.

## What is DDD?

Domain-Driven Design (DDD) is an approach that centers software design around a deep understanding of the business domain. Instead of letting infrastructure, controllers, or database schemas dictate the code structure, DDD encourages designing models that reflect business concepts, invariants, and behaviors.

The core idea: code should express the business. When business rules change, changes should primarily affect domain objects rather than scattered utilities or service layers.

## Traditional (layered) development vs DDD

Traditional layered architectures separate concerns into controllers, services, and DAOs/Repositories. Often the domain objects become anemic (data holders) and business rules live in services or utilities.

Problems with that approach:
- Business rules end up scattered across multiple layers.
- Domain objects become passive DTOs without behavior.
- Changing a business rule requires hunting through services, controllers, and utils.

DDD advocates a richer domain model:
- Entities and value objects encapsulate relevant business logic and invariants.
- Domain services represent operations that span multiple entities.
- Application services orchestrate use-cases (repositories + domain services + infrastructure) but contain little or no domain logic.

## Key DDD Concepts

- Aggregate Root: an entity that controls a consistency boundary. All modifications to related objects should go through the aggregate root to preserve invariants.

- Domain (vs Application) Service: a domain service holds business logic that cannot reasonably belong to a single entity or value object (for example, transferring funds between accounts). Application services coordinate a use-case, retrieving aggregates and calling domain services.

- Domain Events: explicit objects that represent important business state changes (e.g., `OrderCreated`, `UserRegistered`). They capture intent and can be published to other parts of the system.

## Traditional Development Model: A Task Creation Example

In traditional development, we follow requirements and write logic where the database design dictates the code. Suppose we are building a task creation feature with these rules:

* A Task must have a non-empty title.

* The due date cannot be in the past.

* A log must be recorded after a task is created.

In a traditional **"Anemic"** model, the Task object is just a data carrier, and the logic is scattered in the service:

```C#
// Anemic Model: Just a "data bag"
public class Task {
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; }
}

// Service layer: Bloated logic, business rules are scattered
public class TaskService {
    public void CreateTask(TaskDto dto) {
        // Validation Rule 1: logic is here or in a Util
        if (string.IsNullOrEmpty(dto.Title)) throw new Exception("Invalid Title");
        
        // Validation Rule 2: logic is in the service
        if (dto.DueDate < DateTime.Now) throw new Exception("Date in past");

        var task = new Task {
            Title = dto.Title,
            DueDate = dto.DueDate,
            Status = "Created"
        };

        _taskRepository.Save(task); // Data passed to DAO
        _logger.Log("Task Created");
    }
}
```
### The DDD Approach (Rich Model)
In DDD, business logic is encapsulated inside the domain object. The object is no longer just a "data bag"; it is responsible for its own valid state.

```C#
// Domain Entity: Encapsulates business logic
public class Task {
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public DateTime DueDate { get; private set; }

    public Task(string title, DateTime dueDate) {
        // Business rules encapsulated in the constructor
        if (string.IsNullOrWhiteSpace(title)) 
            throw new DomainException("Title is required.");
            
        if (dueDate < DateTime.UtcNow) 
            throw new DomainException("Due date cannot be in the past.");

        this.Id = Guid.NewGuid();
        this.Title = title;
        this.DueDate = dueDate;
    }
}
```
Explanation: The validation is pushed down into the Task entity. In professional terms, business rules are encapsulated inside the domain object.

## Key Design Concepts in DDD

### 1. Aggregate Root
Scenario: A TaskList is associated with multiple TaskItems.

* Traditional: Manage TaskList and TaskItems separately in services.

* DDD: Treat TaskList as the root. You cannot add an item to the list without going through the list’s logic (e.g., checking capacity).

```C#
public class TaskList {
    private readonly List<TaskItem> _items = new();
    public IReadOnlyCollection<TaskItem> Items => _items.AsReadOnly();

    public void AddItem(string title) {
        // Logic controlled by the aggregate root
        if (_items.Count >= 10) 
            throw new DomainException("List is full!");
            
        _items.Add(new TaskItem(title));
    }
}
```

### 2. Domain Service vs. Application Service

- Domain Service: Logic spanning multiple entities (e.g., moving a task from one list to another).

- Application Service: Orchestrates the process (calling repositories, then domain services, emails).

```C#
// Domain Service: Handles logic between two TaskLists
public class TaskTransferService {
    public void MoveTask(TaskList source, TaskList destination, TaskItem item) {
        source.RemoveItem(item);
        destination.AddItem(item.Title);
    }
}

// Application Service: Orchestrates, contains no business logic
public class TaskAppService {
    public void ExecuteMove(Guid itemId, Guid targetListId) {
        var item = _itemRepo.Find(itemId);
        var targetList = _listRepo.Find(targetListId);
        
        // Orchestration
        _transferService.MoveTask(currentList, targetList, item);
        _notificationService.NotifyUser("Task Moved"); // Infrastructure
    }
}
```

Putting rules close to the data they protect improves cohesion and reduces scatter.

## A DDD Real-World Example: Completing a Project

Suppose when a Project is marked "Complete," the system must: validate all tasks are done, update the project status, and notify the owner.

### Traditional (Anemic):

```C#
public class ProjectService {
    public void CompleteProject(Guid projectId) {
        var tasks = _db.Tasks.Where(t => t.ProjectId == projectId).ToList();
        
        // Logic scattered in Service
        if (tasks.Any(t => t.Status != "Done")) {
            throw new Exception("Tasks pending!");
        }

        var project = _db.Projects.Find(projectId);
        project.Status = "Completed"; // Pure data change
        _db.Save();
    }
}
```
### DDD (Rich):

```C#
public class Project {
    private List<Task> _tasks;
    public string Status { get; private set; }

    public void MarkAsComplete() {
        // Business logic encapsulated in the entity
        if (_tasks.Any(t => !t.IsCompleted)) {
            throw new DomainException("Cannot complete project with active tasks.");
        }

        this.Status = "Completed";
        // Record Domain Event
        this.AddEvent(new ProjectCompletedEvent(this.Id));
    }
}
```

The Benefit: When the rule changes (e.g., "Allow completion if only optional tasks are left"), you only change the Project entity. You don't have to "dig" through service layers to find the logic.

## When to Use DDD

- DDD is most valuable for complex domains with evolving business rules (e-commerce, finance, ERP).
- Avoid DDD for simple CRUD applications or small admin panels — it can be overengineering.

Use DDD when: changing business rules should ideally require only domain-level changes and not rip through controllers and DAOs.

## Practical Guidance

- Keep application services thin: they orchestrate repositories, domain services, and side-effects (messaging, persistence), but should not contain domain rules.
- Prefer meaningful domain objects (entities, value objects) that encapsulate behavior and invariants.
- Use domain events to make business state changes explicit and decoupled from infrastructure.

## References
 
- Inspired by [Domain-Driven Design Explained: A Real World Example](https://dev.to/leapcell/domain-driven-design-explained-a-real-world-example-581j) . This document is an original summary and does not reproduce the original text verbatim.


