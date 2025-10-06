# Documentação da API SERPRO (Receita Federal do Brasil)

## Visão Geral

A API SERPRO é fornecida pela Receita Federal do Brasil através do sistema InfoConv (Convênios da Receita Federal) para consulta oficial de dados cadastrais de CPF e CNPJ. O sistema utiliza **protocolos diferentes para cada tipo de documento**:

- **CPF**: REST API com JSON
- **CNPJ**: SOAP Web Service

**Provedor:** SERPRO / Receita Federal do Brasil
**Autenticação:** Certificado Digital (X509)

---

## 1. Consulta CPF (Pessoa Física)

### Tecnologia

**Protocolo:** REST API
**Formato:** JSON
**Método HTTP:** POST
**Content-Type:** application/json
**Autenticação:** Certificado Digital X.509

### Endpoint

```
URL: [Configurado em Web.config - chave "AcessoSerproConsultarCPFD3"]
Método: POST
```

**Configuração no Web.config:**
```xml
<appSettings>
  <add key="AcessoSerproConsultarCPFD3" value="[URL_API_SERPRO_CPF]" />
</appSettings>
```

---

### Estrutura da Requisição

#### Headers HTTP

```
POST [URL_API_SERPRO_CPF] HTTP/1.1
Content-Type: application/json
Content-Length: [tamanho]
[Certificado Digital anexado]
```

#### Corpo da Requisição (JSON)

```json
{
  "listadecpf": "12345678901",
  "cpfUsuario": "25008464825"
}
```

**Parâmetros:**

| Campo       | Tipo   | Obrigatório | Descrição                                        |
|-------------|--------|-------------|--------------------------------------------------|
| listadecpf  | string | Sim         | CPF a ser consultado (apenas dígitos)           |
| cpfUsuario  | string | Sim         | CPF do operador/sistema (constante: 25008464825)|

**Observação:** O campo `cpfUsuario` é uma constante definida no sistema com o valor `"25008464825"`.

---

### Estrutura da Resposta - Sucesso

```json
{
  "codRetorno": "0",
  "msgRetorno": "Consulta realizada com sucesso",
  "qtdeRegistrosRetornados": "1",
  "retornoConsultada": [
    {
      "codRetorno": "0000",
      "msgRetorno": "CONSTA",
      "msgInformativa": "",
      "cpfContribuinte": "12345678901",
      "codSitCad": "0",
      "nomeContribuinte": "NOME DO CONTRIBUINTE",
      "nomeSocial": "",
      "nomeMae": "NOME DA MAE DO CONTRIBUINTE",
      "codSexo": "1",
      "dataNascimento": "01011980",
      "dataInscricao": "01012000",
      "anoObito": "",
      "codMunicNaturIBGE": "3550308",
      "indResidExt": "N",
      "codPaisResidExt": "",
      "nomePaisResidExt": "",
      "codOcupacaoPrinc": "7210",
      "codNaturezaOcup": "10",
      "anoExercicioOcup": "2020",
      "dataUltimaAtual": "20250101",
      "listaMotivoAltSitCad": [
        {
          "codMotivoAltSitCad": "00",
          "descMotivoAltSitCad": "REGULAR"
        }
      ]
    }
  ]
}
```

---

### Campos de Resposta - CPF

#### Nível Principal

| Campo                   | Tipo   | Descrição                                          |
|-------------------------|--------|----------------------------------------------------|
| codRetorno              | string | Código de retorno da operação (0 = sucesso)       |
| msgRetorno              | string | Mensagem de retorno                                |
| qtdeRegistrosRetornados | string | Quantidade de registros retornados                 |
| retornoConsultada       | array  | Lista de dados consultados                         |

#### Nível retornoConsultada (cada item)

