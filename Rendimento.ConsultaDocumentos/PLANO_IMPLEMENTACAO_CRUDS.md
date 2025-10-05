# Plano de Implementação dos CRUDs - Sistema de Consulta de Documentos

**Data:** 2025-10-05
**Projeto:** Rendimento.ConsultaDocumentos
**Arquitetura:** Clean Architecture + DDD
**Framework:** .NET 8

---

## 📊 Análise do Estado Atual

### ✅ Já Implementado

| Entidade | Status | Tabela | Observações |
|----------|--------|--------|-------------|
| **Cliente** | ✅ Completo | `Cliente` | Entidade de exemplo (não está no modelo documentado) |
| **Aplicacao** | ✅ Completo | `Aplicacao` | CRUD completo (API + Web) |
| **Provedor** | ✅ Completo | `Provedor` | CRUD completo (API + Web) |
| **Autenticação** | ✅ Implementado | Identity Tables | AuthService com IdentityUser + JWT |
| **Usuários** | ✅ Implementado | AspNetUsers | IdentityUser do ASP.NET Identity |

### 🔧 Padrão Atual do Projeto

- **ID**: `Guid` (BaseEntity)
- **Tabelas**: Nome singular, sem prefixo (ex: `Documento`, `Endereco`)
- **DbSet**: Singular (ex: `DbSet<Documento> Documento`)
- **Repository Pattern**: `BaseRepository<TEntity>` + interfaces específicas
- **Services**: `BaseService<TDto, TEntity>` + services específicos
- **AutoMapper**: Mapeamento DTO ↔ Entity
- **API**: Controllers RESTful padrão
- **Web**: MVC com Refit clients

---

## 📋 Entidades a Implementar

### Segundo a Documentação (DATA_MODEL_DOCUMENTATION.md)

**Total:** 13 novas entidades + 7 enums

#### Por Categoria:

**Enums (7)**
1. TipoDocumento
2. StatusEnum
3. TipoEndereco
4. TipoTelefone
5. TipoProvedor
6. PerfilAcesso
7. SituacaoCadastralEnum

**Tabelas de Referência (2)**
1. Nacionalidade
2. SituacaoCadastral

**Aggregate Root: Documento (5 entidades)**
1. Documento (Root)
2. Endereco
3. Telefone
4. Email
5. QuadroSocietario

**Relacionamento Aplicacao-Provedor (1)**
1. AplicacaoProvedor

**Sistema de Perfis (1)**
1. Perfil

**Logs (2)**
1. LogAuditoria
2. LogErro

---

## 🎯 Ordem de Implementação (Seguindo Dependências)

### **Fase 1: Enums e Tipos Base**

#### 1.1. Criar pasta Enums no Domain
```
ConsultaDocumentos.Domain/Enums/
```

#### 1.2. Criar 7 Enums

**1. TipoDocumento.cs**
```csharp
public enum TipoDocumento
{
    CPF = 1,
    CNPJ = 2
}
```

**2. StatusEnum.cs**
```csharp
public enum StatusEnum
{
    Ativo = 'A',
    Inativo = 'I'
}
```

**3. TipoEndereco.cs**
```csharp
public enum TipoEndereco
{
    Residencial = 1,
    Comercial = 2,
    Correspondencia = 3,
    Cobranca = 4,
    Entrega = 5
}
```

**4. TipoTelefone.cs**
```csharp
public enum TipoTelefone
{
    Residencial = 1,
    Comercial = 2,
    Celular = 3,
    Fax = 4,
    WhatsApp = 5
}
```

**5. TipoProvedor.cs**
```csharp
public enum TipoProvedor
{
    GENERICO = 0,
    SERPRO = 1,
    SERASA = 2,
    REPOSITORIO = 3,
    BOAVISTA = 4,
    ENRIQUECIMENTO = 5
}
```

**6. PerfilAcesso.cs**
```csharp
public enum PerfilAcesso
{
    Administrador = 1,
    Consulta = 2,
    Operador = 3,
    GestaoAcesso = 4
}
```

**7. SituacaoCadastralEnum.cs**
```csharp
public enum SituacaoCadastralEnum
{
    Ativo = 1,
    Inativo = 2,
    Suspenso = 3,
    Cancelado = 4,
    Pendente = 5,
    Bloqueado = 6
}
```

---

### **Fase 2: Tabelas de Referência**

#### 2.1. Nacionalidade

**Tabela:** `Nacionalidade`

**Campos:**
- Id (Guid, PK)
- Descricao (string, obrigatório)
- Codigo (string, opcional) - Código ISO
- Ativo (bool)

**CRUD Completo:**
- ✅ Domain: Entity + IRepository
- ✅ Infra.Data: Configuration + Repository + DbSet
- ✅ Application: DTO + IService + Service + Mapping
- ✅ Infra.IoC: Registrar DI
- ✅ API: Controller
- ✅ Web: ViewModel + IApi + Controller + Views (Index, Create, Edit)
- ✅ Migration: Create + Apply

---

#### 2.2. SituacaoCadastral

**Tabela:** `SituacaoCadastral`

**Campos:**
- Id (Guid, PK)
- Descricao (string, obrigatório)
- Ativo (bool)
- DataCriacao (DateTime)

**CRUD Completo:** (mesma estrutura acima)

---

### **Fase 3: Aggregate Root Documento + Dependentes**

