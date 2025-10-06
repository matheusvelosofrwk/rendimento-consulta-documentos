# Análise Técnica Detalhada: Sistema de Consulta de Documentos (CPF/CNPJ)

## 1. ARQUITETURA GERAL

### 1.1 Camadas da Aplicação

```
┌─────────────────────────────────────┐
│   ASMX Web Services (API Layer)     │  ← ConsultarCompleto.asmx
├─────────────────────────────────────┤
│   ServicoNegocio (Orchestrator)     │  ← Lógica principal de negócio
├─────────────────────────────────────┤
│   Provider Layer                     │  ← SERPRO, BoaVista, Enriquecimento
├─────────────────────────────────────┤
│   Repository/Cache (TBL_DOCUMENTO)  │  ← Cache local SQL Server
├─────────────────────────────────────┤
│   Database (SQL Server)              │  ← Persistência e auditoria
└─────────────────────────────────────┘
```

## 2. FLUXO DE EXECUÇÃO COMPLETO

### 2.1 Ponto de Entrada - Web Service ASMX

**Arquivo:** `ConsultarCompleto.asmx.cs:29-39`

```csharp
public string Consultar(int pIntTipo, string pStrDocumento, int pIntSistema, 
                        string pStrRepositorio, string pStrVencido)
{
    ServicoNegocio lObjConsultaDocumentos = new ServicoNegocio();
    return lObjConsultaDocumentos.Consultar(1, pIntTipo, pStrDocumento,
                                            pIntSistema, pStrRepositorio, pStrVencido);
}
```

**Parâmetros:**
- `pIntTipo`: 1=CPF, 2=CNPJ
- `pStrDocumento`: Número do documento (sem formatação)
- `pIntSistema`: ID do sistema solicitante (para rateio de custos)
- `pStrRepositorio`: "S"=usa cache, "N"=vai direto ao provedor
- `pStrVencido`: "S"=aceita dados expirados, "N"=força atualização

### 2.2 Orquestrador Principal - ServicoNegocio.Consultar()

**Arquivo:** `ServicoNegocio.cs:304-385`

#### Etapas de Execução:

**1. Validações Iniciais (linhas 310-330)**
- Valida `pIntTipo ∈ {1,2}`
- Valida `pStrDocumento` não nulo/vazio
- Valida `pIntSistema` existe em TBL_SISTEMA via `SPBR_TBL_Sistema_ValidarSistema`

**2. Preparação (linhas 332-353)**
- Define TLS 1.2 para conexões seguras
- Limpa documento (remove formatação)
- Carrega provedores via `ObterDadosProvedores(pIntSistema)`
- Define defaults: `pStrVencido="N"`, `pStrRepositorio="S"`

**3. Validação de Documento (linha 356)**

**Algoritmo de Validação CPF/CNPJ:**

CPF (`ServicoNegocio.cs:4941+`):
- 11 dígitos
- Algoritmo módulo 11 (dígitos verificadores)
- Rejeita sequências repetidas (111.111.111-11, etc)

CNPJ (`ServicoNegocio.cs:4965+`):
- 14 dígitos
- Algoritmo módulo 11 (dois dígitos verificadores)
- Rejeita sequências repetidas e padrões inválidos

**4. Consulta ao Repositório/Cache (linha 361)**

```csharp
if (pStrRepositorio.ToUpper() != "N") {
    lStrRetorno = DoConsultaRepositorio(...)
}
```

**5. Decisão de Consultar Provedores Externos (linhas 364-374)**

Cache Miss Triggers:
```csharp
lStrRetorno.Length < 100 && (
    lStrRetorno == "DESCONECTADO"      ||  // BD indisponível
    lStrRetorno == "NAO_ENCONTRADO"    ||  // Documento não existe em cache
    lStrRetorno == "VENCIDO"           ||  // Documento expirado
    lStrRetorno == "COM_RESTRICAO"     ||  // Documento com restrições
    lStrRetorno.Trim() == string.Empty     // Cache vazio
)
→ ObtemConsulta(...) // Aciona provedores externos
```

