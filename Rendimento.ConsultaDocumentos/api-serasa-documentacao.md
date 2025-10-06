# Documentação da API Serasa (Experian Marketing)

## Visão Geral

A API Serasa é um Web Service SOAP fornecido pela Experian Marketing para consulta de dados cadastrais de pessoas físicas (CPF) e jurídicas (CNPJ) no Brasil.

**Provedor:** Experian Marketing
**Protocolo:** SOAP 1.1 / SOAP 1.2
**Formato:** XML
**Namespace:** `http://www.experianmarketing.com.br/`

---

## Endpoint Base

```
URL: https://www.experianmarketing.com.br/webservice/infobuscaws.asmx
WSDL: https://www.experianmarketing.com.br/webservice/infobuscaws.asmx?WSDL
```

---

## Operações Disponíveis

### 1. retornaDadosPF (Pessoa Física - CPF)

Consulta dados cadastrais de pessoa física por CPF.

**SOAPAction:** `http://www.experianmarketing.com.br/retornaDadosPF`

#### Parâmetros de Entrada

| Parâmetro      | Tipo   | Obrigatório | Descrição                                    |
|----------------|--------|-------------|----------------------------------------------|
| strLogin       | string | Sim         | Usuário de autenticação do serviço          |
| strSenha       | string | Sim         | Senha de autenticação do serviço            |
| strDominio     | string | Sim         | Domínio de autenticação                      |
| strDocumento   | string | Sim         | Número do CPF (apenas dígitos)              |

#### Estrutura da Requisição SOAP

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"
               xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
               xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <soap:Body>
    <retornaDadosPF xmlns="http://www.experianmarketing.com.br/">
      <strLogin>usuario_exemplo</strLogin>
      <strSenha>senha_exemplo</strSenha>
      <strDominio>dominio_exemplo</strDominio>
      <strDocumento>12345678901</strDocumento>
    </retornaDadosPF>
  </soap:Body>
</soap:Envelope>
```

#### Estrutura de Resposta - Sucesso

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <retornaDadosPFResponse xmlns="http://www.experianmarketing.com.br/">
      <retornaDadosPFResult>
        <CPF>12345678901</CPF>
        <Nome>NOME DO CONTRIBUINTE</Nome>
        <Situacao>REGULAR</Situacao>
        <Codigo_de_Controle>123456789</Codigo_de_Controle>
        <Hora>14:30:25</Hora>
        <Fonte_Pesquisada>RFB</Fonte_Pesquisada>
        <!-- Outros campos retornados -->
      </retornaDadosPFResult>
    </retornaDadosPFResponse>
  </soap:Body>
</soap:Envelope>
```

#### Estrutura de Resposta - Erro

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <retornaDadosPFResponse xmlns="http://www.experianmarketing.com.br/">
      <retornaDadosPFResult>
        <Erro>Descrição do erro ocorrido</Erro>
      </retornaDadosPFResult>
    </retornaDadosPFResponse>
  </soap:Body>
</soap:Envelope>
```

#### Campos de Resposta - Pessoa Física

| Campo              | Tipo   | Descrição                                      |
|--------------------|--------|------------------------------------------------|
| CPF                | string | Número do CPF consultado                       |
| Nome               | string | Nome completo do contribuinte                  |
| Situacao           | string | Situação cadastral (REGULAR, SUSPENSA, etc.)   |
| Codigo_de_Controle | string | Código de controle da consulta                 |
| Hora               | string | Hora da consulta (HH:mm:ss)                    |
| Fonte_Pesquisada   | string | Fonte dos dados (ex: RFB)                      |
| Erro               | string | Mensagem de erro (quando aplicável)            |

---

### 2. retornaDadosPJ (Pessoa Jurídica - CNPJ)

Consulta dados cadastrais de pessoa jurídica por CNPJ.

**SOAPAction:** `http://www.experianmarketing.com.br/retornaDadosPJ`

#### Parâmetros de Entrada