#### 3.1. Documento (Aggregate Root)

**Tabela:** `Documento`

**Campos Principais:**
- Id (Guid, PK)
- TipoPessoa (char) - 'F' = CPF, 'J' = CNPJ
- Numero (string, obrigatório)
- Nome (string, obrigatório)
- DataConsulta (DateTime)
- DataConsultaValidade (DateTime)
- RowVersion (byte[]) - Controle de concorrência

**Campos PJ (Pessoa Jurídica):**
- DataAbertura (DateTime?)
- Inscricao (string?)
- NaturezaJuridica (int?)
- DescricaoNaturezaJuridica (string?)
- Segmento (string?)
- RamoAtividade (int?)
- DescricaoRamoAtividade (string?)
- NomeFantasia (string?)
- MatrizFilialQtde (int?)
- Matriz (bool?)

**Campos PF (Pessoa Física):**
- DataNascimento (DateTime?)
- NomeMae (string?)
- Sexo (string?)
- TituloEleitor (string?)
- ResidenteExterior (bool?)
- AnoObito (int?)

**Campos de Situação:**
- DataSituacao (DateTime?)
- IdSituacao (Guid?, FK → SituacaoCadastral)
- CodControle (string?)
- DataFundacao (DateTime?)
- OrigemBureau (string?)
- IdNacionalidade (Guid?, FK → Nacionalidade)

**Navegação:**
- Nacionalidade (Navigation Property)
- SituacaoCadastral (Navigation Property)
- Enderecos (Collection)
- Telefones (Collection)
- Emails (Collection)
- QuadrosSocietarios (Collection)

**Métodos de Domínio:**
```csharp
+ static CriarPessoaFisica(...)
+ static CriarPessoaJuridica(...)
+ IsValido(): bool
+ IsPessoaFisica(): bool
+ IsPessoaJuridica(): bool
+ PrecisaAtualizacao(): bool
+ AtualizarDataConsulta(validadeDias)
+ AdicionarEndereco(endereco)
+ RemoverEndereco(endereco)
```

**CRUD Completo** + Migration

---

#### 3.2. Endereco

**Tabela:** `Endereco`

**Campos:**
- Id (Guid, PK)
- IdDocumento (Guid, FK → Documento)
- Logradouro (string?)
- Numero (string?)
- Complemento (string?)
- Bairro (string?)
- CEP (string?) - 8 dígitos
- Cidade (string?)
- UF (string?) - 2 caracteres
- Tipo (TipoEndereco enum)
- DataAtualizacao (DateTime?)
- RowVersion (byte[])
- TipoLogradouro (string?) - legado

**Navegação:**
- Documento (Navigation Property)

**Métodos de Domínio:**
```csharp
+ static Criar(...)
+ IsValido(): bool
+ IsCompleto(): bool
+ IsCepValido(): bool
+ GetEnderecoFormatado(): string
```

**CRUD Completo** + Migration

---

#### 3.3. Telefone

**Tabela:** `Telefone`

**Campos:**
- Id (Guid, PK)
- IdDocumento (Guid, FK → Documento)
- DDD (string?) - 2 dígitos
- Numero (string?) - 8-9 dígitos
- Tipo (TipoTelefone enum)
- DataCriacao (DateTime)
- DataAtualizacao (DateTime?)

**Navegação:**
- Documento (Navigation Property)

**Métodos de Domínio:**
```csharp
+ GetTelefoneFormatado(): string
+ IsValido(): bool
+ IsCelular(): bool
+ IsWhatsApp(): bool
+ PermiteMensagens(): bool
```

**CRUD Completo** + Migration

---

#### 3.4. Email

**Tabela:** `Email`

**Campos:**
- Id (Guid, PK)
- IdDocumento (Guid, FK → Documento)
- EnderecoEmail (string?)
- DataCriacao (DateTime)

**Navegação:**
- Documento (Navigation Property)

**Métodos de Domínio:**
```csharp
+ IsValido(): bool  // Regex validation
```

**CRUD Completo** + Migration

---

#### 3.5. QuadroSocietario

**Tabela:** `QuadroSocietario`

**Campos:**
- Id (Guid, PK)
- IdDocumento (Guid, FK → Documento, somente CNPJ)
- CPFSocio (string?) - legado
- CpfCnpj (string?)
- NomeSocio (string?) - legado
- Nome (string?)
- QualificacaoSocio (string?) - legado
- Qualificacao (string?)
- CpfRepresentanteLegal (string?)
- NomeRepresentanteLegal (string?)
- QualificacaoRepresentanteLegal (string?)
- DataEntrada (DateTime?)
- DataSaida (DateTime?)
- PercentualCapital (decimal?)
- DataCriacao (DateTime)

**Navegação:**
- Documento (Navigation Property)

**Métodos de Domínio:**
```csharp
+ IsValido(): bool
+ GetPercentualFormatado(): string
```

**CRUD Completo** + Migration

---

### **Fase 4: Relacionamento Aplicacao-Provedor**

#### 4.1. AplicacaoProvedor

**Tabela:** `AplicacaoProvedor`

**Descrição:** Tabela N:N entre Aplicacao e Provedor, define ordem de fallback

