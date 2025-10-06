# Estrutura de Banco de Dados - Sistema de Consulta de Documentos

## Visão Geral

O sistema utiliza o **SQL Server** como banco de dados principal (`ConsultaDocumentos`) com uma arquitetura focada em **cache/repositório** de documentos consultados e **rastreabilidade** de uso de provedores (Serasa, SERPRO, BoaVista, etc.).

**Principais objetivos:**
1. **Cache de Consultas**: Armazenar documentos consultados para reduzir chamadas aos provedores externos
2. **Controle de Validade**: Gerenciar tempo de vida do cache (dias de validade diferentes para CPF/CNPJ)
3. **Auditoria**: Registrar todas as consultas realizadas
4. **Controle de Custo**: Rastrear qual sistema utilizou qual provedor

---

## Diagrama de Relacionamento (Conceitual)

```
┌─────────────────┐
│  TBL_SISTEMA    │ (Sistemas que usam o serviço)
└────────┬────────┘
         │
         │ IdSistema
         ▼
┌──────────────────────────┐
│ TBL_SISTEMA_WEBSERVICE   │ (Log de uso: Sistema + Provedor + Documento)
└────────┬────────┬────────┘
         │        │
         │        │ IdWebService
         │        ▼
         │   ┌─────────────────┐
         │   │ TBL_WEBSERVICE  │ (Configuração de Provedores)
         │   └─────────────────┘
         │
         │ IdDocumento
         ▼
┌─────────────────────────────┐
│     TBL_DOCUMENTO           │ (Cache de CPF/CNPJ)
├─────────────────────────────┤
│ - Dados Cadastrais          │
│ - Situação Cadastral        │
│ - Validade da Consulta      │
└────────┬────────┬───────────┘
         │        │
         │        └─────────────────┐
         │                          │
         ▼                          ▼
┌────────────────────┐   ┌──────────────────────────┐
│ TBL_ENDERECOS      │   │ TBL_QUADRO_SOCIETARIO    │
│ (Endereços do Doc) │   │ (Sócios - apenas CNPJ)   │
└────────────────────┘   └──────────────────────────┘
         │
         ▼
┌─────────────────────────────┐
│  TBL_SITUACAO_CADASTRAL     │ (Tabela de domínio)
└─────────────────────────────┘

         ┌─────────────────┐
         │  LogAuditoria   │ (Auditoria completa)
         └─────────────────┘
```

---

## Tabelas Principais

### 1. TBL_SISTEMA

**Descrição:** Cadastro de sistemas/aplicações que utilizam o serviço de consulta de documentos.

**Colunas:**
| Coluna       | Tipo          | Descrição                                    |
|--------------|---------------|----------------------------------------------|
| IdSistema    | INT (PK)      | Identificador único do sistema               |
| NomeSistema  | VARCHAR(50)   | Nome do sistema solicitante                  |
| Serpro       | BIT           | Flag indicando se sistema tem acesso SERPRO  |

**Stored Procedures:**
- `SPBR_TBL_Sistema_ValidarSistema` - Valida se o sistema existe

**Uso no Código:**
```csharp
// ServicoNegocio.cs:123
db.ExecuteReaderCommand("SPBR_TBL_Sistema_ValidarSistema", CommandType.StoredProcedure, lObjIListDbParameter)
```

**Exemplo de Dados:**
```sql
INSERT INTO TBL_SISTEMA VALUES (1, 'Sistema Portal')
INSERT INTO TBL_SISTEMA VALUES (2, 'Sistema Crédito')
```

---

### 2. TBL_WEBSERVICE

**Descrição:** Configuração dos provedores de dados (Serasa, SERPRO, BoaVista, etc.).

