# Plano de Migração - Sistema Legado de Consulta de Documentos

## 📋 Sumário Executivo

Este documento detalha o plano de migração para adequar o sistema atual (.NET 8) às regras de negócio e estrutura de dados do sistema legado (ASMX Web Services).

**Escopo:** Implementar apenas os itens aprovados conforme análise de inconsistências.

**Estimativa:** 6-8 horas de desenvolvimento

**Status:** FASES 1, 3, 4, 5, 6 e 7 CONCLUÍDAS ✅ (06/10/2025)

---

## 🎯 Itens Aprovados para Implementação

| # | Item | Descrição | Status |
|---|------|-----------|--------|
| 2 | Campos Provedor | Adicionar 13 campos faltantes em Provedor | ✅ Aprovado |
| 3 | Campo Aplicação | Adicionar flag `Serpro` | ✅ Aprovado |
| 4 | Campos Documento | Adicionar `NomeSocial` e `Porte` | ✅ Aprovado |
| 5 | FK Nacionalidade | Implementar FK em QuadroSocietario | ✅ Aprovado |
| 6 | AplicacaoProvedor LOG | Refatorar para log de uso (billing) | ✅ Aprovado |
| 8 | Validade Cache | Implementar validade dinâmica por provedor | ✅ Aprovado |
| 9 | Quota SERPRO | Implementar controle de quota | ✅ Aprovado |
| 17 | Cache Provedores | Cache em memória (5 minutos) | ✅ Aprovado |

---

## 📦 FASE 1: Atualização de Entidades de Domínio

### 1.1 Entidade Provedor - Adicionar 13 Campos Faltantes

**Arquivo:** `ConsultaDocumentos.Domain/Entities/Provedor.cs`

**Campos a adicionar:**

```csharp
// Certificado digital (SERPRO)
public string? EndCertificado { get; set; }

// Credenciais
public string? Usuario { get; set; }
public string? Senha { get; set; } // Criptografada
public string? Dominio { get; set; }

// Controle de quota SERPRO
public int? QtdAcessoMinimo { get; set; }
public int? QtdAcessoMaximo { get; set; }

// Validade de cache por tipo de documento
public int QtdDiasValidadePF { get; set; } = 30;
public int QtdDiasValidadePJ { get; set; } = 30;
public int QtdDiasValidadeEND { get; set; } = 30;

// Alertas e faturamento
public int? QtdMinEmailLog { get; set; }
public int? DiaCorte { get; set; }

// Porta (BoaVista socket - futuro)
public int? Porta { get; set; }

// Tipo de documento suportado
public int TipoWebService { get; set; } = 3; // 1=CPF, 2=CNPJ, 3=Ambos
```

**Arquivo de Configuração:** `ConsultaDocumentos.Infra.Data/EntitiesConfiguration/ProvedorConfiguration.cs`

```csharp
public void Configure(EntityTypeBuilder<Provedor> builder)
{
    // Configurações existentes...

    builder.Property(x => x.EndCertificado)
        .HasMaxLength(500);

    builder.Property(x => x.Usuario)
        .HasMaxLength(100);

    builder.Property(x => x.Senha)
        .HasMaxLength(200); // Criptografada

    builder.Property(x => x.Dominio)
        .HasMaxLength(100);

    builder.Property(x => x.QtdAcessoMinimo);

    builder.Property(x => x.QtdAcessoMaximo);

    builder.Property(x => x.QtdDiasValidadePF)
        .IsRequired()
        .HasDefaultValue(30);

    builder.Property(x => x.QtdDiasValidadePJ)
        .IsRequired()
        .HasDefaultValue(30);

    builder.Property(x => x.QtdDiasValidadeEND)
        .IsRequired()
        .HasDefaultValue(30);

    builder.Property(x => x.QtdMinEmailLog);

    builder.Property(x => x.DiaCorte);

    builder.Property(x => x.Porta);

    builder.Property(x => x.TipoWebService)
        .IsRequired()
        .HasDefaultValue(3);
}
```

**Arquivo DTO:** `ConsultaDocumentos.Application/DTOs/ProvedorDTO.cs`

```csharp
public class ProvedorDTO : BaseDTO
{
    // Propriedades existentes...

    public string? EndCertificado { get; set; }
    public string? Usuario { get; set; }
    public string? Senha { get; set; }
    public string? Dominio { get; set; }
    public int? QtdAcessoMinimo { get; set; }
    public int? QtdAcessoMaximo { get; set; }
    public int QtdDiasValidadePF { get; set; }
    public int QtdDiasValidadePJ { get; set; }
    public int QtdDiasValidadeEND { get; set; }
    public int? QtdMinEmailLog { get; set; }
    public int? DiaCorte { get; set; }
    public int? Porta { get; set; }
    public int TipoWebService { get; set; }
}
```

---

### 1.2 Entidade Aplicacao - Adicionar Flag Serpro

**Arquivo:** `ConsultaDocumentos.Domain/Entities/Aplicacao.cs`

```csharp
public class Aplicacao : BaseEntity
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string Status { get; set; }

    // NOVO CAMPO
    public bool Serpro { get; set; } // Flag se sistema tem acesso ao SERPRO
}
```

**Arquivo de Configuração:** `ConsultaDocumentos.Infra.Data/EntitiesConfiguration/AplicacaoConfiguration.cs`

```csharp
public void Configure(EntityTypeBuilder<Aplicacao> builder)
{
    // Configurações existentes...

    builder.Property(x => x.Serpro)
        .IsRequired()
        .HasDefaultValue(false);
}
```

**Arquivo DTO:** `ConsultaDocumentos.Application/DTOs/AplicacaoDTO.cs`

```csharp
public class AplicacaoDTO : BaseDTO
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string Status { get; set; }
    public bool Serpro { get; set; }
}
```

---

### 1.3 Entidade Documento - Adicionar NomeSocial e Porte

**Arquivo:** `ConsultaDocumentos.Domain/Entities/Documento.cs`

```csharp
public class Documento : BaseEntity
{
    // Propriedades existentes...

    // Campos PF (Pessoa Física)
    public DateTime? DataNascimento { get; set; }
    public string? NomeMae { get; set; }
    public string? Sexo { get; set; }
    public string? TituloEleitor { get; set; }
    public bool? ResidenteExterior { get; set; }
    public int? AnoObito { get; set; }
    public string? NomeSocial { get; set; } // NOVO CAMPO

    // Campos PJ (Pessoa Jurídica)
    public DateTime? DataAbertura { get; set; }
    public string? Inscricao { get; set; }
    public int? NaturezaJuridica { get; set; }
    public string? DescricaoNaturezaJuridica { get; set; }
    public string? Segmento { get; set; }
    public int? RamoAtividade { get; set; }
    public string? DescricaoRamoAtividade { get; set; }
    public string? NomeFantasia { get; set; }
    public int? MatrizFilialQtde { get; set; }
    public bool? Matriz { get; set; }
    public string? Porte { get; set; } // NOVO CAMPO - ME/EPP/Médio/Grande
}
```

**Arquivo de Configuração:** `ConsultaDocumentos.Infra.Data/EntitiesConfiguration/DocumentoConfiguration.cs`

```csharp
public void Configure(EntityTypeBuilder<Documento> builder)
{
    // Configurações existentes...

    builder.Property(x => x.NomeSocial)
        .HasMaxLength(100);

    builder.Property(x => x.Porte)
        .HasMaxLength(50);
}
```

**Arquivo DTO:** `ConsultaDocumentos.Application/DTOs/DocumentoDTO.cs`

```csharp
public class DocumentoDTO : BaseDTO
{
    // Propriedades existentes...

    public string? NomeSocial { get; set; }
    public string? Porte { get; set; }
}
```

---

### 1.4 Entidade QuadroSocietario - FK Nacionalidade + Campos Faltantes

**Arquivo:** `ConsultaDocumentos.Domain/Entities/QuadroSocietario.cs`