| Campo               | Tipo   | Descrição                                                 |
|---------------------|--------|-----------------------------------------------------------|
| codRetorno          | string | Código de retorno do registro (0000 = sucesso)           |
| msgRetorno          | string | Mensagem (CONSTA, NÃO CONSTA, etc.)                      |
| msgInformativa      | string | Mensagem informativa adicional                            |
| cpfContribuinte     | string | CPF consultado                                            |
| codSitCad           | string | Código da situação cadastral (0=Regular, 2=Suspensa, etc.)|
| nomeContribuinte    | string | Nome completo do contribuinte                             |
| nomeSocial          | string | Nome social (se houver)                                   |
| nomeMae             | string | Nome da mãe                                               |
| codSexo             | string | Código do sexo (1=Masculino, 2=Feminino)                 |
| dataNascimento      | string | Data de nascimento (formato: ddMMyyyy)                    |
| dataInscricao       | string | Data de inscrição no CPF (formato: ddMMyyyy)             |
| anoObito            | string | Ano de óbito (se aplicável)                               |
| codMunicNaturIBGE   | string | Código IBGE do município de naturalidade                  |
| indResidExt         | string | Indicador de residente no exterior (S/N)                 |
| codPaisResidExt     | string | Código do país de residência (se exterior)               |
| nomePaisResidExt    | string | Nome do país de residência (se exterior)                 |
| codOcupacaoPrinc    | string | Código da ocupação principal                              |
| codNaturezaOcup     | string | Código da natureza da ocupação                            |
| anoExercicioOcup    | string | Ano de exercício da ocupação                              |
| dataUltimaAtual     | string | Data da última atualização (formato: yyyyMMdd)           |
| listaMotivoAltSitCad| array  | Lista de motivos de alteração de situação cadastral      |

#### Nível listaMotivoAltSitCad (cada item)

| Campo                | Tipo   | Descrição                                |
|----------------------|--------|------------------------------------------|
| codMotivoAltSitCad   | string | Código do motivo de alteração           |
| descMotivoAltSitCad  | string | Descrição do motivo de alteração        |

---

### Códigos de Situação Cadastral (CPF)

| Código | Descrição          |
|--------|--------------------|
| 0      | REGULAR            |
| 2      | SUSPENSA           |
| 3      | TITULAR FALECIDO   |
| 4      | PENDENTE REGULARIZ.|
| 5      | CANCELADA          |
| 8      | NULA               |
| 9      | CANCELADA POR MULT.|

---

### Estrutura de Resposta - Erro

Quando `codRetorno != "0"` ou `retornoConsultada[].codRetorno != "0000"`:

```json
{
  "codRetorno": "999",
  "msgRetorno": "Erro ao processar consulta",
  "qtdeRegistrosRetornados": "0",
  "retornoConsultada": []
}
```

---

## 2. Consulta CNPJ (Pessoa Jurídica)

### Tecnologia

**Protocolo:** SOAP 1.1 / SOAP 1.2
**Formato:** XML
**Namespace:** `https://acesso.infoconv.receita.fazenda.gov.br/ws/cnpj/`
**Autenticação:** Certificado Digital X.509

### Endpoint

```
URL: https://acesso.infoconv.receita.fazenda.gov.br/ws/cnpj/ConsultarCNPJ.asmx
WSDL: https://acesso.infoconv.receita.fazenda.gov.br/ws/cnpj/ConsultarCNPJ.asmx?WSDL
```

---

### Operação Utilizada: ConsultarCNPJP7_SC

**Descrição:** Consultar CNPJ - Perfil 7 com Sistema Convenente

**SOAPAction:** `https://acesso.infoconv.receita.fazenda.gov.br/ws/cnpj/ConsultarCNPJP7_SC`

#### Parâmetros de Entrada

| Parâmetro         | Tipo   | Obrigatório | Descrição                                      |
|-------------------|--------|-------------|------------------------------------------------|
| CNPJ              | string | Sim         | Número do CNPJ (apenas dígitos)               |
| CPFUsuario        | string | Sim         | CPF do operador (constante: 25008464825)      |
| SistemaConvenente | string | Sim         | Identificação do sistema convenente/solicitante|

---

### Estrutura da Requisição SOAP

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"
               xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
               xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <soap:Body>
    <ConsultarCNPJP7_SC xmlns="https://acesso.infoconv.receita.fazenda.gov.br/ws/cnpj/">
      <CNPJ>12345678000195</CNPJ>
      <CPFUsuario>25008464825</CPFUsuario>
      <SistemaConvenente>SISTEMA_ID</SistemaConvenente>
    </ConsultarCNPJP7_SC>
  </soap:Body>
