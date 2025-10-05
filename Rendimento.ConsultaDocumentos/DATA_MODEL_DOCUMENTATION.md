# Documentação do Modelo de Dados - Sistema de Consulta de Documentos

**Versão:** 2.0
**Última Atualização:** 2025-10-03
**Arquitetura:** Clean Architecture + DDD

---

## Índice

1. [Visão Geral](#visão-geral)
2. [Diagrama de Relacionamentos](#diagrama-de-relacionamentos)
3. [Catálogo de Entidades](#catálogo-de-entidades)
4. [Catálogo de Enums](#catálogo-de-enums)
5. [Padrões de Auditoria](#padrões-de-auditoria)
6. [Mapa de Dependências](#mapa-de-dependências)
7. [Índices e Otimizações](#índices-e-otimizações)

---

## Visão Geral

Este documento descreve o modelo de dados completo do **Sistema de Consulta de Documentos CPF/CNPJ**, um sistema modernizado construído com .NET 8 seguindo princípios de Clean Architecture e Domain-Driven Design (DDD).

### Estatísticas do Modelo

- **17 Entidades de Domínio**
- **6 Enumeradores**
- **9 Relacionamentos N:1 (Many-to-One)**
- **3 Relacionamentos N:N (Many-to-Many)**
- **2 Tabelas de Log/Auditoria**
- **2 Tabelas de Referência**

### Tecnologias

- **ORM:** Entity Framework Core 8.0
- **Banco de Dados:** SQLite (Development) / SQL Server (Production)
- **Padrões:** Repository Pattern, Unit of Work, Aggregate Roots

---

## Diagrama de Relacionamentos

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          DOMÍNIO PRINCIPAL                               │
└─────────────────────────────────────────────────────────────────────────┘

                        ┌─────────────────────┐
                        │    Nacionalidade    │
                        │  (Tabela Referência)│
                        └──────────┬──────────┘
                                   │ 1
                                   │
                                   │ N
                        ┌──────────▼──────────┐
         ┌──────────────┤     DOCUMENTO       ├──────────────┐
         │              │  (Aggregate Root)   │              │
         │              └──────────┬──────────┘              │
         │ 1                       │ 1                       │ 1
         │                         │                         │
         │ N                       │ N                       │ N
┌────────▼────────┐    ┌──────────▼─────────┐    ┌─────────▼────────┐
│    Endereco     │    │     Telefone       │    │      Email       │
│                 │    │                    │    │                  │
│ - TipoEndereco  │    │ - TipoTelefone     │    │ - EnderecoEmail  │
└─────────────────┘    └────────────────────┘    └──────────────────┘

                       ┌────────────────────┐
                       │ QuadroSocietario   │
                       │   (somente CNPJ)   │
                       └────────────────────┘
                                   │ N
                                   │
                                   │ 1
                        ┌──────────▼──────────┐
                        │     DOCUMENTO       │
                        └─────────────────────┘

         ┌─────────────────────┐
         │ SituacaoCadastral   │
         │  (Tabela Referência)│
         └──────────┬──────────┘
                    │ 1
                    │
                    │ N
         ┌──────────▼──────────┐
         │     DOCUMENTO       │
         └─────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│                    AUTENTICAÇÃO E APLICAÇÕES                             │
└─────────────────────────────────────────────────────────────────────────┘

                        ┌─────────────────────┐
                        │     Aplicacao       │
                        │  (Aggregate Root)   │
                        └──────────┬──────────┘
                                   │ 1
                        ┌──────────┼──────────┐
                        │ N        │          │ N
               ┌────────▼────┐     │     ┌────▼────────────────┐
               │Autenticacao │     │     │ AplicacaoProvedor   │
               │  (Tokens)   │     │     │    (Many-to-Many)   │
               └─────────────┘     │     └─────┬───────────────┘
                                   │           │ N
                                   │ 1         │
                        ┌──────────▼──────┐    │ 1
                        │  LogAuditoria   │    │
                        │    (Logs)       │    │
                        └─────────────────┘    │
                                               │
                                    ┌──────────▼──────────┐
                                    │     Provedor        │
                                    │ (Provedores Externos│
                                    └─────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│                    ADMINISTRAÇÃO E SEGURANÇA                             │
└─────────────────────────────────────────────────────────────────────────┘

                        ┌─────────────────────┐
                        │      Usuario        │
                        │  (Aggregate Root)   │
                        └──────────┬──────────┘
                                   │ 1
                                   │
                                   │ N
                        ┌──────────▼──────────┐
                        │   UsuarioPerfil     │
                        │   (Many-to-Many)    │
                        └──────────┬──────────┘
                                   │ N
                                   │
                                   │ 1
                        ┌──────────▼──────────┐
                        │       Perfil        │
                        │                     │
                        └─────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│                         LOGS E AUDITORIA                                 │
└─────────────────────────────────────────────────────────────────────────┘

                ┌─────────────────────┐    ┌─────────────────────┐
                │   LogAuditoria      │    │      LogErro        │
                │ (Consultas API)     │    │  (Erros Sistema)    │
                └─────────────────────┘    └─────────────────────┘
```

---

## Catálogo de Entidades

### 1. Documento (Aggregate Root)

**Descrição:** Entidade principal que representa documentos CPF (Pessoa Física) ou CNPJ (Pessoa Jurídica) consultados no sistema. Segue padrões DDD com factory methods e encapsulamento.

**Tabela:** `TBL_DOCUMENTOS`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| IdDocumento | int | PK | Sim | Identificador único |
| TipoPessoa | char | - | Sim | 'F' = CPF, 'J' = CNPJ |
| Numero | string | - | Sim | Número do documento (CPF/CNPJ) |
| Nome | string | - | Sim | Nome da pessoa/razão social |
| DataConsulta | DateTime | - | Sim | Data da última consulta |
| DataConsultaValidade | DateTime | - | Sim | Data de validade dos dados |
| RowVersion | byte[] | - | Não | Controle de concorrência otimista |
| **Campos PJ (Pessoa Jurídica)** |
| DataAbertura | DateTime? | - | Não | Data de abertura da empresa |
| Inscricao | string? | - | Não | Inscrição estadual/municipal |
| NaturezaJuridica | int? | - | Não | Código da natureza jurídica |
| DescricaoNaturezaJuridica | string? | - | Não | Descrição da natureza jurídica |
| Segmento | string? | - | Não | Segmento de atuação |
| RamoAtividade | int? | - | Não | Código do ramo de atividade |
| DescricaoRamoAtividade | string? | - | Não | Descrição do ramo de atividade |
| NomeFantasia | string? | - | Não | Nome fantasia da empresa |
| MatrizFilialQtde | int? | - | Não | Quantidade de filiais |
| Matriz | bool? | - | Não | Indica se é matriz |
| **Campos PF (Pessoa Física)** |
| DataNascimento | DateTime? | - | Não | Data de nascimento |
| NomeMae | string? | - | Não | Nome da mãe |
| Sexo | string? | - | Não | Sexo (M/F) |
| TituloEleitor | string? | - | Não | Número do título de eleitor |
| ResidenteExterior | bool? | - | Não | Indica se reside no exterior |
| AnoObito | int? | - | Não | Ano de óbito (se aplicável) |
| **Campos de Situação** |
| DataSituacao | DateTime? | - | Não | Data da situação cadastral |
| IdSituacao | int? | FK | Não | Chave para SituacaoCadastral |
| CodControle | string? | - | Não | Código de controle |
| DataFundacao | DateTime? | - | Não | Data de fundação |
| OrigemBureau | string? | - | Não | Origem do bureau de dados |
| IdNacionalidade | int? | FK | Não | Chave para Nacionalidade |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| 1:N | Endereco | 1 → N | Um documento possui vários endereços |
| 1:N | Telefone | 1 → N | Um documento possui vários telefones |
| 1:N | Email | 1 → N | Um documento possui vários emails |
| 1:N | QuadroSocietario | 1 → N | Um CNPJ possui vários sócios |
| N:1 | Nacionalidade | N → 1 | Vários documentos podem ter a mesma nacionalidade |
| N:1 | SituacaoCadastral | N → 1 | Vários documentos podem ter a mesma situação |

#### Métodos de Negócio (Domain Logic)

```csharp
// Factory Methods
+ static CriarPessoaFisica(numero, nome, ...): Documento
+ static CriarPessoaJuridica(numero, nome, ...): Documento

// Consultas
+ IsValido(): bool                    // Verifica se está dentro da validade
+ IsPessoaFisica(): bool              // Verifica se é CPF
+ IsPessoaJuridica(): bool            // Verifica se é CNPJ
+ GetTipoDocumento(): TipoDocumento   // Retorna enum TipoDocumento
+ PrecisaAtualizacao(): bool          // Verifica se precisa nova consulta

// Comandos
+ AtualizarDataConsulta(validadeDias): void
+ AtualizarDadosPessoaFisica(...): void
+ AtualizarDadosPessoaJuridica(...): void
+ AdicionarEndereco(endereco): void
+ RemoverEndereco(endereco): void
```

#### Regras de Validação

- Número do documento é obrigatório
- Nome é obrigatório
- TipoPessoa deve ser 'F' ou 'J'
- Campos PF são validados apenas se TipoPessoa = 'F'
- Campos PJ são validados apenas se TipoPessoa = 'J'

---

### 2. Endereco

**Descrição:** Endereços vinculados a documentos (CPF/CNPJ). Suporta múltiplos tipos de endereço por documento.

**Tabela:** `TBL_ENDERECOS`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| IdEndereco | int | PK | Sim | Identificador único |
| IdDocumento | int | FK | Sim | Chave para Documento |
| Logradouro | string? | - | Não | Nome da rua/avenida |
| Numero | string? | - | Não | Número do imóvel |
| Complemento | string? | - | Não | Complemento |
| Bairro | string? | - | Não | Bairro |
| CEP | string? | - | Não | CEP (8 dígitos) |
| Cidade | string? | - | Não | Cidade |
| UF | string? | - | Não | Unidade Federativa (2 caracteres) |
| Tipo | TipoEndereco | - | Sim | Enum: Residencial, Comercial, etc. |
| DataAtualizacao | DateTime? | - | Não | Data da última atualização |
| RowVersion | byte[] | - | Não | Controle de concorrência |
| TipoLogradouro | string? | - | Não | Tipo de logradouro (legado) |

#### Propriedades Computadas

| Propriedade | Tipo | Descrição |
|------------|------|-----------|
| EnderecoCompleto | string | Endereço formatado como string |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| N:1 | Documento | N → 1 | Vários endereços pertencem a um documento |

#### Métodos de Negócio

```csharp
// Factory Method
+ static Criar(idDocumento, logradouro, ...): Endereco

// Consultas
+ IsValido(): bool                     // Tem informações mínimas
+ IsCompleto(): bool                   // Tem todas informações principais
+ IsCepValido(): bool                  // CEP no formato correto (8 dígitos)
+ IsUfValida(): bool                   // UF com 2 caracteres
+ GetEnderecoFormatado(): string       // Retorna endereço formatado
+ IsComercial(): bool                  // Verifica se é comercial
+ IsResidencial(): bool                // Verifica se é residencial
+ IsCorrespondencia(): bool            // Verifica se é correspondência
+ GetTipoDescricao(): string           // Retorna descrição do tipo

// Comandos
+ Atualizar(logradouro, numero, ...): void
```

#### Regras de Validação

- IdDocumento deve ser maior que zero
- CEP deve ter 8 dígitos (se informado)
- UF deve ter 2 caracteres alfabéticos (se informado)
- UF é convertido para maiúsculas automaticamente

---

### 3. Telefone

**Descrição:** Telefones vinculados a documentos. Suporta diferentes tipos (residencial, comercial, celular, WhatsApp).

**Tabela:** `TBL_TELEFONES`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| IdTelefone | int | PK | Sim | Identificador único |
| IdDocumento | int | FK | Sim | Chave para Documento |
| DDD | string? | - | Não | DDD (2 dígitos) |
| Numero | string? | - | Não | Número do telefone (8-9 dígitos) |
| Tipo | TipoTelefone | - | Sim | Enum: Residencial, Comercial, Celular, Fax, WhatsApp |
| DataCriacao | DateTime | - | Sim | Data de criação |
| DataAtualizacao | DateTime? | - | Não | Data da última atualização |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| N:1 | Documento | N → 1 | Vários telefones pertencem a um documento |

#### Métodos de Negócio

```csharp
// Consultas
+ GetTelefoneFormatado(): string       // Retorna "(DDD) Numero - Tipo"
+ IsValido(): bool                     // DDD >= 2 e Numero >= 8
+ IsCelular(): bool                    // Verifica se é celular
+ IsComercial(): bool                  // Verifica se é comercial
+ IsResidencial(): bool                // Verifica se é residencial
+ IsWhatsApp(): bool                   // Verifica se é WhatsApp
+ PermiteMensagens(): bool             // Celular ou WhatsApp
+ GetTipoDescricao(): string           // Retorna descrição do tipo
```

#### Regras de Validação

- DDD deve ter pelo menos 2 caracteres
- Número deve ter pelo menos 8 caracteres
- Tipo é obrigatório

---

### 4. Email

**Descrição:** Emails vinculados a documentos.

**Tabela:** `TBL_EMAILS`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| IdEmail | int | PK | Sim | Identificador único |
| IdDocumento | int | FK | Sim | Chave para Documento |
| EnderecoEmail | string? | - | Não | Endereço de email |
| DataCriacao | DateTime | - | Sim | Data de criação |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| N:1 | Documento | N → 1 | Vários emails pertencem a um documento |

#### Métodos de Negócio

```csharp
+ IsValido(): bool  // Valida formato com regex
```

#### Regras de Validação

- Email deve corresponder ao padrão: `^[^@\s]+@[^@\s]+\.[^@\s]+$`

---

### 5. QuadroSocietario

**Descrição:** Quadro societário de empresas (somente CNPJ). Armazena informações sobre sócios e representantes legais.

**Tabela:** `TBL_QUADRO_SOCIETARIO`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| IdQuadroSocietario | int | PK | Sim | Identificador único |
| IdDocumento | int | FK | Sim | Chave para Documento (CNPJ) |
| CPFSocio | string? | - | Não | CPF do sócio (legado) |
| CpfCnpj | string? | - | Não | CPF/CNPJ do sócio |
| NomeSocio | string? | - | Não | Nome do sócio (legado) |
| Nome | string? | - | Não | Nome do sócio |
| QualificacaoSocio | string? | - | Não | Qualificação (legado) |
| Qualificacao | string? | - | Não | Qualificação do sócio |
| CpfRepresentanteLegal | string? | - | Não | CPF do representante legal |
| NomeRepresentanteLegal | string? | - | Não | Nome do representante legal |
| QualificacaoRepresentanteLegal | string? | - | Não | Qualificação do representante |
| DataEntrada | DateTime? | - | Não | Data de entrada no quadro |
| DataSaida | DateTime? | - | Não | Data de saída do quadro |
| PercentualCapital | decimal? | - | Não | Percentual de participação |
| DataCriacao | DateTime | - | Sim | Data de criação |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| N:1 | Documento | N → 1 | Vários sócios pertencem a um CNPJ |

#### Métodos de Negócio

```csharp
+ IsValido(): bool                   // Tem nome e qualificação
+ GetPercentualFormatado(): string   // Retorna percentual formatado "XX,XX%"
```

---

### 6. Aplicacao (Aggregate Root)

**Descrição:** Aplicações consumidoras da API. Gerencia tokens de autenticação e associação com provedores.

**Tabela:** `TBL_APLICACOES`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| Id | int | PK | Sim | Identificador único |
| Nome | string | - | Sim | Nome da aplicação |
| Descricao | string | - | Não | Descrição da aplicação |
| Status | StatusEnum | - | Sim | Ativo/Inativo |
| DataCriacao | DateTime | - | Sim | Data de criação |
| DataAtualizacao | DateTime? | - | Não | Data da última atualização |
| CriadoPor | int? | - | Não | ID do usuário criador |
| AtualizadoPor | int? | - | Não | ID do último usuário atualizador |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| 1:N | Autenticacao | 1 → N | Uma aplicação possui vários tokens |
| 1:N | AplicacaoProvedor | 1 → N | Associação com provedores |
| 1:N | LogAuditoria | 1 → N | Logs de consultas da aplicação |

#### Métodos de Negócio

```csharp
// Comandos
+ AtualizarDados(nome, descricao, atualizadoPor): void
+ Ativar(atualizadoPor): void
+ Desativar(atualizadoPor): void
+ GerarToken(dataInicio, dataFim, descricao, criadoPor): Autenticacao
+ AssociarProvedor(tipoProvedor, ordem, criadoPor): void
+ AtualizarOrdemProvedor(tipoProvedor, novaOrdem, atualizadoPor): void
+ DesassociarProvedor(tipoProvedor): void
```

#### Regras de Validação

- Nome é obrigatório
- Nome é trimmed automaticamente
- Status inicia como Ativo

---

### 7. Autenticacao

**Descrição:** Tokens de autenticação (API Keys) das aplicações. Gerencia período de validade.

**Tabela:** `TBL_AUTENTICACAO`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| Id | int | PK | Sim | Identificador único |
| AplicacaoId | int | FK | Sim | Chave para Aplicacao |
| DataInicio | DateTime | - | Sim | Data de início da validade |
| DataFim | DateTime | - | Sim | Data de fim da validade |
| Token | string | - | Sim | Token GUID gerado automaticamente |
| Descricao | string | - | Não | Descrição do token |
| Status | StatusEnum | - | Sim | Ativo/Inativo |
| DataCriacao | DateTime | - | Sim | Data de criação |
| DataAtualizacao | DateTime? | - | Não | Data da última atualização |
| CriadoPor | int? | - | Não | ID do usuário criador |
| AtualizadoPor | int? | - | Não | ID do último usuário atualizador |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| N:1 | Aplicacao | N → 1 | Vários tokens pertencem a uma aplicação |

#### Métodos de Negócio

```csharp
// Consultas
+ EstaValido(): bool  // Status ativo E dentro do período de validade

// Comandos
+ AtualizarPeriodo(dataInicio, dataFim, atualizadoPor): void
+ AtualizarDescricao(descricao, atualizadoPor): void
+ Ativar(atualizadoPor): void
+ Desativar(atualizadoPor): void
+ RegenerarToken(atualizadoPor): void
```

#### Regras de Validação

- DataFim deve ser maior que DataInicio
- DataInicio não pode ser no passado (ao criar)
- Token é gerado automaticamente como GUID
- Status inicia como Ativo

---

### 8. AplicacaoProvedor

**Descrição:** Tabela de relacionamento N:N entre Aplicacoes e Provedores. Define ordem de fallback.

**Tabela:** `TBL_APLICACAO_PROVEDOR`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| Id | int | PK | Sim | Identificador único |
| AplicacaoId | int | FK | Sim | Chave para Aplicacao |
| TipoProvedor | TipoProvedor | - | Sim | Enum do tipo de provedor |
| Ordem | int | - | Sim | Ordem de prioridade (1 = primeiro) |
| Status | StatusEnum | - | Sim | Ativo/Inativo |
| DataCriacao | DateTime | - | Sim | Data de criação |
| DataAtualizacao | DateTime? | - | Não | Data da última atualização |
| CriadoPor | int? | - | Não | ID do usuário criador |
| AtualizadoPor | int? | - | Não | ID do último usuário atualizador |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| N:1 | Aplicacao | N → 1 | Várias associações pertencem a uma aplicação |

#### Métodos de Negócio

```csharp
+ AtualizarOrdem(novaOrdem, atualizadoPor): void
+ Ativar(atualizadoPor): void
+ Desativar(atualizadoPor): void
```

#### Regras de Validação

- Ordem deve ser maior que zero
- Não pode ter TipoProvedor duplicado para a mesma Aplicação

---

### 9. Provedor

**Descrição:** Provedores externos de dados (SERPRO, SERASA, etc.). Configuração de endpoints e credenciais.

**Tabela:** `TBL_PROVEDORES`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| Id | int | PK | Sim | Identificador único |
| Nome | string | - | Sim | Nome do provedor |
| Descricao | string | - | Não | Descrição do provedor |
| EndpointUrl | string | - | Sim | URL base do endpoint |
| Credenciais | string | - | Não | Credenciais encriptadas |
| Prioridade | int | - | Sim | Prioridade global (1 = maior) |
| TimeoutSegundos | int | - | Sim | Timeout em segundos |
| Status | StatusEnum | - | Sim | Ativo/Inativo |
| DataCriacao | DateTime | - | Sim | Data de criação |
| DataAtualizacao | DateTime? | - | Não | Data da última atualização |
| CriadoPor | int? | - | Não | ID do usuário criador |
| AtualizadoPor | int? | - | Não | ID do último usuário atualizador |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| 1:N | AplicacaoProvedor | 1 → N | Associações com aplicações |

#### Métodos de Negócio

```csharp
+ AtualizarDados(nome, descricao, endpointUrl, prioridade, timeoutSegundos, atualizadoPor): void
+ AtualizarCredenciais(credenciaisEncriptadas, atualizadoPor): void
+ Ativar(atualizadoPor): void
+ Desativar(atualizadoPor): void
```

#### Regras de Validação

- Nome é obrigatório
- EndpointUrl é obrigatório
- Prioridade deve ser maior que zero
- TimeoutSegundos deve ser maior que zero

---

### 10. Usuario (Aggregate Root)

**Descrição:** Usuários administradores do sistema. Gerencia perfis de acesso.

**Tabela:** `TBL_USUARIOS`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| Id | int | PK | Sim | Identificador único |
| Nome | string | - | Sim | Nome completo |
| Login | string | - | Sim | Login único (lowercase) |
| Email | string | - | Sim | Email (lowercase) |
| Status | StatusEnum | - | Sim | Ativo/Inativo |
| DataCriacao | DateTime | - | Sim | Data de criação |
| DataAtualizacao | DateTime? | - | Não | Data da última atualização |
| CriadoPor | int? | - | Não | ID do usuário criador |
| AtualizadoPor | int? | - | Não | ID do último usuário atualizador |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| 1:N | UsuarioPerfil | 1 → N | Associação com perfis |

#### Métodos de Negócio

```csharp
+ AtualizarDados(nome, email, atualizadoPor): void
+ Ativar(atualizadoPor): void
+ Desativar(atualizadoPor): void
+ AdicionarPerfil(perfilId, criadoPor): void
+ RemoverPerfil(perfilId): void
+ PossuiPerfil(perfil): bool
```

#### Regras de Validação

- Nome é obrigatório
- Login é obrigatório e convertido para lowercase
- Email é obrigatório e convertido para lowercase
- Login não pode ser alterado após criação

---

### 11. Perfil

**Descrição:** Perfis de acesso no sistema (Administrador, Consulta, Operador, etc.).

**Tabela:** `TBL_PERFIS`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| Id | int | PK | Sim | Identificador único |
| Nome | string | - | Sim | Nome do perfil |
| Descricao | string | - | Não | Descrição do perfil |
| Status | StatusEnum | - | Sim | Ativo/Inativo |
| DataCriacao | DateTime | - | Sim | Data de criação |
| DataAtualizacao | DateTime? | - | Não | Data da última atualização |
| CriadoPor | int? | - | Não | ID do usuário criador |
| AtualizadoPor | int? | - | Não | ID do último usuário atualizador |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| 1:N | UsuarioPerfil | 1 → N | Associação com usuários |

#### Métodos de Negócio

```csharp
+ AtualizarDados(nome, descricao, atualizadoPor): void
+ Ativar(atualizadoPor): void
+ Desativar(atualizadoPor): void
```

---

### 12. UsuarioPerfil

**Descrição:** Tabela de relacionamento N:N entre Usuarios e Perfis.

**Tabela:** `TBL_USUARIO_PERFIL`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| UsuarioId | int | PK/FK | Sim | Chave composta - ID do usuário |
| PerfilId | int | PK/FK | Sim | Chave composta - ID do perfil |
| DataCriacao | DateTime | - | Sim | Data de criação |
| CriadoPor | int? | - | Não | ID do usuário criador |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| N:1 | Usuario | N → 1 | Várias associações pertencem a um usuário |
| N:1 | Perfil | N → 1 | Várias associações pertencem a um perfil |

---

### 13. LogAuditoria

**Descrição:** Logs de auditoria de todas as consultas de documentos realizadas via API.

**Tabela:** `TBL_LOG_AUDITORIA`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| Id | long | PK | Sim | Identificador único |
| AplicacaoId | int | FK | Sim | Chave para Aplicacao |
| NomeAplicacao | string | - | Sim | Nome da aplicação (desnormalizado) |
| DocumentoNumero | string | - | Sim | Número do documento consultado |
| TipoDocumento | TipoDocumento | - | Sim | CPF ou CNPJ |
| ParametrosEntrada | string | - | Não | JSON dos parâmetros de entrada |
| ProvedoresUtilizados | string | - | Não | JSON array dos provedores tentados |
| ProvedorPrincipal | string | - | Não | Provedor que retornou os dados |
| ConsultaSucesso | bool | - | Sim | Indica se a consulta teve sucesso |
| RespostaProvedor | string | - | Não | JSON da resposta do provedor |
| MensagemRetorno | string | - | Não | Mensagem de retorno |
| TempoProcessamentoMs | long | - | Sim | Tempo de processamento em milissegundos |
| DataHoraConsulta | DateTime | - | Sim | Data e hora da consulta |
| EnderecoIp | string | - | Não | IP de origem da requisição |
| UserAgent | string | - | Não | User agent do cliente |
| TokenAutenticacao | string | - | Não | Hash do token utilizado |
| OrigemCache | bool | - | Sim | Indica se veio do cache |
| InformacoesAdicionais | string | - | Não | JSON para informações extras |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| N:1 | Aplicacao | N → 1 | Vários logs pertencem a uma aplicação |

#### Métodos de Negócio

```csharp
+ AtualizarResposta(respostaProvedor, mensagemRetorno, consultaSucesso, tempoProcessamentoMs): void
+ DefinirProvedorPrincipal(provedorPrincipal): void
+ AdicionarInformacaoAdicional(chave, valor): void
```

---

### 14. LogErro

**Descrição:** Logs de erros e exceções do sistema.

**Tabela:** `TBL_LOG_ERRO`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| IdLogErro | int | PK | Sim | Identificador único |
| DataHora | DateTime | - | Sim | Data e hora do erro |
| Aplicacao | string? | - | Não | Nome da aplicação |
| Metodo | string? | - | Não | Método onde ocorreu o erro |
| Erro | string? | - | Não | Mensagem de erro |
| StackTrace | string? | - | Não | Stack trace completo |
| Usuario | string? | - | Não | Usuário que executou a operação |
| IdSistema | int? | FK | Não | Chave para Aplicacao |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| N:1 | Aplicacao | N → 1 | Vários erros pertencem a uma aplicação |

---

### 15. Nacionalidade

**Descrição:** Tabela de referência de nacionalidades.

**Tabela:** `TBL_NACIONALIDADES`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| IdNacionalidade | int | PK | Sim | Identificador único |
| Descricao | string | - | Sim | Descrição da nacionalidade |
| Codigo | string? | - | Não | Código ISO da nacionalidade |
| Ativo | bool | - | Sim | Indica se está ativo |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| 1:N | Documento | 1 → N | Uma nacionalidade pode estar em vários documentos |

---

### 16. SituacaoCadastral (Entidade)

**Descrição:** Tabela de referência de situações cadastrais.

**Tabela:** `TBL_SITUACAO_CADASTRAL`

#### Propriedades

| Propriedade | Tipo | PK/FK | Obrigatório | Descrição |
|------------|------|-------|-------------|-----------|
| IdSituacao | int | PK | Sim | Identificador único |
| Descricao | string | - | Sim | Descrição da situação |
| Ativo | bool | - | Sim | Indica se está ativo |
| DataCriacao | DateTime | - | Sim | Data de criação |

#### Relacionamentos

| Tipo | Entidade | Cardinalidade | Descrição |
|------|----------|---------------|-----------|
| 1:N | Documento | 1 → N | Uma situação pode estar em vários documentos |

---

## Catálogo de Enums

### 1. TipoDocumento

**Namespace:** `ConsultaDocumentos.Domain.Enums`

**Descrição:** Tipos de documento suportados.

| Valor | Nome | Descrição |
|-------|------|-----------|
| 1 | CPF | Cadastro de Pessoa Física |
| 2 | CNPJ | Cadastro Nacional de Pessoa Jurídica |

**Uso:** Principalmente em consultas e filtros de API.

---

### 2. StatusEnum

**Namespace:** `ConsultaDocumentos.Domain.Enums`

**Descrição:** Status genérico para entidades do sistema.

| Valor | Nome | Descrição |
|-------|------|-----------|
| 'A' (65) | Ativo | Entidade ativa |
| 'I' (73) | Inativo | Entidade inativa |

**Uso:** Aplicado em: Aplicacao, Autenticacao, AplicacaoProvedor, Provedor, Usuario, Perfil.

---

### 3. PerfilAcesso

**Namespace:** `ConsultaDocumentos.Domain.Enums`

**Descrição:** Perfis de acesso administrativo.

| Valor | Nome | Descrição |
|-------|------|-----------|
| 1 | Administrador | Acesso total ao sistema |
| 2 | Consulta | Somente leitura |
| 3 | Operador | Operações básicas |
| 4 | GestaoAcesso | Gestão de usuários e permissões |

---

### 4. TipoEndereco

**Namespace:** `ConsultaDocumentos.Domain.Enums`

**Descrição:** Tipos de endereço suportados.

| Valor | Nome | Descrição |
|-------|------|-----------|
| 1 | Residencial | Endereço residencial |
| 2 | Comercial | Endereço comercial |
| 3 | Correspondencia | Endereço de correspondência |
| 4 | Cobranca | Endereço de cobrança |
| 5 | Entrega | Endereço de entrega |

#### Extension Methods

```csharp
+ GetDescricao(): string
+ IsComercial(): bool
+ IsResidencial(): bool
+ IsCorrespondencia(): bool
```

---

### 5. TipoTelefone

**Namespace:** `ConsultaDocumentos.Domain.Enums`

**Descrição:** Tipos de telefone suportados.

| Valor | Nome | Descrição |
|-------|------|-----------|
| 1 | Residencial | Telefone residencial |
| 2 | Comercial | Telefone comercial |
| 3 | Celular | Celular |
| 4 | Fax | Fax |
| 5 | WhatsApp | WhatsApp |

#### Extension Methods

```csharp
+ GetDescricao(): string
+ IsCelular(): bool
+ IsComercial(): bool
+ IsResidencial(): bool
+ IsWhatsApp(): bool
+ IsFax(): bool
+ IsMovel(): bool            // Celular ou WhatsApp
+ IsFixo(): bool             // Residencial, Comercial ou Fax
+ PermiteMensagens(): bool   // Celular ou WhatsApp
```

---

### 6. TipoProvedor

**Namespace:** `ConsultaDocumentos.Domain.Enums`

**Descrição:** Tipos de provedores externos de dados.

| Valor | Nome | Descrição |
|-------|------|-----------|
| 0 | GENERICO | Provedor genérico |
| 1 | SERPRO | Serviço Federal de Processamento de Dados |
| 2 | SERASA | Serasa Experian |
| 3 | REPOSITORIO | Repositório local |
| 4 | BOAVISTA | Boa Vista SCPC |
| 5 | ENRIQUECIMENTO | Serviço de enriquecimento de dados |

---

### 7. SituacaoCadastral (Enum)

**Namespace:** `ConsultaDocumentos.Domain.Enums`

**Descrição:** Situação cadastral de documentos.

| Valor | Nome | Descrição |
|-------|------|-----------|
| 1 | Ativo | Cadastro ativo |
| 2 | Inativo | Cadastro inativo |
| 3 | Suspenso | Cadastro suspenso |
| 4 | Cancelado | Cadastro cancelado |
| 5 | Pendente | Cadastro pendente |
| 6 | Bloqueado | Cadastro bloqueado |

#### Extension Methods

```csharp
+ GetDescricao(): string
+ PermiteOperacoes(): bool   // Somente Ativo = true
+ IsAtivo(): bool
+ IsInativo(): bool
+ IsBloqueado(): bool        // Suspenso, Cancelado ou Bloqueado
+ IsPendente(): bool
```

---

## Padrões de Auditoria

O sistema implementa dois padrões de auditoria:

### 1. Rastreabilidade Completa

Aplicado em entidades administrativas que seguem DDD:

| Campo | Tipo | Descrição |
|-------|------|-----------|
| DataCriacao | DateTime | Data de criação UTC |
| DataAtualizacao | DateTime? | Data da última atualização UTC |
| CriadoPor | int? | ID do usuário criador |
| AtualizadoPor | int? | ID do último usuário que atualizou |

**Entidades:** Aplicacao, Autenticacao, AplicacaoProvedor, Usuario, Perfil, Provedor

### 2. Rastreabilidade Básica

Aplicado em entidades de dados de domínio:

| Campo | Tipo | Descrição |
|-------|------|-----------|
| DataCriacao | DateTime | Data de criação |
| DataAtualizacao | DateTime? | Data da última atualização (quando aplicável) |

**Entidades:** Email, Telefone, QuadroSocietario, Endereco

### 3. Controle de Concorrência Otimista

Aplicado para evitar conflitos em atualizações concorrentes:

| Campo | Tipo | Descrição |
|-------|------|-----------|
| RowVersion | byte[] | Token de versão do EF Core |

**Entidades:** Documento, Endereco

**Comportamento:**
- EF Core incrementa automaticamente o RowVersion a cada atualização
- Se duas transações tentarem atualizar o mesmo registro simultaneamente, a segunda receberá `DbUpdateConcurrencyException`

---

## Mapa de Dependências

### Aggregates Roots

O sistema possui **3 Aggregate Roots** seguindo DDD:

#### 1. Documento (Aggregate Root Principal)
```
Documento (Root)
├── Endereco (Entity)
├── Telefone (Entity)
├── Email (Entity)
├── QuadroSocietario (Entity)
├── → Nacionalidade (Reference)
└── → SituacaoCadastral (Reference)
```

**Regra:** Todas as operações em Endereco, Telefone, Email e QuadroSocietario devem passar pelo Documento.

---

#### 2. Aplicacao (Aggregate Root Administrativo)
```
Aplicacao (Root)
├── Autenticacao (Entity)
├── AplicacaoProvedor (Entity)
└── LogAuditoria (Entity - Write Only)
```

**Regra:** Tokens e provedores são gerenciados pela Aplicacao. LogAuditoria é somente escrita.

---

#### 3. Usuario (Aggregate Root de Segurança)
```
Usuario (Root)
└── UsuarioPerfil (Entity)
    └── → Perfil (Reference)
```

**Regra:** Perfis são associados ao usuário através de UsuarioPerfil.

---

### Hierarquia de Relacionamentos

```
┌─────────────────────────────────────────────────────────────────┐
│ NÍVEL 1: TABELAS DE REFERÊNCIA (Sem dependências)               │
└─────────────────────────────────────────────────────────────────┘
  - Nacionalidade
  - SituacaoCadastral
  - Perfil

┌─────────────────────────────────────────────────────────────────┐
│ NÍVEL 2: ENTIDADES PRINCIPAIS (Dependem apenas de Referência)   │
└─────────────────────────────────────────────────────────────────┘
  - Documento → Nacionalidade, SituacaoCadastral
  - Aplicacao (independente)
  - Usuario (independente)
  - Provedor (independente)

┌─────────────────────────────────────────────────────────────────┐
│ NÍVEL 3: ENTIDADES DEPENDENTES                                  │
└─────────────────────────────────────────────────────────────────┘
  - Endereco → Documento
  - Telefone → Documento
  - Email → Documento
  - QuadroSocietario → Documento
  - Autenticacao → Aplicacao
  - AplicacaoProvedor → Aplicacao
  - UsuarioPerfil → Usuario, Perfil

┌─────────────────────────────────────────────────────────────────┐
│ NÍVEL 4: LOGS E AUDITORIA (Somente escrita)                     │
└─────────────────────────────────────────────────────────────────┘
  - LogAuditoria → Aplicacao
  - LogErro → Aplicacao (opcional)
```

---

## Índices e Otimizações

### Índices Recomendados

#### TBL_DOCUMENTOS
```sql
-- Índice único para busca por número de documento
CREATE UNIQUE INDEX IX_Documento_Numero ON TBL_DOCUMENTOS(Numero);

-- Índice composto para filtros comuns
CREATE INDEX IX_Documento_TipoPessoa_DataConsulta ON TBL_DOCUMENTOS(TipoPessoa, DataConsulta);

-- Índice para validade
CREATE INDEX IX_Documento_DataConsultaValidade ON TBL_DOCUMENTOS(DataConsultaValidade);

-- Foreign Keys
CREATE INDEX IX_Documento_IdNacionalidade ON TBL_DOCUMENTOS(IdNacionalidade);
CREATE INDEX IX_Documento_IdSituacao ON TBL_DOCUMENTOS(IdSituacao);
```

#### TBL_ENDERECOS
```sql
-- Foreign Key
CREATE INDEX IX_Endereco_IdDocumento ON TBL_ENDERECOS(IdDocumento);

-- Busca por CEP
CREATE INDEX IX_Endereco_CEP ON TBL_ENDERECOS(CEP);

-- Busca por cidade/UF
CREATE INDEX IX_Endereco_Cidade_UF ON TBL_ENDERECOS(Cidade, UF);
```

#### TBL_TELEFONES
```sql
-- Foreign Key
CREATE INDEX IX_Telefone_IdDocumento ON TBL_TELEFONES(IdDocumento);

-- Busca por DDD
CREATE INDEX IX_Telefone_DDD ON TBL_TELEFONES(DDD);
```

#### TBL_EMAILS
```sql
-- Foreign Key
CREATE INDEX IX_Email_IdDocumento ON TBL_EMAILS(IdDocumento);

-- Busca por email
CREATE INDEX IX_Email_EnderecoEmail ON TBL_EMAILS(EnderecoEmail);
```

#### TBL_AUTENTICACAO
```sql
-- Foreign Key
CREATE INDEX IX_Autenticacao_AplicacaoId ON TBL_AUTENTICACAO(AplicacaoId);

-- Busca por token (única)
CREATE UNIQUE INDEX IX_Autenticacao_Token ON TBL_AUTENTICACAO(Token);

-- Filtros de status e validade
CREATE INDEX IX_Autenticacao_Status_DataFim ON TBL_AUTENTICACAO(Status, DataFim);
```

#### TBL_LOG_AUDITORIA
```sql
-- Foreign Key
CREATE INDEX IX_LogAuditoria_AplicacaoId ON TBL_LOG_AUDITORIA(AplicacaoId);

-- Busca por documento
CREATE INDEX IX_LogAuditoria_DocumentoNumero ON TBL_LOG_AUDITORIA(DocumentoNumero);

-- Filtro temporal (mais usado)
CREATE INDEX IX_LogAuditoria_DataHoraConsulta ON TBL_LOG_AUDITORIA(DataHoraConsulta DESC);

-- Busca por sucesso/falha
CREATE INDEX IX_LogAuditoria_ConsultaSucesso ON TBL_LOG_AUDITORIA(ConsultaSucesso);
```

#### TBL_USUARIOS
```sql
-- Login único
CREATE UNIQUE INDEX IX_Usuario_Login ON TBL_USUARIOS(Login);

-- Email único
CREATE UNIQUE INDEX IX_Usuario_Email ON TBL_USUARIOS(Email);

-- Filtro de status
CREATE INDEX IX_Usuario_Status ON TBL_USUARIOS(Status);
```

#### TBL_USUARIO_PERFIL
```sql
-- Chave composta (PK)
CREATE UNIQUE INDEX IX_UsuarioPerfil_Usuario_Perfil ON TBL_USUARIO_PERFIL(UsuarioId, PerfilId);

-- Foreign Keys
CREATE INDEX IX_UsuarioPerfil_PerfilId ON TBL_USUARIO_PERFIL(PerfilId);
```

---

## Boas Práticas de Uso

### 1. Sempre Usar Aggregates

❌ **Errado:**
```csharp
var endereco = new Endereco();
endereco.IdDocumento = 1;
await _enderecoRepository.AddAsync(endereco);
```

✅ **Correto:**
```csharp
var documento = await _documentoRepository.GetByIdAsync(1);
var endereco = Endereco.Criar(documento.IdDocumento, logradouro, ...);
documento.AdicionarEndereco(endereco);
await _unitOfWork.SaveChangesAsync();
```

### 2. Validar Antes de Persistir

✅ **Sempre validar:**
```csharp
var endereco = Endereco.Criar(...);
if (!endereco.IsValido())
    throw new DomainException("Endereço inválido");
```

### 3. Usar Factory Methods

✅ **Usar factory methods para criar entidades:**
```csharp
var documento = Documento.CriarPessoaFisica(numero, nome, ...);
var endereco = Endereco.Criar(documentoId, logradouro, ...);
```

### 4. Consultas Otimizadas

✅ **Usar Include para eager loading:**
```csharp
var documento = await _context.Documentos
    .Include(d => d.Enderecos)
    .Include(d => d.Telefones)
    .Include(d => d.Emails)
    .FirstOrDefaultAsync(d => d.Numero == numero);
```

### 5. Logs de Auditoria

✅ **Sempre registrar em LogAuditoria:**
```csharp
var log = new LogAuditoria(
    aplicacaoId, nomeAplicacao, documentoNumero,
    tipoDocumento, parametrosJson, provedoresJson,
    provedorPrincipal, sucesso, respostaJson,
    mensagem, tempoMs, ip, userAgent, token);
await _logRepository.AddAsync(log);
```

---

## Referências Técnicas

- **Entity Framework Core:** https://docs.microsoft.com/ef/core/
- **Domain-Driven Design:** Eric Evans, "Domain-Driven Design"
- **Clean Architecture:** Robert C. Martin, "Clean Architecture"
- **Repository Pattern:** Martin Fowler, "Patterns of Enterprise Application Architecture"

---

**Fim da Documentação**