| Parâmetro      | Tipo   | Obrigatório | Descrição                                    |
|----------------|--------|-------------|----------------------------------------------|
| strLogin       | string | Sim         | Usuário de autenticação do serviço          |
| strSenha       | string | Sim         | Senha de autenticação do serviço            |
| strDominio     | string | Sim         | Domínio de autenticação                      |
| strDocumento   | string | Sim         | Número do CNPJ (apenas dígitos)             |

#### Estrutura da Requisição SOAP

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"
               xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
               xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <soap:Body>
    <retornaDadosPJ xmlns="http://www.experianmarketing.com.br/">
      <strLogin>usuario_exemplo</strLogin>
      <strSenha>senha_exemplo</strSenha>
      <strDominio>dominio_exemplo</strDominio>
      <strDocumento>12345678000195</strDocumento>
    </retornaDadosPJ>
  </soap:Body>
</soap:Envelope>
```

#### Estrutura de Resposta - Sucesso

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <retornaDadosPJResponse xmlns="http://www.experianmarketing.com.br/">
      <retornaDadosPJResult>
        <CNPJ>12345678000195</CNPJ>
        <Nome>RAZAO SOCIAL DA EMPRESA LTDA</Nome>
        <DataAbertura>2010-05-15</DataAbertura>
        <Situacao>ATIVA</Situacao>
        <DataSituacao>2010-05-15</DataSituacao>
        <Data>2025-10-05</Data>
        <Hora>14:30:25</Hora>
        <!-- Outros campos retornados -->
      </retornaDadosPJResult>
    </retornaDadosPJResponse>
  </soap:Body>
</soap:Envelope>
```

#### Estrutura de Resposta - Erro

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <retornaDadosPJResponse xmlns="http://www.experianmarketing.com.br/">
      <retornaDadosPJResult>
        <Erro>Descrição do erro ocorrido</Erro>
      </retornaDadosPJResult>
    </retornaDadosPJResponse>
  </soap:Body>
</soap:Envelope>
```

#### Campos de Resposta - Pessoa Jurídica

| Campo         | Tipo     | Descrição                                      |
|---------------|----------|------------------------------------------------|
| CNPJ          | string   | Número do CNPJ consultado                      |
| Nome          | string   | Razão social da empresa                        |
| DataAbertura  | date     | Data de abertura/fundação (yyyy-MM-dd)         |
| Situacao      | string   | Situação cadastral (ATIVA, BAIXADA, etc.)      |
| DataSituacao  | date     | Data da situação cadastral (yyyy-MM-dd)        |
| Data          | date     | Data da consulta (yyyy-MM-dd)                  |
| Hora          | string   | Hora da consulta (HH:mm:ss)                    |
| Erro          | string   | Mensagem de erro (quando aplicável)            |

---

### 3. retornaDadosTelefone

Consulta dados por número de telefone.

**SOAPAction:** `http://www.experianmarketing.com.br/retornaDadosTelefone`

#### Parâmetros de Entrada

| Parâmetro      | Tipo   | Obrigatório | Descrição                                    |
|----------------|--------|-------------|----------------------------------------------|
| strLogin       | string | Sim         | Usuário de autenticação do serviço          |
| strSenha       | string | Sim         | Senha de autenticação do serviço            |
| strDominio     | string | Sim         | Domínio de autenticação                      |
| strDDD         | string | Sim         | Código DDD (2 dígitos)                       |
| strTelefone    | string | Sim         | Número do telefone (sem DDD)                 |

---

### 4. retornaStatusProconTelefone

Consulta status PROCON de um número de telefone.

**SOAPAction:** `http://www.experianmarketing.com.br/retornaStatusProconTelefone`

#### Parâmetros de Entrada

| Parâmetro      | Tipo   | Obrigatório | Descrição                                    |
|----------------|--------|-------------|----------------------------------------------|
| strLogin       | string | Sim         | Usuário de autenticação do serviço          |
| strSenha       | string | Sim         | Senha de autenticação do serviço            |
| strDominio     | string | Sim         | Domínio de autenticação                      |
| strDDD         | string | Sim         | Código DDD (2 dígitos)                       |
| strTelefone    | string | Sim         | Número do telefone (sem DDD)                 |

---

## Autenticação

A autenticação é realizada através de credenciais enviadas em cada requisição:

1. **strLogin**: Usuário fornecido pela Experian
2. **strSenha**: Senha fornecida pela Experian
3. **strDominio**: Domínio de autenticação fornecido pela Experian

**Importante:** As credenciais são obtidas da configuração do provedor armazenada no banco de dados através da tabela `TBL_WEBSERVICE`. No código, são recuperadas via:
- `mObjDadosProvedor[mIntIndiceProvedorAtual].Usuario`
- `mObjDadosProvedor[mIntIndiceProvedorAtual].Senha`
- `mObjDadosProvedor[mIntIndiceProvedorAtual].Dominio`

---

## Configuração de Timeout

O timeout da requisição é configurável através da chave `timeout_serasa` no arquivo `Web.config`:

```xml
<appSettings>
  <add key="timeout_serasa" value="10000" />
</appSettings>
```

**Valor padrão:** 10000 ms (10 segundos)

**Implementação:**
```csharp
lObjInfoBuscaWS.Timeout = RetornaTimeoutSerasa(); // Valor em milissegundos
```

---

## Headers HTTP

### Request Headers

```
POST /webservice/infobuscaws.asmx HTTP/1.1
Host: www.experianmarketing.com.br
Content-Type: text/xml; charset=utf-8
SOAPAction: "http://www.experianmarketing.com.br/[NomeOperacao]"
Content-Length: [tamanho]
```

### Response Headers

```
HTTP/1.1 200 OK
Content-Type: text/xml; charset=utf-8
Content-Length: [tamanho]
```

---

## Tratamento de Erros

### Cenários de Erro

1. **Erro de Autenticação**: Credenciais inválidas
2. **Documento não encontrado**: CPF/CNPJ inexistente na base
3. **Nome em branco**: Quando o retorno não contém nome (tratado como erro crítico)
4. **Timeout**: Requisição excede tempo limite configurado
5. **Erro de comunicação**: Falha na conexão com o serviço

### Tratamento no Código

```csharp
// Verificação de erro no XML de resposta
lObjXmlNodeList = lObjXmlDocument.GetElementsByTagName("Erro");
if (lObjXmlNodeList.Count > 0)
{
    lStrErro = lObjXmlNodeList[0].ChildNodes.Item(0).InnerText.Trim();
}

if (lStrErro.Trim() != string.Empty)
{
    throw new System.Exception("WebService: SERASA - Sistema: " + pIntIdSistema.ToString() +
                               " - Documento: " + pStrDocumento.Trim() + " - " + lStrErro.Trim());
}

// Validação de campos obrigatórios
if (lStrNome.Trim() == String.Empty)
{
    throw new System.Exception("WebService: SERASA - Sistema: " + pIntIdSistema.ToString() +
                               " - Documento: " + pStrDocumento.Trim() + " - Nome em branco");
}
```

### Estrutura de Exceções

Quando ocorre um erro, a exceção lançada segue o padrão:

```
WebService: SERASA - Sistema: [ID_SISTEMA] - Documento: [CPF/CNPJ] - [MENSAGEM_ERRO]
```

---

## Fluxo de Processamento

### Consulta CPF (retornaDadosPF)

```
1. Obter credenciais do provedor (Usuario, Senha, Dominio)
2. Criar instância do InfoBuscaWS
3. Configurar timeout
4. Executar retornaDadosPF(strLogin, strSenha, strDominio, strDocumento)
5. Carregar resposta XML
6. Verificar tag <Erro>
   - Se erro existe → lançar exceção
   - Se não:
     7. Extrair CPF, Nome, Situacao, Codigo_de_Controle, Hora, Fonte_Pesquisada
     8. Validar Nome não vazio
     9. Processar situação cadastral
     10. Armazenar no repositório/cache
     11. Registrar utilização (Sistema + WebService)
```

### Consulta CNPJ (retornaDadosPJ)

```
1. Obter credenciais do provedor (Usuario, Senha, Dominio)
2. Criar instância do InfoBuscaWS
3. Configurar timeout
4. Executar retornaDadosPJ(strLogin, strSenha, strDominio, strDocumento)
5. Carregar resposta XML
6. Verificar tag <Erro>
   - Se erro existe → lançar exceção
   - Se não:
     7. Extrair CNPJ, Nome, DataAbertura, Situacao, DataSituacao, Data, Hora
     8. Validar Nome não vazio
     9. Processar situação cadastral
     10. Armazenar no repositório/cache
     11. Registrar utilização (Sistema + WebService)
```