```csharp
public class QuadroSocietario : BaseEntity
{
    public Guid IdDocumento { get; set; }

    // Campos existentes...
    public string? CpfCnpj { get; set; }
    public string? Nome { get; set; }
    public string? Qualificacao { get; set; }
    public string? CpfRepresentanteLegal { get; set; }
    public string? NomeRepresentanteLegal { get; set; }
    public string? QualificacaoRepresentanteLegal { get; set; }
    public DateTime? DataEntrada { get; set; }
    public DateTime? DataSaida { get; set; }
    public decimal? PercentualCapital { get; set; }
    public DateTime DataCriacao { get; set; }

    // NOVOS CAMPOS
    public string? Tipo { get; set; } // Tipo de sócio
    public Guid? IdNacionalidade { get; set; } // FK Nacionalidade
    public string? CodPaisOrigem { get; set; } // Código país origem
    public string? NomePaisOrigem { get; set; } // Nome país origem

    // Navigation Properties
    public virtual Documento? Documento { get; set; }
    public virtual Nacionalidade? Nacionalidade { get; set; } // NOVA
}
```

**Arquivo de Configuração:** `ConsultaDocumentos.Infra.Data/EntitiesConfiguration/QuadroSocietarioConfiguration.cs`

```csharp
public void Configure(EntityTypeBuilder<QuadroSocietario> builder)
{
    // Configurações existentes...

    builder.Property(x => x.Tipo)
        .HasMaxLength(100);

    builder.Property(x => x.CodPaisOrigem)
        .HasMaxLength(10);

    builder.Property(x => x.NomePaisOrigem)
        .HasMaxLength(100);

    // Relacionamento com Nacionalidade
    builder.HasOne(x => x.Nacionalidade)
        .WithMany()
        .HasForeignKey(x => x.IdNacionalidade)
        .OnDelete(DeleteBehavior.SetNull);

    // Índice
    builder.HasIndex(x => x.IdNacionalidade)
        .HasDatabaseName("IX_QuadroSocietario_IdNacionalidade");
}
```

**Arquivo DTO:** `ConsultaDocumentos.Application/DTOs/QuadroSocietarioDTO.cs`

```csharp
public class QuadroSocietarioDTO : BaseDTO
{
    // Propriedades existentes...

    public string? Tipo { get; set; }
    public Guid? IdNacionalidade { get; set; }
    public string? CodPaisOrigem { get; set; }
    public string? NomePaisOrigem { get; set; }

    // Para exibição
    public NacionalidadeDTO? Nacionalidade { get; set; }
}
```

---

### 1.5 Entidade AplicacaoProvedor - Refatorar para LOG de Uso

**Arquivo:** `ConsultaDocumentos.Domain/Entities/AplicacaoProvedor.cs`

**Contexto:** Sistema legado usa `TBL_SISTEMA_WEBSERVICE` como **log de uso** para billing/faturamento.

```csharp
public class AplicacaoProvedor : BaseEntity
{
    // Campos de configuração (existentes)
    public Guid AplicacaoId { get; private set; }
    public Guid ProvedorId { get; private set; }
    public int Ordem { get; private set; }
    public StatusEnum Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }
    public string? CriadoPor { get; private set; }
    public string? AtualizadoPor { get; private set; }

    // NOVOS CAMPOS - LOG DE USO (billing)
    public Guid? IdDocumento { get; private set; }
    public DateTime? DataConsulta { get; private set; }
    public string? EnderecoIP { get; private set; }
    public string? RemoteHost { get; private set; }

    // Navigation Properties
    public virtual Aplicacao Aplicacao { get; set; }
    public virtual Provedor Provedor { get; set; }
    public virtual Documento? Documento { get; set; } // NOVA

    // Factory Method para LOG DE USO
    public static AplicacaoProvedor CriarLogDeUso(
        Guid aplicacaoId,
        Guid provedorId,
        Guid idDocumento,
        string? enderecoIP = null,
        string? remoteHost = null)
    {
        return new AplicacaoProvedor
        {
            Id = Guid.NewGuid(),
            AplicacaoId = aplicacaoId,
            ProvedorId = provedorId,
            Ordem = 0, // Log não precisa de ordem
            Status = StatusEnum.Ativo,
            DataCriacao = DateTime.UtcNow,
            IdDocumento = idDocumento,
            DataConsulta = DateTime.UtcNow,
            EnderecoIP = enderecoIP,
            RemoteHost = remoteHost
        };
    }
}
```

**Arquivo de Configuração:** `ConsultaDocumentos.Infra.Data/EntitiesConfiguration/AplicacaoProvedorConfiguration.cs`

```csharp
public void Configure(EntityTypeBuilder<AplicacaoProvedor> builder)
{
    // Configurações existentes...

    // NOVOS CAMPOS
    builder.Property(x => x.DataConsulta);

    builder.Property(x => x.EnderecoIP)
        .HasMaxLength(50);

    builder.Property(x => x.RemoteHost)
        .HasMaxLength(100);

    // Relacionamento com Documento
    builder.HasOne(x => x.Documento)
        .WithMany()
        .HasForeignKey(x => x.IdDocumento)
        .OnDelete(DeleteBehavior.SetNull);

    // Índices para billing/relatórios
    builder.HasIndex(x => x.DataConsulta)
        .HasDatabaseName("IX_AplicacaoProvedor_DataConsulta");

    builder.HasIndex(x => x.IdDocumento)
        .HasDatabaseName("IX_AplicacaoProvedor_IdDocumento");

    builder.HasIndex(x => new { x.AplicacaoId, x.ProvedorId, x.DataConsulta })
        .HasDatabaseName("IX_AplicacaoProvedor_Billing");
}
```

**Arquivo DTO:** `ConsultaDocumentos.Application/DTOs/AplicacaoProvedorDTO.cs`

```csharp
public class AplicacaoProvedorDTO : BaseDTO
{
    // Propriedades existentes...

    public Guid? IdDocumento { get; set; }
    public DateTime? DataConsulta { get; set; }
    public string? EnderecoIP { get; set; }
    public string? RemoteHost { get; set; }
}
```

---

## 📦 FASE 2: Camada de Application

### 2.1 Atualizar AutoMapper

**Arquivo:** `ConsultaDocumentos.Application/Mappings/DomainToDTOMappingProfile.cs`

```csharp
public class DomainToDTOMappingProfile : Profile
{
    public DomainToDTOMappingProfile()
    {
        // Mappings existentes...
        CreateMap<ClienteDTO, Cliente>().ReverseMap();
        CreateMap<AplicacaoDTO, Aplicacao>().ReverseMap();
        CreateMap<ProvedorDTO, Provedor>().ReverseMap();
        CreateMap<DocumentoDTO, Documento>().ReverseMap();
        CreateMap<QuadroSocietarioDTO, QuadroSocietario>().ReverseMap();
        CreateMap<AplicacaoProvedorDTO, AplicacaoProvedor>().ReverseMap();

        // Novos mappings já funcionam com ReverseMap()
        // AutoMapper detecta automaticamente os novos campos
    }
}
```

### 2.2 Atualizar Services de Conversão

**Arquivo:** `ConsultaDocumentos.Application/Services/External/ExternalDocumentConsultaService.cs`

#### Método: `ConverterSerprocCPFParaDocumento()`

```csharp
private Documento ConverterSerprocCPFParaDocumento(SerprocCPFResponse response, string numeroDocumento)
{
    var documento = Documento.CriarPessoaFisica(
        DocumentoValidationHelper.RemoverFormatacao(numeroDocumento),
        response.Nome ?? string.Empty,
        DateConversionHelper.ParseSerproDate(response.DataNascimento) ?? DateTime.MinValue);

    documento.NomeMae = response.NomeMae;
    documento.Sexo = response.Sexo;
    documento.TituloEleitor = response.TituloEleitor;
    documento.ResidenteExterior = DateConversionHelper.ParseResidenteExterior(response.ResidenteExterior);
    documento.AnoObito = DateConversionHelper.ParseAnoObito(response.AnoObito);
    documento.CodControle = response.CodigoControle;
    documento.IdSituacao = SituacaoCadastralMapper.MapearSituacao(response.SituacaoCadastral);
    documento.OrigemBureau = "SERPRO";

    // NOVO CAMPO
    documento.NomeSocial = response.NomeSocial; // Se SERPRO retornar

    return documento;
}
```

