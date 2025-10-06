# Rendimento.ConsultaDocumentos

Sistema de consulta de documentos desenvolvido com .NET 8, seguindo os princípios de Clean Architecture.

## 📋 Sobre o Projeto

**Rendimento.ConsultaDocumentos** é uma solução completa que implementa funcionalidades de consulta de documentos, oferecendo tanto uma API REST quanto uma aplicação Web MVC.

## 🏗️ Arquitetura

O projeto segue os princípios de Clean Architecture, organizado em 6 camadas distintas:

```
┌─────────────────────────────────────────┐
│         API          │       Web        │
├─────────────────────────────────────────┤
│             Infra.IoC                   │
├─────────────────────────────────────────┤
│   Application    │    Infra.Data       │
├─────────────────────────────────────────┤
│              Domain                     │
└─────────────────────────────────────────┘
```

### Camadas

- **ConsultaDocumentos.Domain**: Entidades de domínio e interfaces. Contém `BaseEntity` e `IBaseRepository<TEntity>`
- **ConsultaDocumentos.Application**: Lógica de negócio, serviços, DTOs e perfis AutoMapper
- **ConsultaDocumentos.Infra.Data**: Acesso a dados com EF Core, repositórios, configurações de entidades e migrations
- **ConsultaDocumentos.Infra.Ioc**: Configuração de injeção de dependências via `DependecyInjection.cs`
- **ConsultaDocumentos.API**: API REST com controllers
- **ConsultaDocumentos.Web**: Aplicação MVC que consome a API usando Refit

## 🚀 Tecnologias Utilizadas

- **.NET 8**: Framework principal
- **Entity Framework Core**: ORM para acesso a dados
- **SQL Server**: Banco de dados relacional
- **AutoMapper**: Mapeamento entre entidades e DTOs
- **Refit**: Cliente HTTP tipado para consumo da API
- **ASP.NET Core MVC**: Interface web
- **ASP.NET Core Web API**: API REST

## 📁 Estrutura do Projeto

```
Rendimento.ConsultaDocumentos/
├── ConsultaDocumentos.Domain/          # Entidades e interfaces
│   ├── Entities/
│   └── Interfaces/
├── ConsultaDocumentos.Application/     # Lógica de negócio
│   ├── DTOs/
│   ├── Services/
│   ├── Interfaces/
│   └── Mappings/
├── ConsultaDocumentos.Infra.Data/      # Acesso a dados
│   ├── Context/
│   ├── Repositories/
│   ├── EntitiesConfiguration/
│   └── Migrations/
├── ConsultaDocumentos.Infra.Ioc/       # Injeção de dependências
├── ConsultaDocumentos.API/             # API REST
│   └── Controllers/
└── ConsultaDocumentos.Web/             # Aplicação MVC
    ├── Controllers/
    ├── Views/
    ├── Models/
    └── Clients/
```

## ⚙️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (Express ou superior)
- Editor de código (Visual Studio, VS Code, Rider, etc.)

## 🔧 Configuração e Instalação

### 1. Clone o repositório

```bash
git clone <url-do-repositorio>
cd rendimento-consulta-documentos/Rendimento.ConsultaDocumentos
```

### 2. Restaure as dependências

```bash
dotnet restore
```

### 3. Configure a string de conexão

Edite o arquivo `appsettings.json` nos projetos **API** e **Web** com sua string de conexão do SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RendimentoConsultaDocumentos;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 4. Execute as migrations

```bash
dotnet ef database update --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API
```

### 5. Execute o projeto

**Executar a API:**
```bash
dotnet run --project ConsultaDocumentos.API
```

**Executar a aplicação Web:**
```bash
dotnet run --project ConsultaDocumentos.Web
```

## 📝 Comandos Úteis

### Build

```bash
# Build da solução completa
dotnet build

# Build de um projeto específico
dotnet build ConsultaDocumentos.API
```

### Migrations

```bash
# Adicionar nova migration
dotnet ef migrations add NomeDaMigration --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API

# Atualizar banco de dados
dotnet ef database update --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API

# Remover última migration
dotnet ef migrations remove --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API

# Gerar script SQL da migration
dotnet ef migrations script --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API
```

## 🎯 Padrões e Convenções

### Repository Pattern Genérico
- `BaseRepository<TEntity>` fornece operações CRUD para todas as entidades
- Entidades herdam de `BaseEntity` (com propriedade `Guid Id`)
- Interface `IBaseRepository<TEntity>` está na camada Domain

### AutoMapper
- Perfis de mapeamento em `Application/Mappings/`
- Mapeamento entre entidades de domínio e DTOs

### Refit (Projeto Web)
- Cliente HTTP tipado para consumo da API
- Interfaces em `Web/Clients/`
- Configuração com resiliência (3 tentativas, timeout de 10s, circuit breaker)

### Dependency Injection
- Toda configuração centralizada em `ConsultaDocumentos.Infra.Ioc/DependecyInjection.cs`
- Métodos de extensão: `AddInfrastructure()` e `AddAutoMapper()`

## 📦 Adicionando Novas Entidades

Para adicionar uma nova entidade ao sistema, siga esta ordem:

1. Criar entidade em `Domain/Entities/` herdando de `BaseEntity`
2. Criar interface do repositório em `Domain/Interfaces/`
3. Criar configuração da entidade em `Infra.Data/EntitiesConfiguration/`
4. Criar implementação do repositório em `Infra.Data/Repositories/`
5. Adicionar `DbSet<TEntity>` no `ApplicationDbContext`
6. Criar DTO em `Application/DTOs/`
7. Criar interface do serviço em `Application/Interfaces/`
8. Criar implementação do serviço em `Application/Services/`
9. Adicionar mapeamento em `Application/Mappings/DomainToDTOMappingProfile.cs`
10. Registrar repositório e serviço em `Infra.IoC/DependecyInjection.cs`
11. Criar controller em `API/Controllers/`
12. (Opcional) Criar ViewModel, cliente Refit, controller MVC e views em `Web/`
13. Criar migration com `dotnet ef migrations add`
14. Aplicar migration com `dotnet ef database update`