</soap:Envelope>
```

---

### Estrutura de Resposta SOAP - Sucesso

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <ConsultarCNPJP7_SCResponse xmlns="https://acesso.infoconv.receita.fazenda.gov.br/ws/cnpj/">
      <ConsultarCNPJP7_SCResult>
        <CNPJPerfil7>
          <CNPJ>12345678000195</CNPJ>
          <Estabelecimento>0001</Estabelecimento>
          <NomeEmpresarial>EMPRESA EXEMPLO LTDA</NomeEmpresarial>
          <NomeFantasia>EXEMPLO COMERCIO</NomeFantasia>
          <SituacaoCadastral>ATIVA</SituacaoCadastral>
          <MotivoSituacao></MotivoSituacao>
          <DataSituacaoCadastral>01/01/2020</DataSituacaoCadastral>
          <CidadeExterior></CidadeExterior>
          <CodigoPais></CodigoPais>
          <NomePais></NomePais>
          <NaturezaJuridica>206-2 - SOCIEDADE EMPRESARIA LIMITADA</NaturezaJuridica>
          <DataAbertura>01/01/2015</DataAbertura>
          <CNAEPrincipal>4712-1/00 - COMERCIO VAREJISTA</CNAEPrincipal>
          <CNAESecundario>
            <string>4713-0/02 - COMERCIO VAREJISTA DE MERCADORIAS</string>
          </CNAESecundario>
          <TipoLogradouro>RUA</TipoLogradouro>
          <Logradouro>EXEMPLO</Logradouro>
          <NumeroLogradouro>123</NumeroLogradouro>
          <Complemento>SALA 01</Complemento>
          <Bairro>CENTRO</Bairro>
          <CEP>01000000</CEP>
          <UF>SP</UF>
          <CodigoMunicipio>7107</CodigoMunicipio>
          <NomeMunicipio>SAO PAULO</NomeMunicipio>
          <Referencia></Referencia>
          <DDD1>11</DDD1>
          <Telefone1>12345678</Telefone1>
          <DDD2></DDD2>
          <Telefone2></Telefone2>
          <Email>contato@exemplo.com.br</Email>
          <CPFResponsavel>12345678901</CPFResponsavel>
          <NomeResponsavel>NOME DO RESPONSAVEL</NomeResponsavel>
          <CapitalSocial>100000.00</CapitalSocial>
          <Sociedade>
            <SocioPerfil7>
              <CPFCNPJSocio>98765432100</CPFCNPJSocio>
              <NomeSocio>SOCIO EXEMPLO</NomeSocio>
              <QualificacaoSocio>49 - SOCIO-ADMINISTRADOR</QualificacaoSocio>
              <DataEntrada>01/01/2015</DataEntrada>
              <CPFRepresentante></CPFRepresentante>
              <NomeRepresentante></NomeRepresentante>
              <QualificacaoRepresentante></QualificacaoRepresentante>
            </SocioPerfil7>
          </Sociedade>
          <Porte>01 - MICRO EMPRESA</Porte>
          <OpcaoSimples>SIM</OpcaoSimples>
          <OpcaoSIMEI>NAO</OpcaoSIMEI>
          <SituacaoEspecial></SituacaoEspecial>
          <DataSituacaoEspecial></DataSituacaoEspecial>
          <Erro></Erro>
        </CNPJPerfil7>
      </ConsultarCNPJP7_SCResult>
    </ConsultarCNPJP7_SCResponse>
  </soap:Body>
</soap:Envelope>
```

---

### Campos de Resposta - CNPJ (CNPJPerfil7)

#### Dados Principais

| Campo                  | Tipo   | Descrição                                           |
|------------------------|--------|-----------------------------------------------------|
| CNPJ                   | string | Número do CNPJ (com raiz + filial)                 |
| Estabelecimento        | string | Número do estabelecimento/filial                    |
| NomeEmpresarial        | string | Razão social / Nome empresarial                     |
| NomeFantasia           | string | Nome fantasia                                       |
| SituacaoCadastral      | string | Situação cadastral (ATIVA, BAIXADA, SUSPENSA, etc.) |
| MotivoSituacao         | string | Motivo da situação cadastral                        |
| DataSituacaoCadastral  | string | Data da situação cadastral (dd/MM/yyyy)            |
| DataAbertura           | string | Data de abertura/constituição (dd/MM/yyyy)         |

#### Dados de Localização