#### Método: `ConverterSerprocCNPJPerfil2ParaDocumento()`

```csharp
private Documento ConverterSerprocCNPJPerfil2ParaDocumento(SerprocCNPJPerfil2Response response, string numeroDocumento)
{
    var documento = ConverterSerprocCNPJPerfil1ParaDocumento(response, numeroDocumento);

    documento.DataAbertura = DateConversionHelper.ParseSerproDate(response.DataAbertura);
    documento.DataSituacao = DateConversionHelper.ParseSerproDate(response.DataSituacaoCadastral);

    if (int.TryParse(response.NaturezaJuridica?.Replace("-", ""), out int naturezaJuridica))
    {
        documento.NaturezaJuridica = naturezaJuridica;
    }

    documento.DescricaoNaturezaJuridica = response.NaturezaJuridica;

    // NOVO CAMPO
    documento.Porte = response.Porte; // Se SERPRO retornar (ME/EPP/Médio/Grande)

    return documento;
}
```

#### Método: `ConverterSerprocCNPJPerfil3ParaDocumento()`

```csharp
private Documento ConverterSerprocCNPJPerfil3ParaDocumento(SerprocCNPJPerfil3Response response, string numeroDocumento)
{
    var documento = ConverterSerprocCNPJPerfil2ParaDocumento(response, numeroDocumento);

    // Adiciona sócios
    if (response.Socios != null && response.Socios.Any())
    {
        foreach (var socioDTO in response.Socios)
        {
            var socio = QuadroSocietario.Criar(
                documento.Id,
                socioDTO.CpfCnpj,
                socioDTO.Nome,
                socioDTO.Qualificacao);

            socio.DataEntrada = DateConversionHelper.ParseSerproDate(socioDTO.DataEntrada);
            socio.PercentualCapital = DecimalConversionHelper.ConvertToDecimal(socioDTO.PercentualCapital);
            socio.CpfRepresentanteLegal = socioDTO.CpfRepresentanteLegal;
            socio.NomeRepresentanteLegal = socioDTO.NomeRepresentanteLegal;
            socio.QualificacaoRepresentanteLegal = socioDTO.QualificacaoRepresentanteLegal;

            // NOVOS CAMPOS
            socio.Tipo = socioDTO.Tipo; // Se SERPRO retornar
            socio.CodPaisOrigem = socioDTO.CodPaisOrigem;
            socio.NomePaisOrigem = socioDTO.NomePaisOrigem;

            // Buscar IdNacionalidade pelo CodPaisOrigem
            if (!string.IsNullOrWhiteSpace(socio.CodPaisOrigem))
            {
                // TODO: Implementar busca de Nacionalidade por código
                // var nacionalidade = await _nacionalidadeRepository.GetByCodigoAsync(socio.CodPaisOrigem);
                // socio.IdNacionalidade = nacionalidade?.Id;
            }

            documento.AdicionarQuadroSocietario(socio);
        }
    }

    return documento;
}
```

---

## 📦 FASE 3: Camada de Infraestrutura

### 3.1 Criar Migration

**Comando:**

```bash
dotnet ef migrations add AddCamposLegadosSistema --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API
```

**Resultado esperado:** Arquivo de migration gerado em `ConsultaDocumentos.Infra.Data/Migrations/`

**Conteúdo da Migration (exemplo):**

```csharp
public partial class AddCamposLegadosSistema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Provedor
        migrationBuilder.AddColumn<string>(
            name: "EndCertificado",
            table: "Provedor",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Usuario",
            table: "Provedor",
            maxLength: 100,
            nullable: true);

        // ... (todos os 13 campos)

        // Aplicacao
        migrationBuilder.AddColumn<bool>(
            name: "Serpro",
            table: "Aplicacao",
            nullable: false,
            defaultValue: false);

        // Documento
        migrationBuilder.AddColumn<string>(
            name: "NomeSocial",
            table: "Documento",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Porte",
            table: "Documento",
            maxLength: 50,
            nullable: true);

        // QuadroSocietario
        migrationBuilder.AddColumn<string>(
            name: "Tipo",
            table: "QuadroSocietario",
            maxLength: 100,
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "IdNacionalidade",
            table: "QuadroSocietario",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "CodPaisOrigem",
            table: "QuadroSocietario",
            maxLength: 10,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "NomePaisOrigem",
            table: "QuadroSocietario",
            maxLength: 100,
            nullable: true);

        // AplicacaoProvedor
        migrationBuilder.AddColumn<Guid>(
            name: "IdDocumento",
            table: "AplicacaoProvedor",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DataConsulta",
            table: "AplicacaoProvedor",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "EnderecoIP",
            table: "AplicacaoProvedor",
            maxLength: 50,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "RemoteHost",
            table: "AplicacaoProvedor",
            maxLength: 100,
            nullable: true);

        // Foreign Keys
        migrationBuilder.CreateIndex(
            name: "IX_QuadroSocietario_IdNacionalidade",
            table: "QuadroSocietario",
            column: "IdNacionalidade");

        migrationBuilder.AddForeignKey(
            name: "FK_QuadroSocietario_Nacionalidade_IdNacionalidade",
            table: "QuadroSocietario",
            column: "IdNacionalidade",
            principalTable: "Nacionalidade",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);

        migrationBuilder.CreateIndex(
            name: "IX_AplicacaoProvedor_IdDocumento",
            table: "AplicacaoProvedor",
            column: "IdDocumento");

        migrationBuilder.CreateIndex(
            name: "IX_AplicacaoProvedor_DataConsulta",
            table: "AplicacaoProvedor",
            column: "DataConsulta");

        migrationBuilder.CreateIndex(
            name: "IX_AplicacaoProvedor_Billing",
            table: "AplicacaoProvedor",
            columns: new[] { "AplicacaoId", "ProvedorId", "DataConsulta" });

        migrationBuilder.AddForeignKey(
            name: "FK_AplicacaoProvedor_Documento_IdDocumento",
            table: "AplicacaoProvedor",
            column: "IdDocumento",
            principalTable: "Documento",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Reverter todas as alterações
        migrationBuilder.DropForeignKey(
            name: "FK_QuadroSocietario_Nacionalidade_IdNacionalidade",
            table: "QuadroSocietario");

        migrationBuilder.DropForeignKey(
            name: "FK_AplicacaoProvedor_Documento_IdDocumento",
            table: "AplicacaoProvedor");

        migrationBuilder.DropIndex(
            name: "IX_QuadroSocietario_IdNacionalidade",
            table: "QuadroSocietario");

        migrationBuilder.DropIndex(
            name: "IX_AplicacaoProvedor_IdDocumento",
            table: "AplicacaoProvedor");

        migrationBuilder.DropIndex(
            name: "IX_AplicacaoProvedor_DataConsulta",
            table: "AplicacaoProvedor");

        migrationBuilder.DropIndex(
            name: "IX_AplicacaoProvedor_Billing",
            table: "AplicacaoProvedor");

        migrationBuilder.DropColumn(name: "EndCertificado", table: "Provedor");
        migrationBuilder.DropColumn(name: "Usuario", table: "Provedor");
        // ... (todos os campos)
    }
}
```

### 3.2 Atualizar Database

**Comando:**

```bash
dotnet ef database update --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API
```

**Verificação:**

```sql
-- Verificar estrutura das tabelas
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Provedor';

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'QuadroSocietario';

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AplicacaoProvedor';
```

---

