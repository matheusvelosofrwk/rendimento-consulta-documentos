# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Rendimento.ConsultaDocumentos** is a .NET 8 solution implementing Clean Architecture for document consultation functionality. The solution includes both a REST API and an MVC Web application.

## Architecture

The solution follows Clean Architecture with 6 distinct layers:

### Layer Structure
- **ConsultaDocumentos.Domain**: Core entities and interfaces. Contains `BaseEntity` (with Guid Id) and `IBaseRepository<TEntity>` interface
- **ConsultaDocumentos.Application**: Business logic layer with services, DTOs, and AutoMapper profiles
- **ConsultaDocumentos.Infra.Data**: Data access layer with EF Core, repositories, entity configurations, and migrations
- **ConsultaDocumentos.Infra.Ioc**: Dependency injection configuration via `DependecyInjection.cs` extension methods
- **ConsultaDocumentos.API**: REST API project exposing endpoints via controllers
- **ConsultaDocumentos.Web**: MVC web application that consumes the API using Refit

### Key Dependencies Flow
```
API/Web → Infra.IoC → Application + Infra.Data → Domain
```

The IoC layer registers all services and repositories, configures EF Core DbContext, and AutoMapper.

## Common Commands

### Build and Run
```bash
# Build entire solution
dotnet build

# Run API (default ports configured in launchSettings.json)
dotnet run --project ConsultaDocumentos.API

# Run Web application
dotnet run --project ConsultaDocumentos.Web

# Restore all NuGet packages
dotnet restore
```

### Database Migrations
```bash
# Add new migration (run from solution root)
dotnet ef migrations add MigrationName --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API

# Update database
dotnet ef database update --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API

# Remove last migration
dotnet ef migrations remove --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API

# View migration SQL
dotnet ef migrations script --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API
```

**Note**: Migrations are stored in `ConsultaDocumentos.Infra.Data/Migrations/`. The startup project must be specified as the API project which has the connection string configuration.

## Key Patterns and Conventions

### Generic Repository Pattern
- `BaseRepository<TEntity>` in `Infra.Data/Repositories/` provides CRUD operations for all entities
- Entities inherit from `BaseEntity` (which has `Guid Id`)
- Repository interface `IBaseRepository<TEntity>` is in Domain layer
- Entity-specific repositories (like `ClienteRepository`) inherit from `BaseRepository<TEntity>`

### AutoMapper Configuration
- Mapping profiles are in `Application/Mappings/`
- Registration: `services.AddAutoMapper()` extension method in `Infra.Ioc/DependecyInjection.cs`
- Maps between Domain entities and Application DTOs

### Refit HTTP Client (Web Project Only)
- The Web project consumes the API via Refit typed clients
- Client interfaces in `Web/Clients/` (e.g., `IClienteApi`)
- Configuration in `Web/RefitConfiguration.cs` with:
  - Base URL from `appsettings.json` → `ApiClients:JsonPlaceholder:BaseUrl`
  - Standard resilience handler (3 retries, 10s timeout, circuit breaker)
- ViewModels in `Web/Models/` are used for Refit requests/responses

### Dependency Injection
- All DI configuration is centralized in `ConsultaDocumentos.Infra.Ioc/DependecyInjection.cs`
- Two main extension methods:
  - `AddInfrastructure(IConfiguration)`: Registers DbContext, repositories, and services
  - `AddAutoMapper()`: Registers AutoMapper profiles
- Connection string key: `"DefaultConnection"` in appsettings.json

### Database Configuration
- **Provider**: SQL Server (via `Microsoft.EntityFrameworkCore.SqlServer`)
- **Context**: `ApplicationDbContext` in `Infra.Data/Context/`
- **Entity Configurations**: Fluent API configurations in `Infra.Data/EntitiesConfiguration/`
- Configurations are auto-applied via `modelBuilder.ApplyConfigurationsFromAssembly()`

### MVC Views Structure (Web Project)
- Views are organized in `Web/Views/{ControllerName}/` folders
- Common views per entity: `Index.cshtml`, `Create.cshtml`, `Edit.cshtml`
- Shared views in `Web/Views/Shared/`: `_Layout.cshtml`, `_ValidationScriptsPartial.cshtml`, `Error.cshtml`
- Global configurations:
  - `_ViewImports.cshtml`: Imports namespaces and enables Tag Helpers
  - `_ViewStart.cshtml`: Sets default layout
- Views use strongly-typed models (ViewModels from `Web/Models/`)
- Tag Helpers are enabled for forms and validation (`asp-for`, `asp-action`, `asp-validation-for`)

## Adding New Entities

When adding a new entity, follow this pattern:

1. **Domain Layer**: Create entity inheriting from `BaseEntity` in `Domain/Entities/`
2. **Domain Layer**: Create repository interface inheriting from `IBaseRepository<TEntity>` in `Domain/Interfaces/`
3. **Infra.Data**: Create entity configuration in `Infra.Data/EntitiesConfiguration/` implementing `IEntityTypeConfiguration<TEntity>`
4. **Infra.Data**: Create repository implementation in `Infra.Data/Repositories/` inheriting from `BaseRepository<TEntity>`
5. **Infra.Data**: Add `DbSet<TEntity>` to `ApplicationDbContext`
6. **Application Layer**: Create DTO inheriting from `BaseDTO` in `Application/DTOs/`
7. **Application Layer**: Create service interface in `Application/Interfaces/`
8. **Application Layer**: Create service implementation in `Application/Services/`
9. **Application Layer**: Add mapping in `Application/Mappings/DomainToDTOMappingProfile.cs`
10. **Infra.IoC**: Register repository and service in `DependecyInjection.AddInfrastructure()`
11. **API**: Create controller in `API/Controllers/`
12. **Web** (if needed):
    - Create ViewModel in `Web/Models/` inheriting from `BaseViewModel`
    - Create Refit client interface in `Web/Clients/` with HTTP method attributes
    - Create MVC controller in `Web/Controllers/`
    - Create Razor Views in `Web/Views/{EntityName}/` folder:
      - `Index.cshtml`: List view with table displaying all records
      - `Create.cshtml`: Form for creating new records
      - `Edit.cshtml`: Form for editing existing records
      - (Optional) `Delete.cshtml`, `Details.cshtml`
13. **Create Migration**: Run `dotnet ef migrations add MigrationName --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API` command
14. **Apply Migration to Database**: Run `dotnet ef database update --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API` to execute the migration and update the database schema