| Campo                  | Tipo   | Descrição                                           |
|------------------------|--------|-----------------------------------------------------|
| CidadeExterior         | string | Cidade no exterior (se aplicável)                   |
| CodigoPais             | string | Código do país (se exterior)                        |
| NomePais               | string | Nome do país (se exterior)                          |

#### Natureza Jurídica e Atividades

| Campo                  | Tipo   | Descrição                                           |
|------------------------|--------|-----------------------------------------------------|
| NaturezaJuridica       | string | Código e descrição da natureza jurídica            |
| CNAEPrincipal          | string | CNAE principal (código + descrição)                |
| CNAESecundario         | array  | Lista de CNAEs secundários                          |

#### Endereço

| Campo                  | Tipo   | Descrição                                           |
|------------------------|--------|-----------------------------------------------------|
| TipoLogradouro         | string | Tipo de logradouro (RUA, AVENIDA, etc.)            |
| Logradouro             | string | Nome do logradouro                                  |
| NumeroLogradouro       | string | Número do endereço                                  |
| Complemento            | string | Complemento do endereço                             |
| Bairro                 | string | Bairro                                              |
| CEP                    | string | CEP (8 dígitos)                                     |
| UF                     | string | Sigla da UF                                         |
| CodigoMunicipio        | string | Código do município                                 |
| NomeMunicipio          | string | Nome do município                                   |
| Referencia             | string | Referência do endereço                              |

#### Contato

| Campo                  | Tipo   | Descrição                                           |
|------------------------|--------|-----------------------------------------------------|
| DDD1                   | string | DDD do telefone 1                                   |
| Telefone1              | string | Número do telefone 1                                |
| DDD2                   | string | DDD do telefone 2                                   |
| Telefone2              | string | Número do telefone 2                                |
| Email                  | string | E-mail de contato                                   |

#### Responsável Legal

| Campo                  | Tipo   | Descrição                                           |
|------------------------|--------|-----------------------------------------------------|
| CPFResponsavel         | string | CPF do responsável legal                            |
| NomeResponsavel        | string | Nome do responsável legal                           |

#### Capital e Sociedade

| Campo                  | Tipo   | Descrição                                           |
|------------------------|--------|-----------------------------------------------------|
| CapitalSocial          | string | Capital social (formato: 999999.99)                |
| Sociedade              | array  | Quadro societário (SocioPerfil7[])                 |

#### Regime Tributário

| Campo                  | Tipo   | Descrição                                           |
|------------------------|--------|-----------------------------------------------------|
| Porte                  | string | Porte da empresa (01=ME, 03=EPP, 05=Demais)        |
| OpcaoSimples           | string | Optante pelo Simples Nacional (SIM/NAO)            |
| OpcaoSIMEI             | string | Optante pelo SIMEI (SIM/NAO)                       |

#### Situação Especial

| Campo                  | Tipo   | Descrição                                           |
|------------------------|--------|-----------------------------------------------------|
| SituacaoEspecial       | string | Situação especial (se houver)                       |
| DataSituacaoEspecial   | string | Data da situação especial                           |

#### Erro

| Campo                  | Tipo   | Descrição                                           |
|------------------------|--------|-----------------------------------------------------|
| Erro                   | string | Mensagem de erro (vazio quando sucesso)            |

---

### Estrutura de Sócio (SocioPerfil7)

| Campo                     | Tipo   | Descrição                                      |
|---------------------------|--------|------------------------------------------------|
| CPFCNPJSocio              | string | CPF ou CNPJ do sócio                          |
| NomeSocio                 | string | Nome do sócio                                  |
| QualificacaoSocio         | string | Código e descrição da qualificação            |
| DataEntrada               | string | Data de entrada na sociedade (dd/MM/yyyy)     |
| CPFRepresentante          | string | CPF do representante legal (se aplicável)     |
| NomeRepresentante         | string | Nome do representante legal                    |
| QualificacaoRepresentante | string | Qualificação do representante                  |

---

### Estrutura de Resposta SOAP - Erro

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <ConsultarCNPJP7_SCResponse xmlns="https://acesso.infoconv.receita.fazenda.gov.br/ws/cnpj/">
      <ConsultarCNPJP7_SCResult>
        <CNPJPerfil7>
          <Erro>CNPJ INEXISTENTE NA BASE DE DADOS</Erro>
        </CNPJPerfil7>
      </ConsultarCNPJP7_SCResult>
    </ConsultarCNPJP7_SCResponse>
  </soap:Body>