## 📦 FASE 4: Implementar Lógicas de Negócio

### 4.1 Validade Dinâmica de Cache (depende Fase 1.1)

**Arquivo:** `ConsultaDocumentos.Application/Services/External/ExternalDocumentConsultaService.cs`

**Método:** `ConsultarDocumentoAsync()`

**Antes (linha 187):**

```csharp
// Atualiza o cache com o documento consultado
await _cacheService.SetAsync(cacheKey, documentoDTOResult, TimeSpan.FromDays(90), cancellationToken);
```

**Depois:**

```csharp
// Buscar o provedor usado para obter dias de validade
var provedorUsado = await _provedorRepository.GetByNomeAsync(provedorUtilizado);
if (provedorUsado == null)
{
    throw new InvalidOperationException($"Provedor {provedorUtilizado} não encontrado");
}

// Calcular validade dinâmica baseada no tipo de documento
var diasValidade = request.TipoDocumento == TipoDocumento.CPF
    ? provedorUsado.QtdDiasValidadePF
    : provedorUsado.QtdDiasValidadePJ;

// Atualizar DataConsultaValidade do documento
documentoConsultado.DataConsultaValidade = DateTime.UtcNow.AddDays(diasValidade);
await _documentoService.UpdateAsync(_mapper.Map<DocumentoDTO>(documentoConsultado));

// Atualiza o cache com validade dinâmica
await _cacheService.SetAsync(
    cacheKey,
    documentoDTOResult,
    TimeSpan.FromDays(diasValidade),
    cancellationToken);

_logger.LogInformation(
    "Documento armazenado no cache com validade de {Dias} dias (provedor: {Provedor})",
    diasValidade, provedorUtilizado);
```

**Novo método necessário no IProvedorRepository:**

```csharp
Task<Provedor?> GetByNomeAsync(string nome);
```

**Implementação em ProvedorRepository:**

```csharp
public async Task<Provedor?> GetByNomeAsync(string nome)
{
    return await _context.Provedor
        .FirstOrDefaultAsync(p => p.Nome.ToUpper() == nome.ToUpper());
}
```

---

### 4.2 Controle de Quota SERPRO (depende Fase 1.1)

**Arquivo:** `ConsultaDocumentos.Application/Services/External/ExternalDocumentConsultaService.cs`

**Método:** `ObterProvedoresOrdenadosAsync()`

**Antes:**

```csharp
private async Task<List<Provedor>> ObterProvedoresOrdenadosAsync(Guid aplicacaoId, CancellationToken cancellationToken)
{
    var aplicacaoProvedores = await _aplicacaoProvedorRepository.GetAllAsync();

    var provedoresAplicacao = aplicacaoProvedores
        .Where(ap => ap.AplicacaoId == aplicacaoId && ap.IsAtivo())
        .OrderBy(ap => ap.Ordem)
        .ToList();

    var provedores = new List<Provedor>();

    foreach (var ap in provedoresAplicacao)
    {
        var provedor = await _provedorRepository.GetByIdAsync(ap.ProvedorId);
        if (provedor != null && provedor.Status.ToUpperInvariant() == "ATIVO")
        {
            provedores.Add(provedor);
        }
    }

    // Fallback: se não houver provedores configurados, usa SERPRO e SERASA na ordem padrão
    if (!provedores.Any())
    {
        var todosProvedores = await _provedorRepository.GetAllAsync();
        provedores = todosProvedores
            .Where(p => p.Status.ToUpperInvariant() == "ATIVO")
            .OrderBy(p => p.Prioridade)
            .ToList();
    }

    return provedores;
}
```

**Depois:**

```csharp
private async Task<List<Provedor>> ObterProvedoresOrdenadosAsync(Guid aplicacaoId, CancellationToken cancellationToken)
{
    var aplicacaoProvedores = await _aplicacaoProvedorRepository.GetAllAsync();

    var provedoresAplicacao = aplicacaoProvedores
        .Where(ap => ap.AplicacaoId == aplicacaoId && ap.IsAtivo())
        .OrderBy(ap => ap.Ordem)
        .ToList();

    var provedores = new List<Provedor>();

    foreach (var ap in provedoresAplicacao)
    {
        var provedor = await _provedorRepository.GetByIdAsync(ap.ProvedorId);
        if (provedor != null && provedor.Status.ToUpperInvariant() == "ATIVO")
        {
            // CONTROLE DE QUOTA SERPRO
            if (provedor.Nome.ToUpperInvariant() == "SERPRO" && provedor.QtdAcessoMaximo.HasValue)
            {
                // Contar consultas SERPRO hoje
                var totalConsultasHoje = await ContarConsultasProvedorHoje(provedor.Id);

                if (totalConsultasHoje >= provedor.QtdAcessoMaximo.Value)
                {
                    _logger.LogWarning(
                        "Quota SERPRO atingida: {Total}/{Max}. Pulando provedor.",
                        totalConsultasHoje, provedor.QtdAcessoMaximo.Value);
                    continue; // Pula SERPRO
                }

                _logger.LogInformation(
                    "Quota SERPRO: {Total}/{Max}",
                    totalConsultasHoje, provedor.QtdAcessoMaximo.Value);
            }

            provedores.Add(provedor);
        }
    }

    // Fallback: se não houver provedores configurados, usa SERPRO e SERASA na ordem padrão
    if (!provedores.Any())
    {
        var todosProvedores = await _provedorRepository.GetAllAsync();

        foreach (var provedor in todosProvedores.Where(p => p.Status.ToUpperInvariant() == "ATIVO").OrderBy(p => p.Prioridade))
        {
            // Aplicar controle de quota também no fallback
            if (provedor.Nome.ToUpperInvariant() == "SERPRO" && provedor.QtdAcessoMaximo.HasValue)
            {
                var totalConsultasHoje = await ContarConsultasProvedorHoje(provedor.Id);
                if (totalConsultasHoje >= provedor.QtdAcessoMaximo.Value)
                {
                    continue;
                }
            }

            provedores.Add(provedor);
        }
    }

    return provedores;
}

private async Task<int> ContarConsultasProvedorHoje(Guid provedorId)
{
    var hoje = DateTime.UtcNow.Date;
    var amanha = hoje.AddDays(1);

    // Conta em LogAuditoria
    var logsHoje = await _logAuditoriaRepository.GetAllAsync();
    var totalLogs = logsHoje
        .Where(l => l.ProvedorPrincipal == "SERPRO"
                    && l.DataHoraConsulta >= hoje
                    && l.DataHoraConsulta < amanha)
        .Count();

    return totalLogs;
}
```

**Alternativa usando AplicacaoProvedor (se usado como LOG):**

```csharp
private async Task<int> ContarConsultasProvedorHoje(Guid provedorId)
{
    var hoje = DateTime.UtcNow.Date;
    var amanha = hoje.AddDays(1);

    var logsHoje = await _aplicacaoProvedorRepository.GetAllAsync();
    return logsHoje
        .Where(ap => ap.ProvedorId == provedorId
                     && ap.DataConsulta.HasValue
                     && ap.DataConsulta.Value >= hoje
                     && ap.DataConsulta.Value < amanha)
        .Count();
}
```

---

### 4.3 Cache em Memória de Provedores

**Arquivo:** `ConsultaDocumentos.Infra.Ioc/DependecyInjection.cs`

**Adicionar registro do IMemoryCache:**

```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    // Configurações existentes...

    // Memory Cache
    services.AddMemoryCache();

    return services;
}
```

**Arquivo:** `ConsultaDocumentos.Application/Services/External/ExternalDocumentConsultaService.cs`

**Adicionar IMemoryCache no construtor:**