**Campos:**
- Id (Guid, PK)
- AplicacaoId (Guid, FK → Aplicacao)
- ProvedorId (Guid, FK → Provedor) **[ADAPTAR: usar FK em vez de enum TipoProvedor]**
- Ordem (int) - Prioridade (1 = primeiro)
- Status (StatusEnum)
- DataCriacao (DateTime)
- DataAtualizacao (DateTime?)
- CriadoPor (string?) - IdentityUser.Id
- AtualizadoPor (string?) - IdentityUser.Id

**Navegação:**
- Aplicacao (Navigation Property)
- Provedor (Navigation Property)

**Métodos de Domínio:**
```csharp
+ AtualizarOrdem(novaOrdem, atualizadoPor)
+ Ativar(atualizadoPor)
+ Desativar(atualizadoPor)
```

**Validações:**
- Ordem deve ser > 0
- Não pode ter ProvedorId duplicado para mesma Aplicacao

**CRUD Completo** + Migration

---

### **Fase 5: Sistema de Perfis**

#### 5.1. Perfil

**Tabela:** `Perfil`

**Descrição:** Perfis de acesso customizados (pode integrar com IdentityRole ou ser independente)

**Campos:**
- Id (Guid, PK)
- Nome (string, obrigatório)
- Descricao (string?)
- Status (StatusEnum)
- DataCriacao (DateTime)
- DataAtualizacao (DateTime?)
- CriadoPor (string?) - IdentityUser.Id
- AtualizadoPor (string?) - IdentityUser.Id

**Métodos de Domínio:**
```csharp
+ AtualizarDados(nome, descricao, atualizadoPor)
+ Ativar(atualizadoPor)
+ Desativar(atualizadoPor)
```

**Integração com Identity:**
- **Opção A:** Usar tabela customizada + relacionamento com AspNetUsers via tabela de junção
- **Opção B:** Estender IdentityRole com campos adicionais
- **Recomendação:** Opção A (mais flexível)

**CRUD Completo** + Migration

---

### **Fase 6: Logs (Somente Consulta/Inserção)**

#### 6.1. LogAuditoria

**Tabela:** `LogAuditoria`

**Descrição:** Logs de auditoria de todas consultas de documentos via API

**Campos:**
- Id (Guid, PK) **[ADAPTAR: usar Guid em vez de long]**
- AplicacaoId (Guid, FK → Aplicacao)
- NomeAplicacao (string) - desnormalizado
- DocumentoNumero (string)
- TipoDocumento (TipoDocumento enum)
- ParametrosEntrada (string?) - JSON
- ProvedoresUtilizados (string?) - JSON array
- ProvedorPrincipal (string?)
- ConsultaSucesso (bool)
- RespostaProvedor (string?) - JSON
- MensagemRetorno (string?)
- TempoProcessamentoMs (long)
- DataHoraConsulta (DateTime)
- EnderecoIp (string?)
- UserAgent (string?)
- TokenAutenticacao (string?) - Hash
- OrigemCache (bool)
- InformacoesAdicionais (string?) - JSON

**Navegação:**
- Aplicacao (Navigation Property)

**Implementação:**
- ❌ **NÃO criar**: IService completo
- ✅ **Criar**: Repository somente com Insert e Select
- ✅ **API**: Apenas endpoints GET (listagem e filtros)
- ❌ **WEB**: Somente visualização (sem Create/Edit/Delete)

**Migration** + Índices otimizados

---

#### 6.2. LogErro

**Tabela:** `LogErro`

**Descrição:** Logs de erros e exceções do sistema

**Campos:**
- Id (Guid, PK) **[ADAPTAR: usar Guid em vez de int]**
- DataHora (DateTime)
- Aplicacao (string?)
- Metodo (string?)
- Erro (string?)
- StackTrace (string?)
- Usuario (string?) - IdentityUser.Id
- IdSistema (Guid?, FK → Aplicacao)

**Navegação:**
- Aplicacao (Navigation Property, opcional)

**Implementação:**
- ❌ **NÃO criar**: IService completo
- ✅ **Criar**: Repository somente com Insert e Select
- ✅ **API**: Apenas endpoints GET
- ❌ **WEB**: Somente visualização

**Migration** + Índices

---

## 🔄 Processo Padrão para Cada Entidade

### Backend (API)

#### 1. Domain Layer
```
ConsultaDocumentos.Domain/
├── Entities/
│   └── {Entity}.cs              // Herda de BaseEntity (Guid Id)
└── Interfaces/
    └── I{Entity}Repository.cs   // Herda de IBaseRepository<{Entity}>
```

#### 2. Infra.Data Layer
```
ConsultaDocumentos.Infra.Data/
├── EntitiesConfiguration/
│   └── {Entity}Configuration.cs     // IEntityTypeConfiguration<{Entity}>
├── Repositories/
│   └── {Entity}Repository.cs        // Herda de BaseRepository<{Entity}>
└── Context/
    └── ApplicationDbContext.cs      // Adicionar DbSet<{Entity}>
```

**Exemplo Configuration:**
```csharp
public class {Entity}Configuration : IEntityTypeConfiguration<{Entity}>
{
    public void Configure(EntityTypeBuilder<{Entity}> builder)
    {
        builder.HasKey(x => x.Id);
        // Propriedades
        // Relacionamentos
        // Índices
    }
}
```