**Colunas:**
| Coluna                 | Tipo        | Descrição                                          |
|------------------------|-------------|----------------------------------------------------|
| IdWebService (PK)      | INT         | ID do provedor (1=SERPRO, 2=SERASA, 4=BoaVista...) |
| EndWebService          | VARCHAR(50) | URL/Endpoint do serviço                            |
| DesWebService          | VARCHAR(200)| Descrição do provedor                              |
| EndCertificado         | VARCHAR(50) | Caminho do certificado digital                     |
| Usuario                | VARCHAR(50) | Usuário de autenticação                            |
| Senha                  | VARCHAR(50) | Senha de autenticação                              |
| FlagAtivo              | VARCHAR(1)  | S/N - Se o provedor está ativo                     |
| TipoWebService         | INT         | Tipo de documento (1=CPF, 2=CNPJ, 3=Ambos)        |
| Dominio                | VARCHAR(50) | Domínio de autenticação (usado no Serasa)         |
| QtdAcessoMinimo        | INT         | Controle de cota mínima                            |
| QtdAcessoMaximo        | INT         | Controle de cota máxima                            |
| QtdDiasValidadePF      | INT         | Dias de validade para cache CPF                   |
| QtdDiasValidadePJ      | INT         | Dias de validade para cache CNPJ                  |
| QtdDiasValidadeEND     | INT         | Dias de validade para cache Endereços             |
| QtdMinEmailLog         | INT         | Qtd mínima de erros para enviar e-mail            |
| DiaCorte               | INT         | Dia de corte para faturamento                     |
| Porta                  | INT         | Porta de conexão (usado no BoaVista socket)       |
| Prioridade             | INT         | Ordem de prioridade (1=mais prioritário)          |

**Stored Procedures:**
- `SPBR_TBL_WebService_ConsultarPorTipo` - Busca provedores ativos por tipo e prioridade
- `SPBR_TBL_WebService_Atualizar` - Atualiza configuração do provedor
- `SPBR_TBL_WebService_Inserir` - Insere novo provedor

**Uso no Código:**
```csharp
// ServicoNegocio.cs:194
// Carrega provedores ordenados por prioridade
db.ExecuteReaderCommand("SPBR_TBL_WebService_ConsultarPorTipo", CommandType.StoredProcedure, ...)
```

**Exemplo de Dados:**
```sql
INSERT INTO TBL_WEBSERVICE VALUES
(1, 'https://api.serpro.gov.br', 'SERPRO', 'C:\cert.pfx', '', '', 'S', 3, '', NULL, NULL, 30, 30, 30, NULL, NULL, NULL, 2, NULL)

INSERT INTO TBL_WEBSERVICE VALUES
(2, 'https://www.experianmarketing.com.br', 'SERASA', NULL, 'usuario', 'senha', 'S', 3, 'dominio', NULL, NULL, 15, 15, 15, NULL, NULL, NULL, 1, NULL)
```

---

### 3. TBL_DOCUMENTO

**Descrição:** **Repositório/Cache** de documentos consultados (CPF e CNPJ).

**Colunas Principais:**
| Coluna                      | Tipo         | Descrição                                         |
|-----------------------------|--------------|---------------------------------------------------|
| IdDocumento (PK, Identity)  | INT          | Identificador único do documento no cache         |
| tpo_Pessoa                  | VARCHAR(1)   | Tipo: 'F'=CPF, 'J'=CNPJ                          |
| Numero                      | VARCHAR(18)  | Número do CPF/CNPJ                                |
| Nome                        | VARCHAR(100) | Nome/Razão Social                                 |
| NomeSocial                  | VARCHAR(100) | Nome social (CPF)                                 |
| IdSituacao (FK)             | INT          | FK para TBL_SITUACAO_CADASTRAL                   |
| DataSituacao                | DATETIME     | Data da situação cadastral                        |
| DataConsulta                | DATETIME     | Data/hora da consulta                             |
| DataConsultaValidade        | DATETIME     | Data até quando o cache é válido                  |
| CodControle                 | VARCHAR(30)  | Código de controle da consulta (Serasa)          |
| DataAbertura                | DATETIME     | Data de abertura/inscrição                        |
| DataFundacao                | SMALLDATETIME| Data de fundação (PJ)                             |

**Colunas CPF (Pessoa Física):**
| Coluna               | Tipo         | Descrição                                |
|----------------------|--------------|------------------------------------------|
| NomeMae              | VARCHAR(100) | Nome da mãe                              |
| DataNascimento       | DATETIME     | Data de nascimento                       |
| Sexo                 | VARCHAR(10)  | Sexo                                     |
| TituloEleitor        | VARCHAR(20)  | Número do título de eleitor              |
| AnoObito             | VARCHAR(10)  | Ano de óbito (se aplicável)              |
| ResidenteExterior    | VARCHAR(10)  | Indicador de residente no exterior (S/N) |
| idNacionalidade (FK) | INT          | FK para TBL_Nacionalidade                |