---

## Mapeamento de Situações Cadastrais

### Pessoa Física (CPF)

O sistema mapeia as situações retornadas pelo Serasa para códigos internos através do método `RetornaSituacaoSerasaPF()`.

### Pessoa Jurídica (CNPJ)

O sistema mapeia as situações retornadas pelo Serasa para códigos internos através do método `RetornaSituacaoSerasaPJ()`.

**Situações comuns:**
- REGULAR / ATIVA → Cadastro ativo
- SUSPENSA / BAIXADA → Cadastro inativo
- CANCELADA / NULA → Cadastro cancelado

---

## Integração no Sistema

### Localização do Código

**Arquivo:** `Source/Components/entNegocio/_projeto/Servico/ServicoNegocio.cs`

**Métodos principais:**
- `DoConsultaSerasaPF()` - Linha 1190 (Consulta Pessoa Física)
- `DoConsultaSerasaPJ()` - Linha 1347 (Consulta Pessoa Jurídica)

**Web Service Proxy:**
- `Source/Components/entNegocio/_projeto/Web References/br.com.experianmarketing.www/Reference.cs`
- Classe: `InfoBuscaWS` (herda de `SoapHttpClientProtocol`)

### Prioridade no Sistema

O Serasa é identificado pelo código `2` no sistema de provedores e pode ser configurado com diferentes prioridades na cadeia de fallback de consultas.

```csharp
private const int mCstSERASA = 2;
private const string mCstNOME_SERASA = "SERASA";
```

---

## Exemplos de Uso

### Exemplo 1: Consulta CPF

```csharp
InfoBuscaWS lObjInfoBuscaWS = new InfoBuscaWS();
lObjInfoBuscaWS.Timeout = 10000; // 10 segundos

XmlNode lObjXmlNode = lObjInfoBuscaWS.retornaDadosPF(
    "usuario_sistema",
    "senha_sistema",
    "dominio_sistema",
    "12345678901"
);

XmlDocument lObjXmlDocument = new XmlDocument();
lObjXmlDocument.LoadXml(lObjXmlNode.OuterXml);
```

### Exemplo 2: Consulta CNPJ

```csharp
InfoBuscaWS lObjInfoBuscaWS = new InfoBuscaWS();
lObjInfoBuscaWS.Timeout = 10000; // 10 segundos

XmlNode lObjXmlNode = lObjInfoBuscaWS.retornaDadosPJ(
    "usuario_sistema",
    "senha_sistema",
    "dominio_sistema",
    "12345678000195"
);

XmlDocument lObjXmlDocument = new XmlDocument();
lObjXmlDocument.LoadXml(lObjXmlNode.OuterXml);
```

---

## Observações Importantes

1. **Protocolo SOAP:** O serviço utiliza SOAP 1.1 e SOAP 1.2
2. **Encoding:** UTF-8 em todas as requisições e respostas
3. **HTTPS:** Todas as comunicações são via HTTPS
4. **Credenciais:** Armazenadas de forma criptografada no banco de dados
5. **Cache:** Respostas são armazenadas no repositório local para reduzir chamadas
6. **Fallback:** Em caso de falha, o sistema pode tentar outros provedores configurados
7. **Auditoria:** Todas as consultas são registradas na tabela `LogAuditoria`
8. **Custo:** Sistema rastreia consumo por `pIntSistema` para billing/reporting

---

## Referências

- **WSDL:** `Source/Components/entNegocio/_projeto/Web References/br.com.experianmarketing.www/infobuscaws.wsdl`
- **Proxy Class:** `Source/Components/entNegocio/_projeto/Web References/br.com.experianmarketing.www/Reference.cs`
- **Implementação:** `Source/Components/entNegocio/_projeto/Servico/ServicoNegocio.cs`
- **Configuração:** `Source/Site/ASPNetApp/_projeto/Web.Config`