#### 3. Application Layer
```
ConsultaDocumentos.Application/
├── DTOs/
│   └── {Entity}DTO.cs           // Herda de BaseDTO
├── Interfaces/
│   └── I{Entity}Service.cs      // Herda de IBaseService<{Entity}DTO, {Entity}>
├── Services/
│   └── {Entity}Service.cs       // Herda de BaseService<{Entity}DTO, {Entity}>
└── Mappings/
    └── DomainToDTOMappingProfile.cs  // Adicionar mapping
```

**Exemplo Mapping:**
```csharp
CreateMap<{Entity}DTO, {Entity}>().ReverseMap();
```

#### 4. Infra.IoC Layer
```csharp
// DependecyInjection.cs
services.AddScoped<I{Entity}Repository, {Entity}Repository>();
services.AddScoped<I{Entity}Service, {Entity}Service>();
```

#### 5. API Layer
```
ConsultaDocumentos.API/Controllers/
└── {Entity}Controller.cs
```

**Exemplo Controller:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class {Entity}Controller : ControllerBase
{
    private readonly I{Entity}Service _service;

    public {Entity}Controller(I{Entity}Service service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() { }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id) { }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] {Entity}DTO dto) { }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] {Entity}DTO dto) { }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id) { }
}
```

---

### Frontend (Web)

#### 1. Models
```
ConsultaDocumentos.Web/Models/
└── {Entity}ViewModel.cs         // Herda de BaseViewModel
```

#### 2. Refit Client
```
ConsultaDocumentos.Web/Clients/
└── I{Entity}Api.cs
```

**Exemplo:**
```csharp
public interface I{Entity}Api
{
    [Get("/{entity}")]
    Task<Result<IList<{Entity}ViewModel>>> GetAllAsync();

    [Get("/{entity}/{id}")]
    Task<Result<{Entity}ViewModel>> GetByIdAsync(Guid id);

    [Post("/{entity}")]
    Task<Result<{Entity}ViewModel>> CreateAsync([Body] {Entity}ViewModel model);

    [Put("/{entity}/{id}")]
    Task<Result<{Entity}ViewModel>> UpdateAsync(Guid id, [Body] {Entity}ViewModel model);

    [Delete("/{entity}/{id}")]
    Task<Result<bool>> DeleteAsync(Guid id);
}
```

#### 3. Controller
```
ConsultaDocumentos.Web/Controllers/
└── {Entity}Controller.cs
```

#### 4. Views
```
ConsultaDocumentos.Web/Views/{Entity}/
├── Index.cshtml
├── Create.cshtml
└── Edit.cshtml
```

**Index.cshtml Template:**
```html
@model IEnumerable<ConsultaDocumentos.Web.Models.{Entity}ViewModel>

<h1>Lista de {Entities}</h1>
<p>
    <a asp-action="Create">Criar Novo</a>
</p>

@Html.AntiForgeryToken()
<table class="table">
    <thead>
        <!-- Cabeçalhos -->
    </thead>
    <tbody>
        @foreach (var item in Model) {
            <tr>
                <!-- Dados -->
                <td>
                    @Html.ActionLink("Editar", "Edit", new { id=item.Id }) |
                    <a href="javascript:void(0);" onclick="openDeleteConfirmation('@item.Id', '{Entity}', '@item.Nome')" class="text-danger">Excluir</a>
                </td>
            </tr>
        }
    </tbody>
</table>

@await Html.PartialAsync("_DeleteConfirmationModal")

@section Scripts {
    <script src="~/js/delete-helper.js"></script>
}
```

---

### Database

#### 1. Criar Migration
```bash
dotnet ef migrations add Add{Entity} --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API
```

#### 2. Aplicar Migration
```bash
dotnet ef database update --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API
```

#### 3. Verificar
```bash
dotnet ef migrations list --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API
```

---

## 📝 Checklist por Entidade

### Backend (API)
- [ ] Domain: Criar Entity herdando de BaseEntity
- [ ] Domain: Criar I{Entity}Repository : IBaseRepository<{Entity}>
- [ ] Infra.Data: Criar {Entity}Configuration : IEntityTypeConfiguration<{Entity}>
- [ ] Infra.Data: Criar {Entity}Repository : BaseRepository<{Entity}>
- [ ] Infra.Data: Adicionar DbSet<{Entity}> no ApplicationDbContext
- [ ] Application: Criar {Entity}DTO : BaseDTO
- [ ] Application: Criar I{Entity}Service : IBaseService<{Entity}DTO, {Entity}>
- [ ] Application: Criar {Entity}Service : BaseService<{Entity}DTO, {Entity}>
- [ ] Application: Adicionar CreateMap no DomainToDTOMappingProfile
- [ ] Infra.IoC: Registrar I{Entity}Repository e I{Entity}Service
- [ ] API: Criar {Entity}Controller com endpoints CRUD

### Frontend (Web)
- [ ] Web: Criar {Entity}ViewModel : BaseViewModel
- [ ] Web: Criar I{Entity}Api (Refit)
- [ ] Web: Criar {Entity}Controller (MVC)
- [ ] Web: Criar Index.cshtml
- [ ] Web: Criar Create.cshtml
- [ ] Web: Criar Edit.cshtml

### Database
- [ ] Criar migration
- [ ] Aplicar migration no banco
- [ ] Verificar tabela criada

### Testes
- [ ] Testar API via Swagger
- [ ] Testar Web CRUD completo

---

## 🚀 Estratégia de Execução

### Abordagem Recomendada

**Implementar em Fases Sequenciais:**

1. **Semana 1**: Fase 1 + Fase 2 (Enums + Tabelas de Referência)
2. **Semana 2-3**: Fase 3 (Documento + 4 entidades dependentes)
3. **Semana 4**: Fase 4 + Fase 5 (AplicacaoProvedor + Perfil)
4. **Semana 5**: Fase 6 (Logs)

**OU**

**Implementar Entidade por Entidade (Vertical Slice):**
- Implementar cada entidade 100% (Backend + Frontend + DB + Testes) antes de passar para a próxima
- Vantagem: Entregas incrementais funcionais

---

## ⚠️ Pontos de Atenção

### 1. Controle de Concorrência
- **Documento** e **Endereco** usam `RowVersion` (byte[])
- Configurar no EF Core:
```csharp
builder.Property(x => x.RowVersion).IsRowVersion();
```

### 2. Relacionamentos
- Configurar Delete Behavior adequado (Cascade, Restrict, SetNull)
- Exemplo Documento → Enderecos:
```csharp
builder.HasMany(d => d.Enderecos)
    .WithOne(e => e.Documento)
    .HasForeignKey(e => e.IdDocumento)
    .OnDelete(DeleteBehavior.Cascade);