**Colunas CNPJ (Pessoa Jurídica):**
| Coluna                        | Tipo         | Descrição                                |
|-------------------------------|--------------|------------------------------------------|
| NomeFantasia                  | VARCHAR(100) | Nome fantasia                            |
| Inscricao                     | VARCHAR(50)  | Inscrição estadual/municipal             |
| NaturezaJuridica              | INT          | Código da natureza jurídica              |
| DescricaoNaturezaJuridica     | VARCHAR(100) | Descrição da natureza jurídica           |
| Segmento                      | VARCHAR(100) | Segmento de atuação                      |
| RamoAtividade                 | INT          | Código do ramo de atividade              |
| DescricaoRamoAtividade        | VARCHAR(100) | Descrição do ramo de atividade           |
| MatrizFilialQtde              | INT          | Quantidade de filiais                    |
| Matriz                        | BIT          | Flag se é matriz                         |
| Porte                         | VARCHAR(10)  | Porte da empresa                         |

**Stored Procedures:**
- `SPBR_CONSULTAR_TBL_DOCUMENTO` - **Principal**: Busca documento no cache com validação de prazo
- `SPBR_INCLUIR_TBL_DOCUMENTO` - Insere ou atualiza documento no cache
- `SPBR_ATUALIZAR_TBL_DOCUMENTO` - Atualiza dados do documento

**Lógica de Validade (SPBR_CONSULTAR_TBL_DOCUMENTO):**
```sql
-- Verifica validade do cache
AND (@DiasValidade = -1  -- -1 = ignora validade
     OR DateDiff(d, Cast(DataConsulta as date), Cast(getdate() as date)) <= @DiasValidade)
```

**Uso no Código:**
```csharp
// ServicoNegocio.cs:3492 - DoConsultaRepositorio
// Busca documento no cache
db.ExecuteReaderCommand("SPBR_CONSULTAR_TBL_DOCUMENTO", CommandType.StoredProcedure, lObjIListDbParameter)

// ServicoNegocio.cs:3209 - VerificarDadosRepositorio
// Insere/atualiza documento no cache após consulta ao provedor
db.ExecuteReaderCommand("SPBR_INCLUIR_TBL_DOCUMENTO", CommandType.StoredProcedure, lObjIListDbParameter)
```

---

### 4. TBL_SITUACAO_CADASTRAL

**Descrição:** Tabela de domínio com situações cadastrais possíveis.

**Colunas:**
| Coluna          | Tipo         | Descrição                                    |
|-----------------|--------------|----------------------------------------------|
| IdSituacao (PK) | INT          | ID da situação                               |
| SituacaoSerasa  | VARCHAR(50)  | Descrição da situação (REGULAR, SUSPENSA...) |

**Exemplos:**
```sql
INSERT INTO TBL_SITUACAO_CADASTRAL VALUES (0, 'REGULAR')
INSERT INTO TBL_SITUACAO_CADASTRAL VALUES (2, 'SUSPENSA')
INSERT INTO TBL_SITUACAO_CADASTRAL VALUES (3, 'CANCELADA')
INSERT INTO TBL_SITUACAO_CADASTRAL VALUES (4, 'BAIXADA')
```

---

### 5. TBL_SISTEMA_WEBSERVICE

**Descrição:** **Log de uso** - Registra qual sistema usou qual provedor para consultar qual documento.

**Colunas:**
| Coluna                          | Tipo         | Descrição                                |
|---------------------------------|--------------|------------------------------------------|
| IdSistemaWebservice (PK, Identity) | INT       | ID único do registro                     |
| IdSistema (FK)                  | INT          | FK para TBL_SISTEMA                      |
| IdWebService (FK)               | INT          | FK para TBL_WEBSERVICE                   |
| IdDocumento (FK)                | INT          | FK para TBL_DOCUMENTO                    |
| DataConsulta                    | DATETIME     | Data/hora da consulta                    |
| EnderecoIP                      | VARCHAR(50)  | IP da requisição                         |
| RemoteHost                      | VARCHAR(100) | Host remoto                              |

**Stored Procedures:**
- `SPBR_INCLUIR_TBL_SISTEMA_WEBSERVICE` - Insere log de uso
- `SPBR_CONSULTA_TBL_SISTEMA_WEBSERVICE` - Consulta logs de uso