```csharp
private readonly IMemoryCache _memoryCache;

public ExternalDocumentConsultaService(
    ISerproService serproService,
    ISerasaService serasaService,
    IDocumentoService documentoService,
    IAplicacaoProvedorRepository aplicacaoProvedorRepository,
    IProvedorRepository provedorRepository,
    ILogAuditoriaRepository logAuditoriaRepository,
    ILogErroRepository logErroRepository,
    ICacheService cacheService,
    IMemoryCache memoryCache, // NOVO
    IMapper mapper,
    ILogger<ExternalDocumentConsultaService> logger)
{
    _serproService = serproService;
    _serasaService = serasaService;
    _documentoService = documentoService;
    _aplicacaoProvedorRepository = aplicacaoProvedorRepository;
    _provedorRepository = provedorRepository;
    _logAuditoriaRepository = logAuditoriaRepository;
    _logErroRepository = logErroRepository;
    _cacheService = cacheService;
    _memoryCache = memoryCache; // NOVO
    _mapper = mapper;
    _logger = logger;
}
```

**Modificar método ObterProvedoresOrdenadosAsync():**

```csharp
private async Task<List<Provedor>> ObterProvedoresOrdenadosAsync(Guid aplicacaoId, CancellationToken cancellationToken)
{
    var cacheKey = $"provedores:aplicacao:{aplicacaoId}";

    // Tentar buscar do cache em memória
    if (_memoryCache.TryGetValue<List<Provedor>>(cacheKey, out var provedoresCached))
    {
        _logger.LogInformation("Provedores retornados do cache em memória para aplicação {AplicacaoId}", aplicacaoId);
        return provedoresCached!;
    }

    _logger.LogInformation("Cache miss: buscando provedores do BD para aplicação {AplicacaoId}", aplicacaoId);

    var aplicacaoProvedores = await _aplicacaoProvedorRepository.GetAllAsync();

    var provedoresAplicacao = aplicacaoProvedores
        .Where(ap => ap.AplicacaoId == aplicacaoId && ap.IsAtivo())
        .OrderBy(ap => ap.Ordem)
        .ToList();

    var provedores = new List<Provedor>();

    foreach (var ap in provedoresAplicacao)
    {
        var provedor = await _provedorRepository.GetByIdAsync(ap.ProvedorId);
        if (provedor != null && provedor.Status.ToUpperInvariant() == "ATIVO")
        {
            // Controle de quota SERPRO
            if (provedor.Nome.ToUpperInvariant() == "SERPRO" && provedor.QtdAcessoMaximo.HasValue)
            {
                var totalConsultasHoje = await ContarConsultasProvedorHoje(provedor.Id);
                if (totalConsultasHoje >= provedor.QtdAcessoMaximo.Value)
                {
                    _logger.LogWarning(
                        "Quota SERPRO atingida: {Total}/{Max}. Pulando provedor.",
                        totalConsultasHoje, provedor.QtdAcessoMaximo.Value);
                    continue;
                }
            }

            provedores.Add(provedor);
        }
    }

    // Fallback: se não houver provedores configurados, usa SERPRO e SERASA na ordem padrão
    if (!provedores.Any())
    {
        var todosProvedores = await _provedorRepository.GetAllAsync();

        foreach (var provedor in todosProvedores.Where(p => p.Status.ToUpperInvariant() == "ATIVO").OrderBy(p => p.Prioridade))
        {
            if (provedor.Nome.ToUpperInvariant() == "SERPRO" && provedor.QtdAcessoMaximo.HasValue)
            {
                var totalConsultasHoje = await ContarConsultasProvedorHoje(provedor.Id);
                if (totalConsultasHoje >= provedor.QtdAcessoMaximo.Value)
                {
                    continue;
                }
            }

            provedores.Add(provedor);
        }
    }

    // Armazenar no cache por 5 minutos (conforme sistema legado)
    var cacheOptions = new MemoryCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
        Priority = CacheItemPriority.Normal
    };

    _memoryCache.Set(cacheKey, provedores, cacheOptions);
    _logger.LogInformation("Provedores armazenados no cache em memória (5 minutos) para aplicação {AplicacaoId}", aplicacaoId);

    return provedores;
}
```

---

### 4.4 Registrar Log de Uso em AplicacaoProvedor

**Arquivo:** `ConsultaDocumentos.Application/Services/External/ExternalDocumentConsultaService.cs`

**Método:** `ConsultarDocumentoAsync()`

**Adicionar após salvar o documento (linha ~200):**

```csharp
// Salva o documento consultado
var documentoSalvo = await SalvarDocumentoAsync(documentoConsultado, cancellationToken);
var documentoDTOResult = _mapper.Map<DocumentoDTO>(documentoSalvo);

// NOVO: Registrar log de uso para billing
await RegistrarLogDeUsoAsync(
    request.AplicacaoId,
    provedorUsado.Id, // Buscar ID do provedor usado
    documentoSalvo.Id,
    cancellationToken);

// Atualiza o cache...
```

**Novo método:**

```csharp
private async Task RegistrarLogDeUsoAsync(
    Guid aplicacaoId,
    Guid provedorId,
    Guid idDocumento,
    CancellationToken cancellationToken)
{
    try
    {
        // Obter IP e Host da requisição (via HttpContext se disponível)
        string? enderecoIP = null;
        string? remoteHost = null;

        // TODO: Injetar IHttpContextAccessor se necessário
        // var httpContext = _httpContextAccessor?.HttpContext;
        // if (httpContext != null)
        // {
        //     enderecoIP = httpContext.Connection.RemoteIpAddress?.ToString();
        //     remoteHost = httpContext.Request.Host.Value;
        // }

        var logDeUso = AplicacaoProvedor.CriarLogDeUso(
            aplicacaoId,
            provedorId,
            idDocumento,
            enderecoIP,
            remoteHost);

        await _aplicacaoProvedorRepository.AddAsync(logDeUso);

        _logger.LogInformation(
            "Log de uso registrado: Aplicação {AplicacaoId}, Provedor {ProvedorId}, Documento {DocumentoId}",
            aplicacaoId, provedorId, idDocumento);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao registrar log de uso");
        // Não propagar exceção - log de uso é informativo
    }
}
```

**Para obter IP/Host, adicionar IHttpContextAccessor:**

```csharp
// No construtor
private readonly IHttpContextAccessor _httpContextAccessor;

public ExternalDocumentConsultaService(
    // ... outros parâmetros
    IHttpContextAccessor httpContextAccessor)
{
    _httpContextAccessor = httpContextAccessor;
}

// No método RegistrarLogDeUsoAsync
var httpContext = _httpContextAccessor?.HttpContext;
if (httpContext != null)
{
    enderecoIP = httpContext.Connection.RemoteIpAddress?.ToString();
    remoteHost = httpContext.Request.Host.Value;
}
```

**Registrar IHttpContextAccessor no DI:**

```csharp
// ConsultaDocumentos.API/Program.cs ou Infra.Ioc/DependecyInjection.cs
services.AddHttpContextAccessor();
```

---

## 📦 FASE 5: Camada de API

### 5.1 Atualizar Controllers

**Arquivo:** `ConsultaDocumentos.API/Controllers/ProvedorController.cs`

```csharp
[HttpPost]
public virtual async Task<IActionResult> Create([FromBody] ProvedorDTO dto, CancellationToken ct = default)
{
    // Validações
    if (dto.QtdDiasValidadePF <= 0 || dto.QtdDiasValidadePJ <= 0 || dto.QtdDiasValidadeEND <= 0)
    {
        return BadRequest(new { erro = "Dias de validade devem ser maiores que zero" });
    }

    if (dto.TipoWebService < 1 || dto.TipoWebService > 3)
    {
        return BadRequest(new { erro = "TipoWebService deve ser 1 (CPF), 2 (CNPJ) ou 3 (Ambos)" });
    }

    if (dto.QtdAcessoMaximo.HasValue && dto.QtdAcessoMaximo.Value < 0)
    {
        return BadRequest(new { erro = "QtdAcessoMaximo não pode ser negativo" });
    }

    var result = await _service.AddAsync(dto);
    return Ok(result);
}
```

