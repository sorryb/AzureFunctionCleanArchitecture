# CleanArchitecture8 Web Project

This project is part of the CleanArchitecture8 solution, specifically the Web component. Below is an overview of the project structure and its purpose.

## Clean Architecture Concept

Clean Architecture is a software design philosophy that emphasizes the separation of concerns and the independence of the business logic from the frameworks and tools used to implement it. The main goals of Clean Architecture are:

- **Independence of Frameworks**: The architecture does not depend on the existence of some library of feature-laden software. This allows you to use such frameworks as tools rather than having to cram your system into their limited constraints.
- **Testable**: The business rules can be tested without the UI, Database, Web Server, or any other external element.
- **Independent of UI**: The UI can change easily, without changing the rest of the system. For example, a web UI could be replaced with a console UI without changing the business rules.
- **Independent of Database**: You can swap out Oracle or SQL Server for Mongo, BigTable, CouchDB, or something else. Your business rules are not bound to the database.
- **Independent of any external agency**: In fact, your business rules simply don’t know anything at all about the outside world.

The architecture is typically represented by a series of concentric circles, with the innermost circle representing the most abstract and high-level policies, and the outermost circle representing the most concrete and low-level details.

## Project Structure

The `Web` project follows the principles of Clean Architecture and Domain-Driven Design (DDD). Here is a high-level overview of the structure:

```
/src/Web/
├── Endpoints/
│   ├── TodoItems.cs
│   ├── TodoLists.cs
│   ├── Users.cs
│   └── WeatherForecasts.cs
├── Infrastructure/
│   ├── EndpointGroupBase.cs
│   ├── IEndpointRouteBuilderExtensions.cs
│   ├── MethodInfoExtensions.cs
│   ├── WebApplicationExtensions.cs
│   └── CustomExceptionHandler.cs
├── Pages/
│   ├── Error.cshtml
│   ├── _ViewImports.cshtml
│   └── Shared/
│       └── _LoginPartial.cshtml
├── Services/
│   └── CurrentUser.cs
├── wwwroot/
│   ├── favicon.ico
│   └── specification.json
├── GlobalUsings.cs
├── Program.cs
├── DependencyInjection.cs
├── ReadMe.md
├── Web.csproj
├── appsettings.json
├── appsettings.Development.json
└── Web.http
```

### Endpoints

- **TodoItems.cs**: Endpoint definitions for TodoItems.
- **TodoLists.cs**: Endpoint definitions for TodoLists.
- **Users.cs**: Endpoint definitions for Users.
- **WeatherForecasts.cs**: Endpoint definitions for WeatherForecasts.

### Infrastructure

- **EndpointGroupBase.cs**: Base class for endpoint groups.
- **IEndpointRouteBuilderExtensions.cs**: Extensions for endpoint route builder.
- **MethodInfoExtensions.cs**: Extensions for MethodInfo.
- **WebApplicationExtensions.cs**: Extensions for WebApplication.
- **CustomExceptionHandler.cs**: Custom exception handler.

### Pages

- **Error.cshtml**: Error page.
- **_ViewImports.cshtml**: View imports.
- **Shared**
    - **_LoginPartial.cshtml**: Partial view for login.

### Services

- **CurrentUser.cs**: Service for current user.

### wwwroot

- **favicon.ico**: Favicon for the website.
- **specification.json**: API specification file.

### Configuration Files

- **appsettings.json**: Configuration settings for the application.
- **appsettings.Development.json**: Development-specific configuration settings.
- **Program.cs**: The entry point for the application.
- **DependencyInjection.cs**: Configures services and the app's request pipeline.
- **Web.csproj**: Project file.
- **Web.http**: HTTP file for testing API endpoints.
- **ReadMe.md**: Project documentation.

## Getting Started

To run the project, follow these steps:

1. **Restore Dependencies**: Run `dotnet restore` to restore the required packages.
2. **Build the Project**: Use `dotnet build` to compile the project.
3. **Run the Application**: Execute `dotnet run` to start the web server.

## Contributing

Contributions are welcome! Please follow the guidelines in the CONTRIBUTING.md file.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.