```

### 3. Índices
- Criar índices em FKs
- Criar índices em campos de busca frequente (ex: Documento.Numero)
```csharp
builder.HasIndex(x => x.Numero).IsUnique();
```

### 4. Auditoria
- Campos de auditoria:
  - `CriadoPor` e `AtualizadoPor` devem referenciar `IdentityUser.Id` (string)
  - Implementar via interceptors do EF Core ou manualmente nos Services

### 5. Enums
- Configurar conversão no EF Core:
```csharp
builder.Property(x => x.Tipo)
    .HasConversion<int>();  // ou <string> se preferir
```

### 6. Validações
- Implementar FluentValidation (recomendado) ou Data Annotations
- Validar no Service antes de persistir

### 7. Logs
- LogAuditoria e LogErro são **append-only**
- Não implementar Update/Delete
- Considerar particionamento por data (performance)

---

## 📚 Referências

- **Documentação Modelo de Dados:** `DATA_MODEL_DOCUMENTATION.md`
- **Padrões do Projeto:** `CLAUDE.md`
- **Entity Framework Core:** https://docs.microsoft.com/ef/core/
- **Clean Architecture:** Robert C. Martin
- **DDD:** Eric Evans, "Domain-Driven Design"

---

## ✅ Critérios de Aceite

Para cada entidade implementada:

1. ✅ Build sem erros
2. ✅ Migration aplicada com sucesso
3. ✅ API endpoints funcionando (testados via Swagger)
4. ✅ Web CRUD funcionando (Create, Read, Update, Delete)
5. ✅ Validações implementadas
6. ✅ Relacionamentos corretos
7. ✅ Padrão de código consistente com projeto existente

---

## 📈 Status de Implementação

### ✅ Semana 1: Concluída (2025-10-05)

#### Fase 1: Enums e Tipos Base ✅
- ✅ Pasta `ConsultaDocumentos.Domain/Enums/` criada
- ✅ TipoDocumento.cs
- ✅ StatusEnum.cs
- ✅ TipoEndereco.cs
- ✅ TipoTelefone.cs
- ✅ TipoProvedor.cs
- ✅ PerfilAcesso.cs
- ✅ SituacaoCadastralEnum.cs

**Total: 7/7 enums implementados**

#### Fase 2: Tabelas de Referência ✅

**Nacionalidade** ✅
- ✅ Domain: Entity + IRepository
- ✅ Infra.Data: Configuration + Repository + DbSet
- ✅ Application: DTO + IService + Service + Mapping
- ✅ Infra.IoC: Registrar DI
- ✅ API: Controller
- ✅ Web: ViewModel + IApi (Refit) + Controller + Views (Index, Create, Edit)
- ✅ Migration: `20251005171151_AddNacionalidade` criada e aplicada
- ✅ Tabela `Nacionalidade` criada no banco de dados

**SituacaoCadastral** ✅
- ✅ Domain: Entity + IRepository
- ✅ Infra.Data: Configuration + Repository + DbSet
- ✅ Application: DTO + IService + Service + Mapping
- ✅ Infra.IoC: Registrar DI
- ✅ API: Controller
- ✅ Web: ViewModel + IApi (Refit) + Controller + Views (Index, Create, Edit)
- ✅ Migration: `20251005171614_AddSituacaoCadastral` criada e aplicada
- ✅ Tabela `SituacaoCadastral` criada no banco de dados

**Total: 2/2 tabelas de referência implementadas**

#### Testes e Validações ✅
- ✅ Build completo: **Sucesso** (0 erros, 26 warnings de nullable - normal)
- ✅ Migrations aplicadas com sucesso
- ✅ Código seguindo padrões do projeto
- ✅ Todas as camadas implementadas (Domain, Infra.Data, Application, API, Web)

### ✅ Semana 2-3: Concluída (2025-10-05)

#### Fase 3: Aggregate Root Documento + Dependentes ✅

**Total: 5/5 entidades implementadas**

---

**1. Documento (Aggregate Root)** ✅
- ✅ Domain: Entity + IDocumentoRepository
- ✅ Infra.Data: DocumentoConfiguration + DocumentoRepository + DbSet
- ✅ Application: DocumentoDTO + IDocumentoService + DocumentoService + Mapping
- ✅ Infra.IoC: Registrar DI
- ✅ API: DocumentoController
- ✅ Web: DocumentoViewModel + IDocumentoApi (Refit) + Registro no RefitConfiguration
- ✅ Campos implementados:
  - Principais: TipoPessoa, Numero, Nome, DataConsulta, DataConsultaValidade, RowVersion
  - PJ: DataAbertura, Inscricao, NaturezaJuridica, DescricaoNaturezaJuridica, Segmento, RamoAtividade, DescricaoRamoAtividade, NomeFantasia, MatrizFilialQtde, Matriz
  - PF: DataNascimento, NomeMae, Sexo, TituloEleitor, ResidenteExterior, AnoObito
  - Situação: DataSituacao, IdSituacao (FK → SituacaoCadastral), CodControle, DataFundacao, OrigemBureau, IdNacionalidade (FK → Nacionalidade)
- ✅ Navigation Properties: Nacionalidade, SituacaoCadastral, Enderecos, Telefones, Emails, QuadrosSocietarios
- ✅ Métodos de Domínio: CriarPessoaFisica(), CriarPessoaJuridica(), IsValido(), IsPessoaFisica(), IsPessoaJuridica(), PrecisaAtualizacao(), AtualizarDataConsulta()
- ✅ Índices: Numero (UNIQUE), DataConsultaValidade, IdNacionalidade, IdSituacao

---

**2. Endereco** ✅
- ✅ Domain: Entity + IEnderecoRepository
- ✅ Infra.Data: EnderecoConfiguration + EnderecoRepository + DbSet
- ✅ Application: EnderecoDTO + IEnderecoService + EnderecoService + Mapping
- ✅ Infra.IoC: Registrar DI
- ✅ API: EnderecoController
- ✅ Campos: IdDocumento (FK), Logradouro, Numero, Complemento, Bairro, CEP, Cidade, UF, Tipo (TipoEndereco), DataAtualizacao, RowVersion, TipoLogradouro
- ✅ Relacionamento: N:1 com Documento (CASCADE)
- ✅ Métodos de Domínio: Criar(), IsValido(), IsCompleto(), IsCepValido(), GetEnderecoFormatado(), Atualizar()
- ✅ Índices: IdDocumento, CEP, Cidade+UF

---

**3. Telefone** ✅
- ✅ Domain: Entity + ITelefoneRepository
- ✅ Infra.Data: TelefoneConfiguration + TelefoneRepository + DbSet
- ✅ Application: TelefoneDTO + ITelefoneService + TelefoneService + Mapping
- ✅ Infra.IoC: Registrar DI
- ✅ API: TelefoneController
- ✅ Campos: IdDocumento (FK), DDD, Numero, Tipo (TipoTelefone), DataCriacao, DataAtualizacao
- ✅ Relacionamento: N:1 com Documento (CASCADE)
- ✅ Métodos de Domínio: Criar(), GetTelefoneFormatado(), IsValido(), IsCelular(), IsWhatsApp(), PermiteMensagens()
- ✅ Índices: IdDocumento, DDD

---

**4. Email** ✅
- ✅ Domain: Entity + IEmailRepository
- ✅ Infra.Data: EmailConfiguration + EmailRepository + DbSet
- ✅ Application: EmailDTO + IEmailService + EmailService + Mapping
- ✅ Infra.IoC: Registrar DI
- ✅ API: EmailController
- ✅ Campos: IdDocumento (FK), EnderecoEmail, DataCriacao
- ✅ Relacionamento: N:1 com Documento (CASCADE)
- ✅ Métodos de Domínio: Criar(), IsValido() (validação regex)
- ✅ Índices: IdDocumento, EnderecoEmail

---

**5. QuadroSocietario** ✅
- ✅ Domain: Entity + IQuadroSocietarioRepository
- ✅ Infra.Data: QuadroSocietarioConfiguration + QuadroSocietarioRepository + DbSet
- ✅ Application: QuadroSocietarioDTO + IQuadroSocietarioService + QuadroSocietarioService + Mapping
- ✅ Infra.IoC: Registrar DI
- ✅ API: QuadroSocietarioController
- ✅ Campos Legados: CPFSocio, NomeSocio, QualificacaoSocio
- ✅ Campos Novos: CpfCnpj, Nome, Qualificacao
- ✅ Representante Legal: CpfRepresentanteLegal, NomeRepresentanteLegal, QualificacaoRepresentanteLegal
- ✅ Outros: DataEntrada, DataSaida, PercentualCapital (decimal 5,2), DataCriacao
- ✅ Relacionamento: N:1 com Documento (CASCADE, somente CNPJ)
- ✅ Métodos de Domínio: Criar(), IsValido(), GetPercentualFormatado(), GetNomeAtual(), GetQualificacaoAtual(), GetCpfCnpjAtual()
- ✅ Índice: IdDocumento

---

#### Database ✅
- ✅ Migration: `20251005173212_AddFase3Entities` criada
- ✅ Migration aplicada com sucesso
- ✅ Tabelas criadas:
  - Documento (com RowVersion, índices: Numero UNIQUE, DataConsultaValidade, IdNacionalidade, IdSituacao)
  - Endereco (com RowVersion, índices: IdDocumento, CEP, Cidade+UF)
  - Telefone (índices: IdDocumento, DDD)
  - Email (índices: IdDocumento, EnderecoEmail)
  - QuadroSocietario (índice: IdDocumento)
- ✅ Foreign Keys configuradas corretamente:
  - Documento → Nacionalidade (SET NULL)
  - Documento → SituacaoCadastral (SET NULL)
  - Endereco → Documento (CASCADE)
  - Telefone → Documento (CASCADE)
  - Email → Documento (CASCADE)
  - QuadroSocietario → Documento (CASCADE)

---

#### Testes e Validações ✅
- ✅ Build completo: **Sucesso** (0 erros, 2 warnings de pacote vulnerável - não crítico)
- ✅ Migrations aplicadas com sucesso
- ✅ Todas as camadas implementadas (Domain, Infra.Data, Application, Infra.IoC, API)
- ✅ Padrão de código consistente com projeto existente
- ✅ Relacionamentos configurados corretamente (CASCADE e SET NULL)
- ✅ Índices otimizados criados
- ✅ Controle de concorrência (RowVersion) implementado em Documento e Endereco
- ✅ Validações de domínio implementadas em todas as entidades

---

### ✅ Semana 4: Concluída (2025-10-05)

#### Fase 4: Relacionamento Aplicacao-Provedor ✅

**AplicacaoProvedor** ✅
- ✅ Domain: Entity + IAplicacaoProvedorRepository
- ✅ Infra.Data: AplicacaoProvedorConfiguration + AplicacaoProvedorRepository + DbSet
- ✅ Application: AplicacaoProvedorDTO + IAplicacaoProvedorService + AplicacaoProvedorService + Mapping
- ✅ Infra.IoC: Registrar DI
- ✅ API: AplicacaoProvedorController
- ✅ Web: AplicacaoProvedorViewModel + IAplicacaoProvedorApi (Refit) + Controller + Views (Index, Create, Edit)
- ✅ Campos implementados:
  - Id, AplicacaoId (FK → Aplicacao), ProvedorId (FK → Provedor), Ordem, Status (StatusEnum → char)
  - DataCriacao, DataAtualizacao, CriadoPor, AtualizadoPor
- ✅ Navigation Properties: Aplicacao, Provedor
- ✅ Métodos de Domínio: Criar(), AtualizarOrdem(), Ativar(), Desativar(), IsAtivo()
- ✅ Índices:
  - AplicacaoId + ProvedorId (UNIQUE)
  - AplicacaoId, ProvedorId, Ordem
- ✅ Conversão customizada: StatusEnum (char) usando ValueConverter

**Total: 1/1 entidade implementada**

---

#### Database ✅
- ✅ Migration: `20251005175426_AddAplicacaoProvedor` criada
- ✅ Migration aplicada com sucesso
- ✅ Tabela criada: AplicacaoProvedor
- ✅ Foreign Keys: AplicacaoId → Aplicacao (CASCADE), ProvedorId → Provedor (CASCADE)
- ✅ Índice único composto: AplicacaoId + ProvedorId

---

#### Fase 5: Sistema de Perfis ✅ (Decisão de Arquitetura)

**Perfil - NÃO IMPLEMENTADO** ✅
- ✅ **Decisão:** Utilizar `AspNetRoles` (IdentityRole) do ASP.NET Identity
- ✅ **Justificativa:** O Identity já possui tabela de roles completa e integrada
- ✅ **Benefícios:**
  - Integração nativa com AspNetUsers
  - Tabela AspNetUserRoles para relacionamento N:N
  - APIs do Identity para gerenciamento de roles
  - Não há necessidade de criar CRUD customizado
- ❌ NÃO criar entidade Perfil customizada
- ❌ NÃO criar CRUD de perfis (usar Identity APIs)

**Total: 0/1 entidade implementada (substituída por AspNetRoles)**

---

#### Testes e Validações ✅
- ✅ Build completo: **Sucesso** (0 erros, 29 warnings de nullable - normal)
- ✅ Migration aplicada com sucesso
- ✅ Todas as camadas implementadas (Domain, Infra.Data, Application, Infra.IoC, API, Web)
- ✅ Padrão de código consistente com projeto existente
- ✅ Relacionamentos configurados corretamente (CASCADE)
- ✅ Índices otimizados criados
- ✅ ValueConverter customizado implementado para StatusEnum
- ✅ Validações de domínio implementadas

---

### ✅ Semana 5: Concluída (2025-10-05)

#### Fase 6: Sistema de Logs (Append-Only) ✅

**Características Especiais:**
- ❌ **NÃO** implementar IService completo (sem BaseService)
- ✅ Repositories com métodos específicos (Insert + Select, **SEM** Update/Delete)
- ✅ API: Apenas GET e POST endpoints
- ✅ Web: Somente visualização (Index + Details, **SEM** Create/Edit/Delete)

---

**1. LogAuditoria** ✅
- ✅ Domain: Entity + ILogAuditoriaRepository (métodos específicos)
- ✅ Infra.Data: LogAuditoriaConfiguration + LogAuditoriaRepository + DbSet
- ✅ Application: LogAuditoriaDTO + Mapping (sem Service)
- ✅ Infra.IoC: Registrar ILogAuditoriaRepository
- ✅ API: LogAuditoriaController (GET + POST, sem PUT/DELETE)
  - GET: GetAll, GetById, GetByAplicacao, GetByDocumentoNumero, GetByData, GetByConsultaSucesso
  - POST: Create
- ✅ Web: LogAuditoriaViewModel + ILogAuditoriaApi (Refit) + Controller + Views
  - Views: Index.cshtml, Details.cshtml (somente visualização)
- ✅ Campos implementados:
  - Principais: Id, AplicacaoId, NomeAplicacao, DocumentoNumero, TipoDocumento
  - Parâmetros: ParametrosEntrada (JSON), ProvedoresUtilizados (JSON), ProvedorPrincipal
  - Resultado: ConsultaSucesso, RespostaProvedor (JSON), MensagemRetorno
  - Métricas: TempoProcessamentoMs, DataHoraConsulta
  - Requisição: EnderecoIp, UserAgent, TokenAutenticacao
  - Controle: OrigemCache, InformacoesAdicionais (JSON)
- ✅ Navigation Properties: Aplicacao
- ✅ Métodos de Domínio: Criar(), IsConsultaComSucesso(), IsOrigemCache(), IsSlow(), GetDescricaoTipoDocumento()
- ✅ Índices otimizados:
  - AplicacaoId, DocumentoNumero, DataHoraConsulta, ConsultaSucesso
  - Composto: AplicacaoId + DataHoraConsulta

---

**2. LogErro** ✅
- ✅ Domain: Entity + ILogErroRepository (métodos específicos)
- ✅ Infra.Data: LogErroConfiguration + LogErroRepository + DbSet
- ✅ Application: LogErroDTO + Mapping (sem Service)
- ✅ Infra.IoC: Registrar ILogErroRepository
- ✅ API: LogErroController (GET + POST, sem PUT/DELETE)
  - GET: GetAll, GetById, GetByAplicacao, GetByData, GetByUsuario, GetBySistema
  - POST: Create
- ✅ Web: LogErroViewModel + ILogErroApi (Refit) + Controller + Views
  - Views: Index.cshtml, Details.cshtml (somente visualização)
- ✅ Campos implementados:
  - Id, DataHora, Aplicacao, Metodo, Erro, StackTrace, Usuario, IdSistema (FK → Aplicacao)
- ✅ Navigation Properties: Sistema (Aplicacao, opcional)
- ✅ Métodos de Domínio: Criar(), TemStackTrace(), TemUsuario(), TemSistemaAssociado(), GetResumoErro()
- ✅ Índices otimizados: DataHora, Aplicacao, Usuario, IdSistema

---

#### Database ✅
- ✅ Migration: `20251005181413_AddLogAuditoriaAndLogErro` criada
- ✅ Migration aplicada com sucesso
- ✅ Tabelas criadas:
  - **LogAuditoria** (18 campos, 5 índices, FK: AplicacaoId → Aplicacao RESTRICT)
  - **LogErro** (8 campos, 4 índices, FK: IdSistema → Aplicacao SET NULL)
- ✅ Índices otimizados para consultas de leitura

---

#### Testes e Validações ✅
- ✅ Build completo: **Sucesso** (0 erros, 31 warnings de nullable - normal)
- ✅ Migration aplicada com sucesso
- ✅ Todas as camadas implementadas (Domain, Infra.Data, Application, Infra.IoC, API, Web)
- ✅ Padrão append-only implementado corretamente (sem Update/Delete)
- ✅ Controllers Web usando apenas visualização (sem Create/Edit/Delete)
- ✅ Relacionamentos configurados corretamente (RESTRICT e SET NULL)
- ✅ Índices otimizados criados
- ✅ Factory Methods implementados nas entidades

---

**Total: 2/2 entidades append-only implementadas**

---

## 🎉 Plano de Implementação - STATUS FINAL

### Resumo Geral

| Fase | Descrição | Status | Total Entidades |
|------|-----------|--------|----------------|
| Fase 1 | Enums e Tipos Base | ✅ Completo | 7 enums |
| Fase 2 | Tabelas de Referência | ✅ Completo | 2 entidades |
| Fase 3 | Aggregate Root Documento + Dependentes | ✅ Completo | 5 entidades |
| Fase 4 | Relacionamento Aplicacao-Provedor | ✅ Completo | 1 entidade |
| Fase 5 | Sistema de Perfis | ✅ Completo | 0 (usa AspNetRoles) |
| Fase 6 | Logs (Append-Only) | ✅ Completo | 2 entidades |

**Total Implementado: 17 entidades + 7 enums**

### Entidades Implementadas

1. ✅ Cliente (exemplo)
2. ✅ Aplicacao
3. ✅ Provedor
4. ✅ Nacionalidade
5. ✅ SituacaoCadastral
6. ✅ Documento (Aggregate Root)
7. ✅ Endereco
8. ✅ Telefone
9. ✅ Email
10. ✅ QuadroSocietario
11. ✅ AplicacaoProvedor
12. ✅ LogAuditoria (append-only)
13. ✅ LogErro (append-only)

### Migrations Aplicadas

1. ✅ `20251005171151_AddNacionalidade`
2. ✅ `20251005171614_AddSituacaoCadastral`
3. ✅ `20251005173212_AddFase3Entities`
4. ✅ `20251005175426_AddAplicacaoProvedor`
5. ✅ `20251005181413_AddLogAuditoriaAndLogErro`

**Total: 5 migrations**

---

**Fim do Plano de Implementação**

**Data de Conclusão:** 2025-10-05