### 2.3 Camada de Cache/Repositório - DoConsultaRepositorio()

**Arquivo:** `ServicoNegocio.cs:3418-3720`

**Stored Procedure Principal:**

```sql
SPBR_CONSULTAR_TBL_DOCUMENTO
Parâmetros:
  @tpo_Pessoa: 'F' (CPF) ou 'J' (CNPJ)
  @Numero: Documento
  @Endereco: 1 (consulta com endereço)
```

**Lógica de Validade (linhas 3637-3659):**

```csharp
// Validação simples (por data de expiração)
if (!pStrVencido.Equals("S")) {
    if (lDatDataAtual > lDatDataConsultaValidade) {
        return "VENCIDO";
    }
}

// Validação por dias de validade (ConsultarPorValidade)
if (pDiasValidade != -1) {
    if (lDatDataAtual > lDatDataConsultaValidade.AddDays(pDiasValidade)) {
        return "VENCIDO";
    }
}
```

**Campos Recuperados do Cache:**
- **Básicos:** Nome, Situação, DataConsulta, DataConsultaValidade
- **CPF:** NomeMae, DataNascimento, Sexo, TituloEleitor, AnoObito, NomeSocial
- **CNPJ:** DataAbertura, Inscricao, NaturezaJuridica, RamoAtividade, Porte, NomeFantasia
- **Endereço:** Logradouro, Bairro, CEP, UF, Cidade, DDD, Telefone
- **Quadro Societário:** Via `SPBR_CONSULTAR_TBL_QUADRO_SOCIETARIO`

## 3. ORQUESTRAÇÃO DE PROVEDORES - ObtemConsulta()

**Arquivo:** `ServicoNegocio.cs:560-688`

### 3.1 Provider Configuration (linhas 138-291)

**Carregamento Dinâmico via SQL:**
```sql
SPBR_TBL_WebService_ConsultarPorTipo
Parâmetros:
  @TipoWebService = 1
  @IdSistema = pIntSistema
```

**Estrutura DadosProvedor:**
```javascript
{
    ID: 1=SERPRO, 2=SERASA, 3=REPOSITORIO, 4=BoaVista, 5=EnriquecimentoDados
    Endereco: URL/IP do provedor
    Porta: Porta de conexão
    Prioridade: Ordem de tentativa (1=primeiro)
    Dominio, Usuario, Senha: Credenciais (senha criptografada)
    Ativo: true/false
}
```

**Lógica de Priorização (linhas 238-244):**
```csharp
// Sistemas podem inverter prioridade SERPRO/EnriquecimentoDados
if (campo.Serpro == true) {
    if (ID == 1) Prioridade = 1;  // SERPRO primeiro
    if (ID == 5) Prioridade = 2;  // EnriquecimentoDados segundo
}
```

### 3.2 Fallback Chain Logic

**Arquivo:** `ServicoNegocio.cs:575-686`

```csharp
while (load) {
    switch (provedor.ID) {
        case 1: DoConsultaRFB()          // SERPRO
        case 2: DoConsultaSerasa()       // SERASA
        case 4: DoConsultaBoaVista()     // BoaVista
        case 5: DoConsultaEnriquecimento() // EnriquecimentoDados
    }

    if (response.Contains("Exception") || response.Contains("Mensagem")) {
        // Erro → tenta próximo provedor
        mIntIndiceProvedorAtual++;
        continue;
    } else {
        // Sucesso → finaliza
        break;
    }
}

// Fallback final: sempre tenta SERPRO se ainda não executado
if (todos_falharam && !mBolProvedorSERPROExecutado) {
    mBolFinalizarConsulta = true;
    DoConsultaRFB(...);
}
```

### 3.3 Modo Contingência (linhas 152-181)

**Configuração:** `contingencia_desconectada_ativa=S` em Web.Config