**Uso no Código:**
```csharp
// ServicoNegocio.cs:3043 - InserirSistemaWebservice
// Registra que o sistema X usou o provedor Y para consultar o documento Z
db.ExecuteNonQueryCommand("SPBR_INCLUIR_TBL_SISTEMA_WEBSERVICE", CommandType.StoredProcedure, lObjIListDbParameter)
```

**Propósito:**
- **Billing/Faturamento**: Saber quantas consultas cada sistema fez em cada provedor
- **Auditoria**: Rastreabilidade completa de uso
- **Análise de Custo**: Calcular custo por sistema baseado no provedor utilizado

---

### 6. TBL_ENDERECOS

**Descrição:** Armazena endereços dos documentos consultados.

**Colunas:**
| Coluna            | Tipo         | Descrição                              |
|-------------------|--------------|----------------------------------------|
| idEndereco (PK)   | INT          | ID único do endereço                   |
| idDocumento (FK)  | INT          | FK para TBL_DOCUMENTO                  |
| Endereco          | VARCHAR(200) | Logradouro completo                    |
| Bairro            | VARCHAR(100) | Bairro                                 |
| CEP               | VARCHAR(10)  | CEP                                    |
| Cidade            | VARCHAR(100) | Cidade                                 |
| UF                | VARCHAR(2)   | Estado                                 |
| DDDTelefone       | VARCHAR(5)   | DDD do telefone                        |
| NumTelefone       | VARCHAR(20)  | Número do telefone                     |
| DataValidade      | DATETIME     | Data de validade do endereço no cache  |

**Stored Procedures:**
- `SPBR_CONSULTAR_TBL_ENDERECO` - Busca endereços do documento
- `SPBR_ATUALIZAR_INSERIR_TBL_ENDERECOS` - Insere ou atualiza endereço

**Observação:**
- Um documento pode ter múltiplos endereços (histórico)
- O endereço mais recente é obtido via `MAX(idEndereco)`

---

### 7. TBL_QUADRO_SOCIETARIO

**Descrição:** Armazena quadro societário de CNPJs (sócios/administradores).

**Colunas:**
| Coluna            | Tipo         | Descrição                              |
|-------------------|--------------|----------------------------------------|
| IdQuadroSoc (PK)  | INT          | ID único do sócio                      |
| IdDocumento (FK)  | INT          | FK para TBL_DOCUMENTO (CNPJ)           |
| Tipo              | VARCHAR(50)  | Tipo de sócio                          |
| Nome              | VARCHAR(200) | Nome do sócio                          |
| Numero            | VARCHAR(20)  | CPF/CNPJ do sócio                      |
| PercPartic        | DECIMAL      | Percentual de participação             |
| Qualificacao      | VARCHAR(100) | Qualificação do sócio                  |
| CodPaisOrigem     | VARCHAR(10)  | Código do país de origem               |
| NomePaisOrigem    | VARCHAR(100) | Nome do país de origem                 |

**Stored Procedures:**
- `SPBR_CONSULTAR_TBL_QUADRO_SOCIETARIO` - Busca sócios do CNPJ
- `SPBR_INCLUIR_QUADRO_SOCIETARIO` - Insere sócio

**Uso no Código:**
```csharp
// ServicoNegocio.cs:3320
// Insere cada sócio retornado pelo provedor (SERPRO/Serasa)
db.ExecuteNonQueryCommand("SPBR_INCLUIR_QUADRO_SOCIETARIO", CommandType.StoredProcedure, lObjIListDbParameterSoc)
```

---

### 8. TBL_DOCUMENTO_HIST

**Descrição:** Histórico de alterações em documentos (versionamento).

**Estrutura:** Mesma da TBL_DOCUMENTO, mas sem PK, com dados históricos.

**Propósito:**
- Manter histórico de mudanças de situação cadastral
- Compliance e rastreabilidade temporal

---

### 9. TBL_LOG_ERRO

**Descrição:** Log de erros da aplicação.

**Colunas:**
| Coluna          | Tipo         | Descrição                              |
|-----------------|--------------|----------------------------------------|
| IdLogErro (PK)  | INT          | ID único do erro                       |
| DataHora        | DATETIME     | Data/hora do erro                      |
| Mensagem        | VARCHAR(MAX) | Mensagem de erro                       |
| StackTrace      | VARCHAR(MAX) | Stack trace completo                   |
| Usuario         | VARCHAR(100) | Usuário relacionado (se houver)        |