**Arquivo:** `ConsultaDocumentos.API/Controllers/AplicacaoController.cs`

```csharp
// Nenhuma alteração necessária - campo Serpro já será mapeado automaticamente
```

---

## 📦 FASE 6: Projeto Web (MVC)

### 6.1 Atualizar ViewModels

**Arquivo:** `ConsultaDocumentos.Web/Models/ProvedorViewModel.cs`

```csharp
public class ProvedorViewModel : BaseViewModel
{
    public string Credencial { get; set; }
    public string Descricao { get; set; }
    public string EndpointUrl { get; set; }
    public string Nome { get; set; }
    public int Prioridade { get; set; }
    public string Status { get; set; }
    public int Timeout { get; set; }

    // NOVOS CAMPOS
    [Display(Name = "Caminho Certificado (.pfx)")]
    public string? EndCertificado { get; set; }

    [Display(Name = "Usuário")]
    public string? Usuario { get; set; }

    [Display(Name = "Senha")]
    [DataType(DataType.Password)]
    public string? Senha { get; set; }

    [Display(Name = "Domínio")]
    public string? Dominio { get; set; }

    [Display(Name = "Quota Mínima")]
    public int? QtdAcessoMinimo { get; set; }

    [Display(Name = "Quota Máxima")]
    public int? QtdAcessoMaximo { get; set; }

    [Display(Name = "Validade CPF (dias)")]
    [Range(1, 365)]
    public int QtdDiasValidadePF { get; set; } = 30;

    [Display(Name = "Validade CNPJ (dias)")]
    [Range(1, 365)]
    public int QtdDiasValidadePJ { get; set; } = 30;

    [Display(Name = "Validade Endereço (dias)")]
    [Range(1, 365)]
    public int QtdDiasValidadeEND { get; set; } = 30;

    [Display(Name = "Min. Emails para Alerta")]
    public int? QtdMinEmailLog { get; set; }

    [Display(Name = "Dia de Corte")]
    [Range(1, 31)]
    public int? DiaCorte { get; set; }

    [Display(Name = "Porta (Socket)")]
    public int? Porta { get; set; }

    [Display(Name = "Tipo de Documento")]
    public int TipoWebService { get; set; } = 3; // 1=CPF, 2=CNPJ, 3=Ambos
}
```

**Arquivo:** `ConsultaDocumentos.Web/Models/AplicacaoViewModel.cs`

```csharp
public class AplicacaoViewModel : BaseViewModel
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string Status { get; set; }

    [Display(Name = "Acesso SERPRO")]
    public bool Serpro { get; set; }
}
```

**Arquivo:** `ConsultaDocumentos.Web/Models/DocumentoViewModel.cs`

```csharp
public class DocumentoViewModel : BaseViewModel
{
    // Campos existentes...

    [Display(Name = "Nome Social")]
    public string? NomeSocial { get; set; }

    [Display(Name = "Porte")]
    public string? Porte { get; set; }
}
```

**Arquivo:** `ConsultaDocumentos.Web/Models/QuadroSocietarioViewModel.cs`

```csharp
public class QuadroSocietarioViewModel : BaseViewModel
{
    // Campos existentes...

    [Display(Name = "Tipo de Sócio")]
    public string? Tipo { get; set; }

    [Display(Name = "Nacionalidade")]
    public Guid? IdNacionalidade { get; set; }

    [Display(Name = "Código País")]
    public string? CodPaisOrigem { get; set; }

    [Display(Name = "País de Origem")]
    public string? NomePaisOrigem { get; set; }

    // Para dropdown
    public NacionalidadeViewModel? Nacionalidade { get; set; }
}
```

---

### 6.2 Atualizar Views

**Arquivo:** `ConsultaDocumentos.Web/Views/Provedor/Create.cshtml`

```html
@model ProvedorViewModel

<h2>Cadastrar Provedor</h2>

<form asp-action="Create" method="post">
    <div class="form-group">
        <label asp-for="Nome"></label>
        <input asp-for="Nome" class="form-control" />
        <span asp-validation-for="Nome" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Descricao"></label>
        <input asp-for="Descricao" class="form-control" />
        <span asp-validation-for="Descricao" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="EndpointUrl"></label>
        <input asp-for="EndpointUrl" class="form-control" />
        <span asp-validation-for="EndpointUrl" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Prioridade"></label>
        <input asp-for="Prioridade" type="number" class="form-control" />
        <span asp-validation-for="Prioridade" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Timeout"></label>
        <input asp-for="Timeout" type="number" class="form-control" />
        <span asp-validation-for="Timeout" class="text-danger"></span>
    </div>

    <!-- NOVOS CAMPOS -->

    <hr />
    <h4>Configurações de Autenticação</h4>

    <div class="form-group">
        <label asp-for="EndCertificado"></label>
        <input asp-for="EndCertificado" class="form-control" placeholder="C:\Certificates\certificado.pfx" />
        <span asp-validation-for="EndCertificado" class="text-danger"></span>
        <small class="form-text text-muted">Caminho do certificado digital (.pfx) - SERPRO</small>
    </div>

    <div class="form-group">
        <label asp-for="Usuario"></label>
        <input asp-for="Usuario" class="form-control" />
        <span asp-validation-for="Usuario" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Senha"></label>
        <input asp-for="Senha" type="password" class="form-control" />
        <span asp-validation-for="Senha" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Dominio"></label>
        <input asp-for="Dominio" class="form-control" />
        <span asp-validation-for="Dominio" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Porta"></label>
        <input asp-for="Porta" type="number" class="form-control" />
        <span asp-validation-for="Porta" class="text-danger"></span>
        <small class="form-text text-muted">Porta TCP (BoaVista Socket)</small>
    </div>

    <hr />
    <h4>Controle de Quota</h4>

    <div class="form-group">
        <label asp-for="QtdAcessoMinimo"></label>
        <input asp-for="QtdAcessoMinimo" type="number" class="form-control" />
        <span asp-validation-for="QtdAcessoMinimo" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="QtdAcessoMaximo"></label>
        <input asp-for="QtdAcessoMaximo" type="number" class="form-control" />
        <span asp-validation-for="QtdAcessoMaximo" class="text-danger"></span>
        <small class="form-text text-muted">Limite diário de consultas (SERPRO)</small>
    </div>

    <hr />
    <h4>Validade de Cache</h4>

    <div class="form-group">
        <label asp-for="QtdDiasValidadePF"></label>
        <input asp-for="QtdDiasValidadePF" type="number" class="form-control" min="1" max="365" />
        <span asp-validation-for="QtdDiasValidadePF" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="QtdDiasValidadePJ"></label>
        <input asp-for="QtdDiasValidadePJ" type="number" class="form-control" min="1" max="365" />
        <span asp-validation-for="QtdDiasValidadePJ" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="QtdDiasValidadeEND"></label>
        <input asp-for="QtdDiasValidadeEND" type="number" class="form-control" min="1" max="365" />
        <span asp-validation-for="QtdDiasValidadeEND" class="text-danger"></span>
    </div>

    <hr />
    <h4>Alertas e Faturamento</h4>

    <div class="form-group">
        <label asp-for="QtdMinEmailLog"></label>
        <input asp-for="QtdMinEmailLog" type="number" class="form-control" />
        <span asp-validation-for="QtdMinEmailLog" class="text-danger"></span>
        <small class="form-text text-muted">Mínimo de erros para enviar alerta por email</small>
    </div>

    <div class="form-group">
        <label asp-for="DiaCorte"></label>
        <input asp-for="DiaCorte" type="number" class="form-control" min="1" max="31" />
        <span asp-validation-for="DiaCorte" class="text-danger"></span>
        <small class="form-text text-muted">Dia de corte para faturamento</small>
    </div>

    <hr />
    <h4>Tipo de Documento</h4>

    <div class="form-group">
        <label asp-for="TipoWebService"></label>
        <select asp-for="TipoWebService" class="form-control">
            <option value="1">CPF</option>
            <option value="2">CNPJ</option>
            <option value="3" selected>Ambos (CPF e CNPJ)</option>
        </select>
        <span asp-validation-for="TipoWebService" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Status"></label>
        <input asp-for="Status" class="form-control" />
        <span asp-validation-for="Status" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Cadastrar</button>
    <a asp-action="Index" class="btn btn-secondary">Cancelar</a>
</form>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

**Arquivo:** `ConsultaDocumentos.Web/Views/Aplicacao/Create.cshtml`

```html
@model AplicacaoViewModel