</soap:Envelope>
```

---

## Autenticação via Certificado Digital

Ambas as APIs (CPF e CNPJ) utilizam **Certificado Digital X.509** para autenticação.

### Configuração do Certificado

**Web.config:**
```xml
<appSettings>
  <add key="certificado" value="C:\caminho\certificado.pfx" />
  <add key="senha_certificado" value="[senha_criptografada]" />
  <add key="chaveCriptografia" value="[chave_para_descriptografar]" />
</appSettings>
```

### Carregamento no Código

```csharp
// O certificado é carregado e armazenado em cache
X509Certificate2 certificado = new X509Certificate2(
    ConfigurationManager.AppSettings["certificado"],
    SenhaCertificadoDescriptografada,
    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet
);

// Anexado às requisições
objRequisicao.ClientCertificates.Add(certificado); // Para REST (CPF)
objConsultarCNPJ.ClientCredentials.ClientCertificate.Certificate = certificado; // Para SOAP (CNPJ)
```

**Importante:**
- A senha do certificado é armazenada criptografada no Web.config
- O certificado é carregado em cache (MemoryCache) para otimização
- Deve ser um certificado válido emitido pela ICP-Brasil

---

## Fluxo de Processamento

### Consulta CPF (REST)

```
1. Criar objeto de requisição (PessoaFisica)
2. Serializar para JSON com camelCase
3. Configurar HttpWebRequest:
   - Content-Type: application/json
   - Method: POST
   - Adicionar certificado digital
4. Enviar requisição
5. Ler resposta JSON
6. Deserializar para PessoaFisicaD3
7. Validar codRetorno:
   - Se "0" → processar retornoConsultada
   - Verificar retornoConsultada[0].codRetorno
     - Se "0000" → sucesso
     - Caso contrário → erro
8. Extrair dados e processar
```

### Consulta CNPJ (SOAP)

```
1. Criar instância do ConsultarCNPJSoapClient
2. Anexar certificado digital às credenciais
3. Executar ConsultarCNPJP7_SC(CNPJ, CPFUsuario, SistemaConvenente)
4. Receber ArrayOfCNPJPerfil7
5. Obter primeiro elemento (GetValue(0))
6. Validar campo Erro:
   - Se vazio → sucesso
   - Caso contrário → processar erro