```csharp
if (contingenciaAtiva) {
    // Usa APENAS repositório local (offline mode)
    // Provedor de contingência configurado:
    //   - ID configurável
    //   - Endpoint alternativo
    //   - Credenciais específicas
    return somente_repositorio();
}
```

## 4. IMPLEMENTAÇÃO DOS PROVEDORES

### 4.1 BoaVista - Protocolo Socket Proprietário

**Arquivo:** `ServicoProvedorBoaVista.cs`

**Comunicação Socket (linhas 168-193):**
- **Protocolo:** TCP direto (System.Net.Sockets.TcpClient)
- **Encoding:** UTF-8
- **Buffer:** 10.000 bytes fixo
- **Timeout:** 10 segundos (send + receive)
- **Mensagens:** CSR60 (request) → CSR61 (response)

**Protocolo CSR (Credit Service Request):**

**CSR60 - Request Message:**
```
Layout fixo posicional:
[Transacao(8)][Versao(2)][ReservadoSol(10)][ReservadoBVS(20)]
[Codigo(8)][Senha(8)][Consulta(8)][VersaoConsulta(2)]
[TipoResposta(1)][TipoTrans(1)][TipoDoc(1)][Documento(14)]
[Email(50)][CodigoFacilitador(8)]\n
```

**Tipos de Consulta:**
- CADAPF - Cadastro Pessoa Física (CPF)
- CADPJ - Cadastro Pessoa Jurídica (CNPJ)

**Tipos de Resposta:**
- 1 - Resumo (dados básicos)
- 2 - Analítico (dados completos)

**CSR61 - Response Message:**

Parser com Registros Tipados (`ProvedorBoaVista/CSR.cs`):

| Tipo | Descrição      | Parser                    |
|------|----------------|---------------------------|
| 308  | CPF Resumo     | CSR61PFRegistroTipo308.cs |
| 334  | CPF Analítico  | CSR61PFRegistroTipo334.cs |
| 332  | CPF Endereços  | CSR61PFRegistroTipo332.cs |
| 335  | Telefones      | CSR61PFRegistroTipo335.cs |
| 353  | CNPJ Analítico | CSR61PJRegistroTipo353.cs |
| 901  | Alerta         | CSR61RegistroTipo901.cs   |
| 999  | Erro           | CSR61RegistroTipo999.cs   |

**Métodos de Parsing (`CSR.cs:43-140`):**
```csharp
LerCampoLong(texto, ref indice, tamanho)    // Números longos
LerCampoInteiro(texto, ref indice, tamanho) // Inteiros
LerCampoAlfa(texto, ref indice, tamanho)    // Texto (trim)
LerCampoTexto(texto, ref indice, tamanho)   // Texto (sem trim)
LerCampoData(texto, ref indice, tamanho)    // Datas DDMMAAAA
```

### 4.2 SERPRO (Receita Federal) - HTTP/REST

**Arquivo:** `ServicoNegocio.cs:768+` (DoConsultaRFB)

**Características:**
- **Protocolo:** HTTPS/REST com TLS 1.2
- **Autenticação:** Certificado digital (.pfx)
- **Endpoint CPF:** `https://acesso.infoconv.receita.fazenda.gov.br/ws/cpfrest/api/ConsultaCpf/ConsultarCPFD3`

**Configuração:**
```xml
<add key="certificado" value="C:\Certificates\infoconv_2025.pfx"/>
<add key="senha_certificado" value="Q+NdXSzzmr5838OvSb/oLA==" /> <!-- Criptografada -->
```

**Controle de Quota (linhas 268-278):**
```csharp
if (provedor_atual == SERPRO && prioridade == 1) {
    int qtdeMaxima = RetornaQtdeAcessoMaximo(SERPRO);
    int totalConsultas = RetornaTotalConsultas(SERPRO);

    if (totalConsultas >= qtdeMaxima) {
        // Muda para segundo provedor na fila
        mIntIndiceProvedorAtual = 2;
    }
}
```

### 4.3 Enriquecimento de Dados - HTTP/REST

**Arquivo:** `ServicoEnriquecimentoDados.cs`