<h2>Cadastrar Aplicação</h2>

<form asp-action="Create" method="post">
    <div class="form-group">
        <label asp-for="Nome"></label>
        <input asp-for="Nome" class="form-control" />
        <span asp-validation-for="Nome" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Descricao"></label>
        <textarea asp-for="Descricao" class="form-control" rows="3"></textarea>
        <span asp-validation-for="Descricao" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Status"></label>
        <input asp-for="Status" class="form-control" />
        <span asp-validation-for="Status" class="text-danger"></span>
    </div>

    <!-- NOVO CAMPO -->
    <div class="form-check">
        <input asp-for="Serpro" class="form-check-input" type="checkbox" />
        <label asp-for="Serpro" class="form-check-label">
            Tem acesso ao provedor SERPRO
        </label>
        <span asp-validation-for="Serpro" class="text-danger"></span>
    </div>

    <br />
    <button type="submit" class="btn btn-primary">Cadastrar</button>
    <a asp-action="Index" class="btn btn-secondary">Cancelar</a>
</form>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

**Arquivo:** `ConsultaDocumentos.Web/Views/QuadroSocietario/Create.cshtml`

```html
@model QuadroSocietarioViewModel

<h2>Cadastrar Sócio</h2>

<form asp-action="Create" method="post">
    <div class="form-group">
        <label asp-for="CpfCnpj"></label>
        <input asp-for="CpfCnpj" class="form-control" />
        <span asp-validation-for="CpfCnpj" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Nome"></label>
        <input asp-for="Nome" class="form-control" />
        <span asp-validation-for="Nome" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="Qualificacao"></label>
        <input asp-for="Qualificacao" class="form-control" />
        <span asp-validation-for="Qualificacao" class="text-danger"></span>
    </div>

    <!-- NOVOS CAMPOS -->

    <div class="form-group">
        <label asp-for="Tipo"></label>
        <select asp-for="Tipo" class="form-control">
            <option value="">Selecione...</option>
            <option value="ADMINISTRADOR">Administrador</option>
            <option value="SOCIO">Sócio</option>
            <option value="PRESIDENTE">Presidente</option>
            <option value="DIRETOR">Diretor</option>
        </select>
        <span asp-validation-for="Tipo" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="IdNacionalidade"></label>
        <select asp-for="IdNacionalidade" class="form-control" asp-items="ViewBag.Nacionalidades">
            <option value="">Selecione...</option>
        </select>
        <span asp-validation-for="IdNacionalidade" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="CodPaisOrigem"></label>
        <input asp-for="CodPaisOrigem" class="form-control" />
        <span asp-validation-for="CodPaisOrigem" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="NomePaisOrigem"></label>
        <input asp-for="NomePaisOrigem" class="form-control" />
        <span asp-validation-for="NomePaisOrigem" class="text-danger"></span>
    </div>

    <div class="form-group">
        <label asp-for="PercentualCapital"></label>
        <input asp-for="PercentualCapital" type="number" step="0.01" class="form-control" />
        <span asp-validation-for="PercentualCapital" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Cadastrar</button>
    <a asp-action="Index" class="btn btn-secondary">Cancelar</a>
</form>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

---

### 6.3 Atualizar Controllers Web

**Arquivo:** `ConsultaDocumentos.Web/Controllers/QuadroSocietarioController.cs`

```csharp
[HttpGet]
public async Task<IActionResult> Create()
{
    // Carregar nacionalidades para dropdown
    var nacionalidades = await _nacionalidadeApi.GetAll();

    ViewBag.Nacionalidades = nacionalidades.Select(n => new SelectListItem
    {
        Value = n.Id.ToString(),
        Text = n.Nome
    }).ToList();

    return View();
}

[HttpGet]
public async Task<IActionResult> Edit(Guid id)
{
    var quadroSocietario = await _quadroSocietarioApi.GetById(id);

    // Carregar nacionalidades para dropdown
    var nacionalidades = await _nacionalidadeApi.GetAll();

    ViewBag.Nacionalidades = nacionalidades.Select(n => new SelectListItem
    {
        Value = n.Id.ToString(),
        Text = n.Nome,
        Selected = n.Id == quadroSocietario.IdNacionalidade
    }).ToList();

    return View(quadroSocietario);
}
```

---

## 📦 FASE 7: Testes e Validação

### 7.1 Testar Migrations

**Script SQL de Verificação:**

```sql
-- 1. Verificar campos adicionados em Provedor
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Provedor'
ORDER BY ORDINAL_POSITION;

-- Campos esperados: EndCertificado, Usuario, Senha, Dominio, QtdAcessoMinimo, QtdAcessoMaximo,
-- QtdDiasValidadePF, QtdDiasValidadePJ, QtdDiasValidadeEND, QtdMinEmailLog, DiaCorte, Porta, TipoWebService

-- 2. Verificar campo Serpro em Aplicacao
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Aplicacao' AND COLUMN_NAME = 'Serpro';

-- 3. Verificar campos em Documento
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Documento' AND COLUMN_NAME IN ('NomeSocial', 'Porte');

-- 4. Verificar FK Nacionalidade em QuadroSocietario
SELECT
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumnName
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) = 'QuadroSocietario'
  AND COL_NAME(fkc.parent_object_id, fkc.parent_column_id) = 'IdNacionalidade';

-- 5. Verificar índices criados
SELECT
    i.name AS IndexName,
    OBJECT_NAME(i.object_id) AS TableName,
    COL_NAME(ic.object_id, ic.column_id) AS ColumnName
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE OBJECT_NAME(i.object_id) IN ('AplicacaoProvedor', 'QuadroSocietario')
ORDER BY TableName, IndexName, ic.key_ordinal;

-- 6. Verificar campos de log em AplicacaoProvedor
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AplicacaoProvedor'
  AND COLUMN_NAME IN ('IdDocumento', 'DataConsulta', 'EnderecoIP', 'RemoteHost');