7. Extrair dados do CNPJPerfil7
```

---

## Headers HTTP

### REST API (CPF)

**Request:**
```
POST [URL] HTTP/1.1
Content-Type: application/json
Content-Length: [tamanho]
[Certificado Digital X.509]
```

**Response:**
```
HTTP/1.1 200 OK
Content-Type: application/json
Content-Length: [tamanho]
```

### SOAP API (CNPJ)

**Request:**
```
POST /ws/cnpj/ConsultarCNPJ.asmx HTTP/1.1
Host: acesso.infoconv.receita.fazenda.gov.br
Content-Type: text/xml; charset=utf-8
SOAPAction: "https://acesso.infoconv.receita.fazenda.gov.br/ws/cnpj/ConsultarCNPJP7_SC"
Content-Length: [tamanho]
[Certificado Digital X.509]
```

**Response:**
```
HTTP/1.1 200 OK
Content-Type: text/xml; charset=utf-8
Content-Length: [tamanho]
```

---

## Tratamento de Erros

### Cenários de Erro Comuns

1. **Certificado Inválido/Expirado**
   - Erro de autenticação TLS
   - Exceção ao adicionar certificado

2. **CPF/CNPJ Inválido**
   - REST (CPF): `codRetorno != "0"` ou `retornoConsultada[].codRetorno != "0000"`
   - SOAP (CNPJ): Campo `Erro` preenchido

3. **Documento Não Encontrado**
   - REST (CPF): `msgRetorno = "NÃO CONSTA"`
   - SOAP (CNPJ): `Erro = "CNPJ INEXISTENTE NA BASE DE DADOS"`

4. **Timeout de Comunicação**
   - Exceção de rede (WebException)

5. **Serviço Indisponível**
   - HTTP 500/503 ou exceção SOAP

### Implementação no Sistema

```csharp
try
{
    // CPF
    var resultado = ConsultarPessoaFisicaRest(cpf);
    if (resultado.codRetorno != "0")
        throw new Exception($"Erro SERPRO CPF: {resultado.msgRetorno}");

    var dados = resultado.retornoConsultada.FirstOrDefault();
    if (dados.codRetorno != "0000")
        throw new Exception($"CPF não encontrado: {dados.msgRetorno}");

    // CNPJ
    var cnpjResultado = ConsultarPessoaJuridica(cnpj, sistema);
    if (!string.IsNullOrEmpty(cnpjResultado.Erro))
        throw new Exception($"Erro SERPRO CNPJ: {cnpjResultado.Erro}");
}
catch (Exception ex)
{
    // Log de erro e tratamento
    LogarErro($"WebService: SERPRO - Documento: {doc} - {ex.Message}");
    throw;
}
```

---

## Outras Operações CNPJ Disponíveis

Além do `ConsultarCNPJP7_SC`, o Web Service oferece outros perfis:

| Operação              | Descrição                                |
|-----------------------|------------------------------------------|
| ConsultarCNPJP1       | Perfil 1 - Dados básicos                |
| ConsultarCNPJP1_SC    | Perfil 1 com Sistema Convenente         |
| ConsultarCNPJP2       | Perfil 2 - Dados + Situação cadastral   |
| ConsultarCNPJP2_SC    | Perfil 2 com Sistema Convenente         |
| ConsultarCNPJP3       | Perfil 3 - Dados + Natureza jurídica    |
| ConsultarCNPJP3_SC    | Perfil 3 com Sistema Convenente         |
| ConsultarCNPJP7_SC    | **Perfil 7 - Completo (USADO)**         |
| ConsultarCNPJP7T      | Perfil 7 - Teste                        |
| ConsultarCNPJPDEC8789 | Perfil DEC 8789                         |

**Observação:** O sistema utiliza `ConsultarCNPJP7_SC` por ser o perfil mais completo, incluindo quadro societário.

---

## Integração no Sistema

### Localização do Código

**Implementação:**
- `Source/Components/entNegocio/_projeto/Servico/ServicoProvedorSerpro.cs`
- `Source/Components/entNegocio/_projeto/Servico/ServicoNegocio.cs` (método `DoConsultaRFB`)

**Modelos de Dados:**
- `Source/Components/entNegocio/_projeto/Model/PessoaFisica.cs` (Request CPF)
- `Source/Components/entNegocio/_projeto/Model/PessoaFisicaD3.cs` (Response CPF)

**SOAP Client:**
- `Source/Components/entNegocio/_projeto/Service References/infoconvConsultarCNPJ/Reference.cs`

**Configuração:**
- `Source/Site/ASPNetApp/_projeto/Web.Config`

### Prioridade no Sistema

O SERPRO é identificado pelo código `1` no sistema de provedores:

```csharp
private const int mCstSERPRO = 1;
private const string mCstNOME_SERPRO = "SERPRO";
```

**Comportamento especial:**
- É sempre tentado como **última alternativa** (fallback final) se todos os outros provedores falharem
- Considerado a fonte de dados mais confiável (Receita Federal oficial)

---

## Requisitos Técnicos

### Certificado Digital

- **Tipo:** X.509 (.pfx / .p12)
- **Emissor:** ICP-Brasil (Autoridade Certificadora válida)
- **Formato:** PKCS#12 com chave privada
- **Armazenamento:** File System com senha criptografada

### Protocolos de Segurança

- **TLS 1.2** ou superior
- **HTTPS** obrigatório
- **Certificado de Cliente** (mTLS - Mutual TLS)

### Dependências .NET

- `System.Net` (HttpWebRequest para REST)
- `System.Net.Http` (HttpClient)
- `System.Security.Cryptography.X509Certificates` (Certificados)
- `System.ServiceModel` (SOAP Client)
- `Newtonsoft.Json` (JSON Serialization)

---

## Exemplos de Código

### Exemplo 1: Consulta CPF (REST)

```csharp
var servicoSerpro = new ServicoProvedorSerpro();

// Request
var cpf = "12345678901";
PessoaFisicaD3 resultado = servicoSerpro.ConsultarPessoaFisicaRest(cpf);