**Stored Procedures:**
- `SPBR_INCLUIR_TBL_LOG_ERRO` - Insere erro no log
- `SPBR_CONSULTAR_TBL_LOG_ERRO` - Consulta erros

---

### 10. LogAuditoria

**Descrição:** **Auditoria completa** de todas as consultas realizadas no sistema.

**Colunas:**
| Coluna                  | Tipo         | Descrição                                          |
|-------------------------|--------------|---------------------------------------------------|
| Id (PK, AutoIncrement)  | INTEGER      | ID único do log                                   |
| AplicacaoId             | INTEGER      | ID da aplicação/sistema                           |
| NomeAplicacao           | TEXT(100)    | Nome da aplicação                                 |
| DocumentoNumero         | TEXT(20)     | Número do documento consultado                    |
| TipoDocumento           | INTEGER      | Tipo: 1=CPF, 2=CNPJ                              |
| ParametrosEntrada       | TEXT         | JSON com parâmetros da requisição                 |
| ProvedoresUtilizados    | TEXT         | JSON com lista de provedores tentados             |
| ProvedorPrincipal       | TEXT(50)     | Provedor que retornou os dados                    |
| ConsultaSucesso         | INTEGER      | 0=Erro, 1=Sucesso                                 |
| RespostaProvedor        | TEXT         | JSON com resposta completa do provedor            |
| MensagemRetorno         | TEXT(500)    | Mensagem de retorno (erro ou sucesso)             |
| TempoProcessamentoMs    | INTEGER      | Tempo de processamento em milissegundos           |
| DataHoraConsulta        | TEXT         | Timestamp da consulta (ISO 8601)                  |
| EnderecoIp              | TEXT(45)     | IP do cliente (IPv4/IPv6)                         |
| UserAgent               | TEXT(500)    | User-Agent da requisição HTTP                     |
| TokenAutenticacao       | TEXT(100)    | Hash do token de autenticação usado               |
| OrigemCache             | INTEGER      | 0=Provedor externo, 1=Cache/Repositório          |
| InformacoesAdicionais   | TEXT         | JSON com dados extras                             |

**Índices:**
```sql
IX_LogAuditoria_AplicacaoId
IX_LogAuditoria_DataHoraConsulta
IX_LogAuditoria_DocumentoNumero
IX_LogAuditoria_AplicacaoId_DataHoraConsulta
IX_LogAuditoria_DocumentoNumero_TipoDocumento
IX_LogAuditoria_DataHora_Sucesso_Aplicacao
```

**Propósito:**
- **Compliance**: Rastreamento completo de todas as consultas
- **Performance Monitoring**: Tempo de resposta de cada provedor
- **Análise de Uso**: Quais documentos são mais consultados
- **Debugging**: Resposta completa armazenada para análise

---

## Fluxo de Dados nas Consultas

### Fluxo Completo (Serasa/SERPRO)