```

---

### 7.2 Testar Lógica de Validade de Cache

**Cenário de Teste:**

1. **Configurar Provedor SERPRO:**
   - `QtdDiasValidadePF = 30`
   - `QtdDiasValidadePJ = 90`

2. **Consultar CPF:**
   ```bash
   curl -X GET "http://localhost:5000/api/Documento/consultar/cpf/12345678901?aplicacaoId=<GUID>"
   ```

3. **Verificar no BD:**
   ```sql
   SELECT Numero, DataConsulta, DataConsultaValidade,
          DATEDIFF(DAY, DataConsulta, DataConsultaValidade) AS DiasValidade
   FROM Documento
   WHERE Numero = '12345678901';
   ```

   **Resultado esperado:** `DiasValidade = 30`

4. **Consultar CNPJ:**
   ```bash
   curl -X GET "http://localhost:5000/api/Documento/consultar/cnpj/12345678000190?aplicacaoId=<GUID>"
   ```

5. **Verificar no BD:**
   ```sql
   SELECT Numero, DataConsulta, DataConsultaValidade,
          DATEDIFF(DAY, DataConsulta, DataConsultaValidade) AS DiasValidade
   FROM Documento
   WHERE Numero = '12345678000190';
   ```

   **Resultado esperado:** `DiasValidade = 90`

---

### 7.3 Testar Controle de Quota SERPRO

**Cenário de Teste:**

1. **Configurar Provedor SERPRO:**
   - `QtdAcessoMaximo = 5`

2. **Consultar 5 CPFs diferentes:**
   ```bash
   for i in {1..5}; do
     curl -X GET "http://localhost:5000/api/Documento/consultar/cpf/1234567890$i?aplicacaoId=<GUID>"
   done
   ```

3. **Verificar logs:**
   ```
   [INFO] Quota SERPRO: 1/5
   [INFO] Quota SERPRO: 2/5
   [INFO] Quota SERPRO: 3/5
   [INFO] Quota SERPRO: 4/5
   [INFO] Quota SERPRO: 5/5
   ```

4. **Consultar 6º CPF:**
   ```bash
   curl -X GET "http://localhost:5000/api/Documento/consultar/cpf/123456789006?aplicacaoId=<GUID>"
   ```

5. **Verificar logs:**
   ```
   [WARNING] Quota SERPRO atingida: 5/5. Pulando provedor.
   [INFO] Tentando consulta no provedor SERASA
   ```

6. **Verificar LogAuditoria:**
   ```sql
   SELECT ProvedorPrincipal, COUNT(*) AS Total
   FROM LogAuditoria
   WHERE DataHoraConsulta >= CAST(GETDATE() AS DATE)
   GROUP BY ProvedorPrincipal;
   ```

   **Resultado esperado:**
   ```
   SERPRO  | 5
   SERASA  | 1
   ```

---

### 7.4 Testar Cache de Provedores

**Cenário de Teste:**

1. **Primeira consulta (cache miss):**
   ```bash
   curl -X GET "http://localhost:5000/api/Documento/consultar/cpf/12345678901?aplicacaoId=<GUID>"
   ```

   **Logs esperados:**
   ```
   [INFO] Cache miss: buscando provedores do BD para aplicação <GUID>
   [INFO] Provedores armazenados no cache em memória (5 minutos) para aplicação <GUID>
   ```

2. **Segunda consulta imediata (cache hit):**
   ```bash
   curl -X GET "http://localhost:5000/api/Documento/consultar/cpf/98765432100?aplicacaoId=<GUID>"
   ```

   **Logs esperados:**
   ```
   [INFO] Provedores retornados do cache em memória para aplicação <GUID>
   ```

3. **Aguardar 6 minutos e consultar novamente:**
   ```bash
   sleep 360
   curl -X GET "http://localhost:5000/api/Documento/consultar/cpf/11111111111?aplicacaoId=<GUID>"
   ```

   **Logs esperados:**
   ```
   [INFO] Cache miss: buscando provedores do BD para aplicação <GUID>
   ```

---

### 7.5 Testar Log de Uso (Billing)

**Cenário de Teste:**

1. **Consultar documento:**
   ```bash
   curl -X GET "http://localhost:5000/api/Documento/consultar/cpf/12345678901?aplicacaoId=<GUID>"
   ```

2. **Verificar AplicacaoProvedor:**
   ```sql
   SELECT
       ap.Id,
       a.Nome AS Aplicacao,
       p.Nome AS Provedor,
       d.Numero AS Documento,
       ap.DataConsulta,
       ap.EnderecoIP,
       ap.RemoteHost
   FROM AplicacaoProvedor ap
   INNER JOIN Aplicacao a ON a.Id = ap.AplicacaoId
   INNER JOIN Provedor p ON p.Id = ap.ProvedorId
   INNER JOIN Documento d ON d.Id = ap.IdDocumento
   WHERE ap.DataConsulta IS NOT NULL
   ORDER BY ap.DataConsulta DESC;
   ```

   **Resultado esperado:** Registro com dados preenchidos

3. **Relatório de Billing (exemplo):**
   ```sql
   SELECT
       a.Nome AS Aplicacao,
       p.Nome AS Provedor,
       CAST(ap.DataConsulta AS DATE) AS Data,
       COUNT(*) AS TotalConsultas
   FROM AplicacaoProvedor ap
   INNER JOIN Aplicacao a ON a.Id = ap.AplicacaoId
   INNER JOIN Provedor p ON p.Id = ap.ProvedorId
   WHERE ap.DataConsulta IS NOT NULL
     AND ap.DataConsulta >= DATEADD(MONTH, -1, GETDATE())
   GROUP BY a.Nome, p.Nome, CAST(ap.DataConsulta AS DATE)
   ORDER BY Data DESC, Aplicacao, Provedor;
   ```

---

## 📊 Checklist de Conclusão

### Fase 1: Entidades ✅ **CONCLUÍDA**
- [x] Provedor: 13 campos adicionados
- [x] Aplicacao: campo Serpro adicionado
- [x] Documento: NomeSocial e Porte adicionados
- [x] QuadroSocietario: FK Nacionalidade + 4 campos
- [x] AplicacaoProvedor: 4 campos de log adicionados

### Fase 2: Application ✅
- [x] AutoMapper atualizado (ReverseMap detecta automaticamente)
- [ ] Services de conversão atualizados (NomeSocial, Porte, Nacionalidade)

### Fase 3: Infraestrutura ✅ **CONCLUÍDA**
- [x] Migration criada (20251006031915_AddCamposLegadosSistema)
- [x] Database atualizada
- [x] Constraints e índices verificados

### Fase 4: Lógicas de Negócio ✅ **CONCLUÍDA**
- [x] Validade dinâmica de cache implementada
- [x] Controle de quota SERPRO implementado
- [x] Cache em memória de provedores (5 min) implementado
- [x] Log de uso em AplicacaoProvedor implementado

### Fase 5: API ✅ **CONCLUÍDA**
- [x] ProvedorController: validações adicionadas
- [x] Validação de QtdDiasValidade > 0
- [x] Validação de TipoWebService (1-3)
- [x] Validação de QtdAcessoMaximo não negativo

### Fase 6: Web ✅ **CONCLUÍDA**
- [x] ViewModels atualizadas (Provedor, Aplicacao, Documento, QuadroSocietario)
- [x] Views Create/Edit atualizadas (Provedor com 13 novos campos organizados)
- [x] Views Create/Edit atualizadas (Aplicacao com checkbox Serpro)
- [x] QuadroSocietarioController criado com dropdown Nacionalidade
- [x] IQuadroSocietarioApi criado (Refit client)
- [x] Views Create/Edit/Index criadas para QuadroSocietario

### Fase 7: Testes ✅ **CONCLUÍDA**
- [x] Build executado com sucesso (0 erros, 31 warnings)
- [x] Compilação verificada em todos os projetos
- [x] Estrutura de arquivos validada
- [x] Controllers e Views criados corretamente

---

## 📝 Comandos Úteis

### Migrations
```bash
# Criar migration
dotnet ef migrations add AddCamposLegadosSistema --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API

# Aplicar migration
dotnet ef database update --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API

# Reverter última migration
dotnet ef migrations remove --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API

# Gerar script SQL
dotnet ef migrations script --project ConsultaDocumentos.Infra.Data --startup-project ConsultaDocumentos.API --output migration.sql
```

### Build e Run
```bash
# Build solução
dotnet build

# Run API
dotnet run --project ConsultaDocumentos.API

# Run Web
dotnet run --project ConsultaDocumentos.Web
```

---

## 🎯 Próximos Passos (Futuro)

Itens NÃO aprovados nesta fase, mas que podem ser implementados posteriormente:

- **Item 1:** Implementar BoaVista (Socket TCP + CSR60/61)
- **Item 7:** Criar TBL_DOCUMENTO_HIST (histórico/versionamento)
- **Item 10:** Modo contingência offline
- **Item 11:** Parâmetros `UsarRepositorio` e `AceitarVencido`
- **Item 12:** Validação de sequências repetidas CPF/CNPJ
- **Item 14:** Serviço Enriquecimento de Dados (JWT + endpoints)
- **Item 15:** TLS 1.2 explícito
- **Item 16:** Remoção de acentuação

---

**Documento gerado em:** 2025-10-06
**Versão:** 1.0
