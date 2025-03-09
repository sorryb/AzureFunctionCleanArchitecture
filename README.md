# Azure Function v4(.net8) and Web App with Clean Architecture .net8

The project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture8) version 8.0.6.

## Build

Run `dotnet build -tl` to build the solution.

## Run

To run the web application:

```bash
cd .\src\Web\
dotnet watch run
```

Navigate to https://localhost:5001. The application will automatically reload if you change any of the source files.

## Code Styles & Formatting

The template includes [EditorConfig](https://editorconfig.org/) support to help maintain consistent coding styles for multiple developers working on the same project across various editors and IDEs. The **.editorconfig** file defines the coding styles applicable to this solution.

## Code Scaffolding

The template includes support to scaffold new commands and queries.

Start in the `.\src\Application\` folder.

Create a new command:

```
dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int
```

Create a new query:

```
dotnet new ca-usecase -n GetTodos -fn TodoLists -ut query -rt TodosVm
```

If you encounter the error *"No templates or subcommands found matching: 'ca-usecase'."*, install the template and try again:

```bash
dotnet new install Clean.Architecture.Solution.Template::8.0.6
```

## Test

The solution contains unit, integration, and functional tests.

To run the tests:
```bash
dotnet test
```

## Help
To learn more about the template go to the [project website](https://github.com/jasontaylordev/CleanArchitecture). Here you can find additional guidance, request new features, report a bug, and discuss the template with other users.

* [CleanArchitecture github](https://github.com/jasontaylordev/CleanArchitecture/tree/main)
* [Clean Architecture with ASP.NET Core 3.0 • Jason Taylor • GOTO 2019 - video](https://www.youtube.com/watch?v=dK4Yb6-LxAk)
* [Clean Architecture with .NET Core: Getting Started Jason Taylor](https://jasontaylor.dev/clean-architecture-getting-started/)

### Reference

For more detailed information on how to implement and configure Azure Functions, refer to the official [Azure Functions documentation](https://docs.microsoft.com/en-us/azure/azure-functions/).

[Azure Functions - Part 1 - Hosting and Networking Options](https://techcommunity.microsoft.com/blog/fasttrackforazureblog/azure-functions---part-1---hosting-and-networking-options/3746795)

[Azure Functions - Part 2 - Unit and Integration Testing](https://techcommunity.microsoft.com/blog/fasttrackforazureblog/azure-functions---part-2---unit-and-integration-testing/3769764)

## Git Commands

```shell
PS D:\> git init
PS D:\> git config user.email "sxxxx@xxxx.com"
PS D:\> git config user.name "xxxxx"
PS D:\> git config --list
```

```shell
gh auth status
gh auth login
gh repo create AzureFunctionCleanArchitecture --public
```

```shell
git add *
git commit -m "starting first commit"
git branch -M main
git remote add origin https://github.com/sorryb/AzureFunctionCleanArchitecture.git
git push -u origin main

git status
```