```
┌─────────────────────────────────────────────────────┐
│ 1. VALIDAÇÃO INICIAL                                │
│    - Validar documento (checksum CPF/CNPJ)          │
│    - Validar sistema (SPBR_TBL_Sistema_ValidarSistema)│
└─────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────┐
│ 2. CARREGAR PROVEDORES                              │
│    - SPBR_TBL_WebService_ConsultarPorTipo           │
│    - Retorna provedores ativos por ordem de         │
│      prioridade (1=mais prioritário)                │
│    - Carga fica em memória cache                    │
└─────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────┐
│ 3. CONSULTAR REPOSITÓRIO/CACHE                      │
│    - SPBR_CONSULTAR_TBL_DOCUMENTO                   │
│    - Verifica se documento existe                   │
│    - Valida prazo de validade (DiasValidade)        │
└─────────────────────────────────────────────────────┘
                         │
                ┌────────┴────────┐
                │                 │
          Cache HIT          Cache MISS
                │                 │
                ▼                 ▼
        ┌──────────────┐   ┌──────────────────────────┐
        │ Retorna      │   │ 4. CONSULTAR PROVEDOR    │
        │ dados do     │   │    (fallback chain)      │
        │ cache        │   │                          │
        └──────────────┘   │ For each provedor:       │
                           │   - Serpro               │
                           │   - Serasa               │
                           │   - BoaVista             │
                           │   - Enriquecimento       │
                           │                          │
                           │ Se todos falharem:       │
                           │   → Tenta SERPRO         │
                           │     (fallback final)     │
                           └────────┬─────────────────┘
                                    │
                                    ▼
                           ┌──────────────────────────┐
                           │ 5. ARMAZENAR NO CACHE    │
                           │    - SPBR_INCLUIR_TBL_   │
                           │      DOCUMENTO           │
                           │    - Calcula validade:   │
                           │      DataConsulta +      │
                           │      QtdDiasValidade     │
                           └────────┬─────────────────┘
                                    │
                                    ▼
                           ┌──────────────────────────┐
                           │ 6. REGISTRAR USO         │
                           │    - SPBR_INCLUIR_TBL_   │
                           │      SISTEMA_WEBSERVICE  │
                           │    - Grava:              │
                           │      * Sistema           │
                           │      * Provedor usado    │
                           │      * Documento         │
                           │      * IP/Host           │
                           └────────┬─────────────────┘
                                    │
                                    ▼
                           ┌──────────────────────────┐
                           │ 7. LOG DE AUDITORIA      │
                           │    - LogAuditoria        │
                           │    - Registra tudo:      │
                           │      * Parâmetros        │
                           │      * Provedores        │
                           │      * Resposta          │
                           │      * Tempo             │
                           │      * Sucesso/Erro      │
                           └──────────────────────────┘
```

---

## Exemplo Prático: Consulta CPF

### Cenário: Sistema "Portal" consulta CPF "12345678901"

#### 1. Validação do Sistema
```sql
EXEC SPBR_TBL_Sistema_ValidarSistema @IdSistema = 1
-- Retorna: Sistema válido
```

#### 2. Carregar Provedores
```sql
EXEC SPBR_TBL_WebService_ConsultarPorTipo
    @FlagAtivo = 'S',
    @TipoWebService = 1, -- CPF
    @IdSistema = 1

-- Retorna (ordenado por Prioridade):
-- 1. Serasa (Prioridade 1, QtdDiasValidadePF=15)
-- 2. SERPRO (Prioridade 2, QtdDiasValidadePF=30)
```

#### 3. Consultar Cache
```sql
EXEC SPBR_CONSULTAR_TBL_DOCUMENTO
    @tpo_Pessoa = 'F',
    @Numero = '12345678901',
    @Endereco = 0,
    @DiasValidade = 15

-- Cenário A: Documento encontrado e válido → Retorna dados do cache
-- Cenário B: Não encontrado ou vencido → Continua para provedor
```

#### 4. Consultar Provedor (se cache miss)
```csharp
// Tenta Serasa (prioridade 1)
DoConsultaSerasaPF(...)
// Se falhar, tenta SERPRO (prioridade 2)
DoConsultaRFB(...)
```

#### 5. Armazenar no Cache
```sql
EXEC SPBR_INCLUIR_TBL_DOCUMENTO
    @IdDocumento = NULL, -- Novo registro
    @tpo_Pessoa = 'F',
    @Numero = '12345678901',
    @Nome = 'JOÃO DA SILVA',
    @IdSituacao = 0, -- REGULAR
    @DataConsulta = '2025-10-05 10:30:00',
    @DataConsultaValidade = '2025-10-20 10:30:00', -- +15 dias (Serasa)
    @NomeMae = 'MARIA DA SILVA',
    @DataNascimento = '1980-01-01',
    ... -- outros campos

-- Retorna: IdDocumento = 12345
```

#### 6. Registrar Uso
```sql
EXEC SPBR_INCLUIR_TBL_SISTEMA_WEBSERVICE
    @IdSistema = 1,        -- Portal
    @IdWebService = 2,     -- Serasa
    @IdDocumento = 12345,  -- Documento recém inserido
    @DataConsulta = GETDATE(),
    @EnderecoIP = '192.168.1.100',
    @RemoteHost = 'portal.empresa.com.br'

-- Grava log de uso para billing
```