**Autenticação JWT (linhas 75-99):**

```http
POST /api/enriquecimento/v1/autenticacao
{
    "empresaId": 1,
    "aplicacaoId": 15,
    "chaveAcesso": "922e9c67-ad12-4eb8-8f6d-66b1185b82bf"
}
→ Response: { "success": true, "data": { "data": "JWT_TOKEN" } }

// Requisições subsequentes:
Authorization: Bearer {JWT_TOKEN}
```

**Endpoints:**
```http
// Dados básicos
GET /api/enriquecimento/v1/pessoas/dadosbasicos?doc={cpf}

// Endereços estendidos
GET /api/enriquecimento/v1/pessoas/enderecosestendido?doc={cpf}
```

**Async/Await Pattern (linhas 103-125):**
```csharp
public async Task<InfoData> ConsultarDadosBasicosPessoaFisica(string doc) {
    var token = await Autenticacao();
    var result = await RequestAsync($"{url}?doc={doc}", GET, token: token);
    return JsonConvert.DeserializeObject<InfoData>(result);
}
```

## 5. PERSISTÊNCIA E AUDITORIA

### 5.1 Tabelas Principais

**TBL_DOCUMENTO - Cache de documentos**

Campos-chave:
- IdDocumento (PK)
- TpoDocumento ('F'/'J')
- Numero (CPF/CNPJ)
- DataConsulta
- DataConsultaValidade
- Situacao
- OrigemConsulta (provedor que retornou)

**TBL_QUADRO_SOCIETARIO - Sócios/Administradores**

Campos:
- Tipo, Nome, Numero (CPF/CNPJ)
- Qualificacao
- CodPaisOrigem, NomePaisOrigem

**TBL_LOG_ERRO - Logs de erros**

Campos:
- IdProvedor, IdSistema
- Documento, Mensagem
- Source, StackTrace

**LogAuditoria - Auditoria completa** (`BD/create_log_auditoria.sql`)

Campos:
- AplicacaoId, NomeAplicacao
- DocumentoNumero, TipoDocumento
- ParametrosEntrada (JSON)
- ProvedoresUtilizados (JSON)
- ProvedorPrincipal
- ConsultaSucesso (0/1)
- RespostaProvedor (JSON)
- TempoProcessamentoMs
- DataHoraConsulta
- EnderecoIp, UserAgent
- TokenAutenticacao (hash)
- OrigemCache (0/1)

### 5.2 Stored Procedures Principais

| Procedure                            | Função                           |
|--------------------------------------|----------------------------------|
| SPBR_TBL_Sistema_ValidarSistema      | Valida ID do sistema solicitante |
| SPBR_TBL_WebService_ConsultarPorTipo | Carrega provedores ativos        |
| SPBR_CONSULTAR_TBL_DOCUMENTO         | Consulta cache de documentos     |
| SPBR_ATUALIZAR_TBL_DOCUMENTO         | Atualiza documento em cache      |
| SPBR_INCLUIR_TBL_DOCUMENTO           | Insere novo documento            |
| SPBR_CONSULTAR_TBL_QUADRO_SOCIETARIO | Consulta sócios                  |

### 5.3 Logging

**log4net Configuration** (`Web.Config:17-38`):
```xml
<appender name="LogFileAppender">
  <param name="File" value="ApplicationLog.log"/>
  <layout type="log4net.Layout.PatternLayout">
    <param name="ConversionPattern" value="%d [%t] %-5p %m%n"/>
  </layout>
</appender>
```

**Níveis de Log:**
- **DEBUG** - Mensagens detalhadas BoaVista (se `trace_boavista=S`)
- **INFO** - Operações normais
- **ERROR** - Exceções e falhas

## 6. REGRAS DE NEGÓCIO CRÍTICAS

### 6.1 Rateio de Custos

**Obrigatório:** Toda consulta DEVE ter `pIntSistema` válido
- Validado contra TBL_SISTEMA
- Usado para billing/reporting de custos por sistema