// Validar resposta
if (resultado.codRetorno == "0")
{
    var dados = resultado.retornoConsultada.FirstOrDefault();

    if (dados.codRetorno == "0000")
    {
        string nome = dados.nomeContribuinte;
        string situacao = dados.codSitCad;
        string dataNasc = dados.dataNascimento;

        Console.WriteLine($"CPF: {cpf}");
        Console.WriteLine($"Nome: {nome}");
        Console.WriteLine($"Situação: {situacao}");
    }
}
```

### Exemplo 2: Consulta CNPJ (SOAP)

```csharp
var servicoSerpro = new ServicoProvedorSerpro();

// Request
var cnpj = "12345678000195";
var cpfUsuario = "25008464825";
var sistema = "SISTEMA_ID";

infoconvConsultarCNPJ.CNPJPerfil7 resultado =
    servicoSerpro.ConsultarPessoaJuridica(cnpj, sistema);

// Validar resposta
if (string.IsNullOrEmpty(resultado.Erro))
{
    string razaoSocial = resultado.NomeEmpresarial;
    string situacao = resultado.SituacaoCadastral;
    string dataAbertura = resultado.DataAbertura;

    Console.WriteLine($"CNPJ: {cnpj}");
    Console.WriteLine($"Razão Social: {razaoSocial}");
    Console.WriteLine($"Situação: {situacao}");
    Console.WriteLine($"Data Abertura: {dataAbertura}");

    // Quadro Societário
    if (resultado.Sociedade != null)
    {
        foreach (var socio in resultado.Sociedade)
        {
            Console.WriteLine($"Sócio: {socio.NomeSocio} - {socio.QualificacaoSocio}");
        }
    }
}
else
{
    Console.WriteLine($"Erro: {resultado.Erro}");
}
```

### Exemplo 3: Configuração de Requisição HTTP com Certificado

```csharp
public static String RequestApi(String uri, String method, Byte[] jsonDataBytes)
{
    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

    request.ContentType = "application/json";
    request.Method = method;
    request.ContentLength = jsonDataBytes.Length;

    // Adicionar certificado digital
    request.ClientCertificates.Add(Setup.Certificado);

    // Enviar request
    using (Stream stream = request.GetRequestStream())
    {
        stream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
    }

    // Ler response
    using (Stream response = request.GetResponse().GetResponseStream())
    using (StreamReader reader = new StreamReader(response))
    {
        return reader.ReadToEnd();
    }
}
```

---

## Observações Importantes

1. **Certificado Digital Obrigatório:** Todas as consultas exigem certificado digital válido
2. **Constante CPF Operador:** O valor `"25008464825"` é fixo no sistema
3. **Protocolos Distintos:** CPF usa REST/JSON, CNPJ usa SOAP/XML
4. **Cache de Certificado:** O certificado é carregado uma vez e mantido em memória
5. **Fallback Final:** SERPRO é sempre tentado por último na cadeia de provedores
6. **Fonte Oficial:** Dados provenientes diretamente da Receita Federal do Brasil
7. **Auditoria:** Todas as consultas são registradas na tabela `LogAuditoria`
8. **Custo:** Sistema rastreia consumo por sistema solicitante (`SistemaConvenente`)

---

## Referências

### CPF (REST)
- **Implementação:** `Source/Components/entNegocio/_projeto/Servico/ServicoProvedorSerpro.cs:25`
- **Request Model:** `Source/Components/entNegocio/_projeto/Model/PessoaFisica.cs`
- **Response Model:** `Source/Components/entNegocio/_projeto/Model/PessoaFisicaD3.cs`

### CNPJ (SOAP)
- **WSDL:** `Source/Components/entNegocio/_projeto/Service References/infoconvConsultarCNPJ/ConsultarCNPJ_producao.wsdl`
- **Proxy Class:** `Source/Components/entNegocio/_projeto/Service References/infoconvConsultarCNPJ/Reference.cs`
- **Implementação:** `Source/Components/entNegocio/_projeto/Servico/ServicoProvedorSerpro.cs:58`

### Comum
- **Orquestração:** `Source/Components/entNegocio/_projeto/Servico/ServicoNegocio.cs:768` (método `DoConsultaRFB`)
- **Setup/Config:** `Source/Components/entNegocio/_projeto/Servico/Setup.cs`
- **Configuração:** `Source/Site/ASPNetApp/_projeto/Web.Config`