#### 7. Auditoria
```sql
INSERT INTO LogAuditoria (
    AplicacaoId, NomeAplicacao, DocumentoNumero, TipoDocumento,
    ParametrosEntrada, ProvedoresUtilizados, ProvedorPrincipal,
    ConsultaSucesso, RespostaProvedor, TempoProcessamentoMs,
    DataHoraConsulta, EnderecoIp, UserAgent, OrigemCache
) VALUES (
    1, 'Portal', '12345678901', 1,
    '{"pStrRepositorio":"S","pStrVencido":"N"}',
    '["Serasa"]',
    'Serasa',
    1, -- Sucesso
    '{"CPF":"12345678901","Nome":"JOÃO DA SILVA",...}',
    1250, -- 1.25s
    '2025-10-05T10:30:00',
    '192.168.1.100',
    'Mozilla/5.0...',
    0 -- Veio do provedor, não do cache
)
```

---

## Gerenciamento de Validade do Cache

### Dias de Validade por Provedor

```sql
SELECT
    IdWebService,
    DesWebService,
    QtdDiasValidadePF,  -- Validade cache CPF
    QtdDiasValidadePJ,  -- Validade cache CNPJ
    QtdDiasValidadeEND  -- Validade cache Endereços
FROM TBL_WEBSERVICE
```

**Exemplo:**
- **Serasa**: 15 dias (CPF), 15 dias (CNPJ), 15 dias (Endereços)
- **SERPRO**: 30 dias (CPF), 30 dias (CNPJ), 30 dias (Endereços)
- **BoaVista**: 10 dias (CPF), 10 dias (CNPJ), 10 dias (Endereços)

### Cálculo de Validade

```csharp
// ServicoNegocio.cs - VerificarDadosRepositorio
DateTime dataConsultaValidade = DateTime.Today.AddDays(qtdDiasValidade);
```

**Lógica:**
1. Ao consultar provedor, o sistema:
   - Pega `QtdDiasValidadePF` ou `QtdDiasValidadePJ` do provedor usado
   - Calcula: `DataConsultaValidade = DataConsulta + QtdDiasValidade`
   - Armazena no `TBL_DOCUMENTO`

2. Em consultas futuras:
   - Compara `DataConsultaValidade` com data atual
   - Se vencido → Consulta provedor novamente
   - Se válido → Retorna do cache

---

## Consultas Úteis para Análise

### 1. Ver documentos no cache e validade
```sql
SELECT
    Numero,
    Nome,
    DataConsulta,
    DataConsultaValidade,
    CASE
        WHEN DataConsultaValidade >= GETDATE() THEN 'VÁLIDO'
        ELSE 'VENCIDO'
    END AS Status,
    DATEDIFF(DAY, DataConsulta, DataConsultaValidade) AS DiasValidade
FROM TBL_DOCUMENTO
ORDER BY DataConsulta DESC
```

### 2. Uso por sistema/provedor (faturamento)
```sql
SELECT
    S.NomeSistema,
    W.DesWebService,
    COUNT(*) AS QtdConsultas,
    MIN(SW.DataConsulta) AS PrimeiraConsulta,
    MAX(SW.DataConsulta) AS UltimaConsulta
FROM TBL_SISTEMA_WEBSERVICE SW
INNER JOIN TBL_SISTEMA S ON S.IdSistema = SW.IdSistema
INNER JOIN TBL_WEBSERVICE W ON W.IdWebService = SW.IdWebService
GROUP BY S.NomeSistema, W.DesWebService
ORDER BY QtdConsultas DESC
```

### 3. Taxa de acerto do cache
```sql
SELECT
    COUNT(CASE WHEN OrigemCache = 1 THEN 1 END) AS ConsultasCache,
    COUNT(CASE WHEN OrigemCache = 0 THEN 1 END) AS ConsultasProvedor,
    CAST(COUNT(CASE WHEN OrigemCache = 1 THEN 1 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) AS TaxaCache
FROM LogAuditoria
WHERE DataHoraConsulta >= DATEADD(DAY, -30, GETDATE())
```

### 4. Tempo médio de resposta por provedor
```sql
SELECT
    ProvedorPrincipal,
    AVG(TempoProcessamentoMs) AS TempoMedio_ms,
    MIN(TempoProcessamentoMs) AS TempoMinimo_ms,
    MAX(TempoProcessamentoMs) AS TempoMaximo_ms,
    COUNT(*) AS QtdConsultas
FROM LogAuditoria
WHERE ConsultaSucesso = 1
  AND DataHoraConsulta >= DATEADD(DAY, -7, GETDATE())
GROUP BY ProvedorPrincipal
ORDER BY TempoMedio_ms
```