### 6.2 Validade de Cache

**Estratégias:**

**1. Validade Simples:**
```
DataAtual > DataConsultaValidade
→ Retorna "VENCIDO" (se pStrVencido="N")
```

**2. Validade por Janela (ConsultarPorValidade):**
```
DataAtual > DataConsultaValidade.AddDays(pDiasValidade)
pDiasValidade = -1 → ignora expiração (sempre retorna cache)
pDiasValidade = 0+ → só retorna se dentro de N dias
```

**3. Cache-only Mode:**
```
pPesquisaProvedor = false
→ Nunca consulta provedores, só retorna cache
```

### 6.3 Situações de Documentos

**CPF (RetornaSituacaoRFBPF):**
- 0 = Regular
- 2 = Suspenso
- 4 = Cancelado por óbito
- Outros = Situações especiais

**CNPJ (RetornaSituacaoRFBPJ):**
- 2 = Ativa
- 3 = Suspensa
- 4 = Inapta
- Outros = Baixada, Nula, etc

### 6.4 Tratamento de Erros

**Fallback Cascade** (`ServicoNegocio.cs:691-761`):
```csharp
try {
    resultado = ConsultaProvedor();
} catch (Exception ex) {
    InserirLogErro(provedor, sistema, doc, ex.Message);

    if (tem_proximo_provedor) {
        tenta_proximo();
    } else if (!serpro_tentado) {
        tenta_serpro_final();
    } else {
        retorna_xml_erro();
    }
}
```

## 7. CONFIGURAÇÕES CRÍTICAS (Web.Config)

### 7.1 Connection Strings

```xml
<!-- Principal -->
<add key="ConexaoConsultaDocumentos" 
     value="Data Source=REND-SRVTSQL-05;
            Integrated Security=SSPI;
            DATABASE=ConsultaDocumentos;
            Application Name=Rendimento.ConsultaDocumentos.ASPNetApp.WebService;"/>
```

### 7.2 Provedores

```xml
<!-- BoaVista -->
<add key="timeout_serasa" value="30000"/> <!-- 30 segundos -->
<add key="trace_boavista" value="S"/>     <!-- Debug logs -->

<!-- SERPRO -->
<add key="certificado" value="C:\Certificates\infoconv_2025.pfx"/>
<add key="senha_certificado" value="Q+NdXSzzmr5838OvSb/oLA=="/> <!-- Criptografado -->

<!-- Enriquecimento -->
<add key="empresaId" value="1"/>
<add key="aplicacaoId" value="15"/>
<add key="chaveAcesso" value="922e9c67-ad12-4eb8-8f6d-66b1185b82bf"/>
<add key="urlBase" value="https://api-enriqdados-01"/>
```

### 7.3 Cache e Performance

```xml
<add key="tempoCacheDadosProvedores" value="5"/> <!-- 5 minutos -->
<add key="consulta_somente_repositorio_local" value="S"/> <!-- Modo offline -->
```

### 7.4 Contingência

```xml
<add key="contingencia_desconectada_ativa" value="N"/>
<add key="contingencia_desconectada_idwebservice" value="1"/>
<add key="contingencia_desconectada_endereco" value="www.bvsnet.com.br"/>
<add key="contingencia_desconectada_porta" value="3006"/>
```

## 8. FLUXOGRAMA COMPLETO

```
┌─────────────────────────────────────────────────────────────┐
│ 1. ASMX Web Service recebe: Consultar(tipo, doc, sistema)  │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│ 2. ServicoNegocio.Consultar():                             │
│    • Valida tipo ∈ {1,2}                                   │
│    • Valida documento não vazio                            │
│    • Valida sistema existe (SPBR_TBL_Sistema_ValidarSistema)│
│    • Limpa documento                                        │
│    • Carrega provedores (SPBR_TBL_WebService_ConsultarPorTipo)│
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│ 3. validarDocumento():                                      │
│    • CPF: 11 dígitos + mod-11                              │
│    • CNPJ: 14 dígitos + mod-11                             │
│    ✗ Inválido → RetornaStringInvalido()                    │
└────────────────────────┬────────────────────────────────────┘
                         │ ✓ Válido
                         ▼
┌─────────────────────────────────────────────────────────────┐
│ 4. DoConsultaRepositorio() - Cache Layer                    │
│    • SELECT SPBR_CONSULTAR_TBL_DOCUMENTO                   │
│    • Verifica DataConsultaValidade                         │
│    • Retorna:                                              │
│      ✓ XML completo (cache hit válido)                     │
│      ✗ "VENCIDO" (expirado)                                │
│      ✗ "NAO_ENCONTRADO" (não existe)                       │
│      ✗ "DESCONECTADO" (BD offline)                         │
│      ✗ "COM_RESTRICAO" (situação irregular)                │
└────────────────────────┬────────────────────────────────────┘
                         │ Cache Miss
                         ▼
┌─────────────────────────────────────────────────────────────┐
│ 5. ObtemConsulta() - Provider Orchestration                 │
│    LOOP pelos provedores (ordem de prioridade):             │
│    ┌──────────────────────────────────────────────┐        │
│    │ switch(provedor.ID):                          │        │
│    │   case 1: DoConsultaRFB() ────────┐          │        │
│    │   case 2: DoConsultaSerasa() ──┐  │          │        │
│    │   case 4: DoConsultaBoaVista() │  │          │        │
│    │   case 5: DoConsultaEnriquecimento()         │        │
│    └──────────────────────────────────────────────┘        │
│         │                              │  │                 │
│         │ Sucesso                      │  │ Erro/Exception  │
│         ▼                              ▼  ▼                 │
│    ┌─────────┐                   ┌──────────────┐          │
│    │ RETORNA │                   │ Próximo?     │          │
│    │  XML    │                   │ ✓→ Loop      │          │
│    └─────────┘                   │ ✗→ Fallback  │          │
│                                  └──────────────┘          │
│                                         │                   │
│                                         ▼                   │
│                              ┌───────────────────┐          │
│                              │ !SERPRO_executado?│          │
│                              │ ✓→ DoConsultaRFB()│          │
│                              │ ✗→ RetornaXmlErro()│         │
│                              └───────────────────┘          │
└─────────────────────────────────────────────────────────────┘
                         │ Sucesso em algum provedor
                         ▼
┌─────────────────────────────────────────────────────────────┐
│ 6. Persistência:                                            │
│    • INSERT/UPDATE SPBR_ATUALIZAR_TBL_DOCUMENTO            │
│    • INSERT TBL_LOG_ERRO (se erro)                         │
│    • INSERT LogAuditoria (auditoria completa)              │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│ 7. Retorno:                                                 │
│    • TiraAcento(xml) - Remove acentuação                   │
│    • return XML formatado ao Web Service                    │
└─────────────────────────────────────────────────────────────┘
```

## 9. PONTOS DE ATENÇÃO

### 9.1 Segurança
- ✓ TLS 1.2 obrigatório (linha 307)
- ✓ Credenciais criptografadas (ChaveCriptografia)
- ✓ Certificado digital SERPRO
- ✓ Senhas mascaradas em logs BoaVista (linha 250)

### 9.2 Performance
- ✓ Cache em memória de provedores (5 minutos default)
- ✓ Connection pooling SQL Server
- ✓ Timeout configurável por provedor
- ⚠ Socket síncrono BoaVista (blocking I/O)

### 9.3 Resiliência
- ✓ Fallback chain automático
- ✓ Modo contingência offline
- ✓ Retry com SERPRO como último recurso
- ✓ Logging completo de erros

### 9.4 Observabilidade
- ✓ log4net (file + console)
- ✓ Auditoria completa (LogAuditoria)
- ✓ Trace BoaVista configurável
- ✓ Tracking por sistema (rateio)

---

> **Nota:** Esta implementação é um sistema legado robusto com múltiplas camadas de fallback e cache otimizado para minimizar custos de consultas externas.