### 5. Documentos mais consultados
```sql
SELECT TOP 10
    DocumentoNumero,
    TipoDocumento,
    COUNT(*) AS QtdConsultas,
    COUNT(DISTINCT AplicacaoId) AS QtdAplicacoes
FROM LogAuditoria
WHERE DataHoraConsulta >= DATEADD(MONTH, -1, GETDATE())
GROUP BY DocumentoNumero, TipoDocumento
ORDER BY QtdConsultas DESC
```

---

## Stored Procedures - Resumo

| Stored Procedure                        | Descrição                                        |
|-----------------------------------------|--------------------------------------------------|
| SPBR_TBL_Sistema_ValidarSistema         | Valida se sistema existe                         |
| SPBR_TBL_WebService_ConsultarPorTipo    | Busca provedores por tipo e prioridade           |
| SPBR_CONSULTAR_TBL_DOCUMENTO            | **Principal**: Busca documento no cache          |
| SPBR_INCLUIR_TBL_DOCUMENTO              | Insere/atualiza documento no cache               |
| SPBR_ATUALIZAR_TBL_DOCUMENTO            | Atualiza documento existente                     |
| SPBR_INCLUIR_TBL_SISTEMA_WEBSERVICE     | Registra uso (sistema + provedor + documento)   |
| SPBR_CONSULTA_TBL_SISTEMA_WEBSERVICE    | Consulta logs de uso                             |
| SPBR_CONSULTAR_TBL_QUADRO_SOCIETARIO    | Busca sócios de CNPJ                             |
| SPBR_INCLUIR_QUADRO_SOCIETARIO          | Insere sócio                                     |
| SPBR_CONSULTAR_TBL_ENDERECO             | Busca endereços do documento                     |
| SPBR_ATUALIZAR_INSERIR_TBL_ENDERECOS    | Insere/atualiza endereço                         |
| SPBR_INCLUIR_TBL_LOG_ERRO               | Registra erro                                    |

---

## Considerações de Performance

### Índices Importantes

**TBL_DOCUMENTO:**
```sql
CREATE INDEX IX_TBL_DOCUMENTO_Numero ON TBL_DOCUMENTO(Numero, tpo_Pessoa)
CREATE INDEX IX_TBL_DOCUMENTO_DataConsultaValidade ON TBL_DOCUMENTO(DataConsultaValidade)
```

**TBL_SISTEMA_WEBSERVICE:**
```sql
CREATE INDEX IX_SISTEMA_WEBSERVICE_IdSistema ON TBL_SISTEMA_WEBSERVICE(IdSistema)
CREATE INDEX IX_SISTEMA_WEBSERVICE_IdWebService ON TBL_SISTEMA_WEBSERVICE(IdWebService)
CREATE INDEX IX_SISTEMA_WEBSERVICE_DataConsulta ON TBL_SISTEMA_WEBSERVICE(DataConsulta)
```

**TBL_QUADRO_SOCIETARIO:**
```sql
CREATE INDEX IX_TBL_QUADRO_SOCIETARIO_01 ON TBL_QUADRO_SOCIETARIO(IdDocumento)
```

### NOLOCK Hints

Todas as consultas utilizam `WITH (NOLOCK)` para evitar locks de leitura:
```sql
FROM TBL_DOCUMENTO TD WITH(NOLOCK)
```

### Cache em Memória

```csharp
// Setup.cs - Configuração de provedores é cacheada
var cache = MemoryCache.Default;
cache.Add("DadosProvedores", provedores, DateTimeOffset.Now.AddMinutes(tempoCacheDadosProvedores));
```

---

## Observações Finais

1. **Cache Inteligente**: Sistema privilegia cache para reduzir custo com provedores externos
2. **Rastreabilidade Total**: Cada consulta é auditada (quem, quando, qual provedor, quanto tempo)
3. **Controle de Custo**: TBL_SISTEMA_WEBSERVICE permite billing preciso por sistema
4. **Validade Dinâmica**: Cada provedor pode ter prazo de validade diferente
5. **Fallback Chain**: Se um provedor falha, tenta o próximo automaticamente
6. **Histórico**: TBL_DOCUMENTO_HIST mantém versões anteriores dos documentos

**Banco de Dados:** `ConsultaDocumentos` (SQL Server)