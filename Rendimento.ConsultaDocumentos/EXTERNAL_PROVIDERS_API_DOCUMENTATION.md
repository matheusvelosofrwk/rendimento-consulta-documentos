# Documentação de Integração - APIs Externas SERPRO e SERASA

## Índice

1. [Visão Geral](#visão-geral)
2. [API SERPRO](#api-serpro)
   - [Consulta CPF](#serpro-consulta-cpf)
   - [Consulta CNPJ Perfil 1](#serpro-consulta-cnpj-perfil-1)
   - [Consulta CNPJ Perfil 2](#serpro-consulta-cnpj-perfil-2)
   - [Consulta CNPJ Perfil 3](#serpro-consulta-cnpj-perfil-3)
   - [Health Check](#serpro-health-check)
3. [API SERASA](#api-serasa)
   - [Consulta CPF](#serasa-consulta-cpf)
   - [Consulta CNPJ](#serasa-consulta-cnpj)
   - [Consulta Score](#serasa-consulta-score)
   - [Health Check](#serasa-health-check)
4. [Detalhes Técnicos](#detalhes-técnicos)
5. [Mapeamento de Dados](#mapeamento-de-dados)
6. [Códigos de Situação Cadastral](#códigos-de-situação-cadastral)
7. [Tratamento de Erros](#tratamento-de-erros)
8. [Exemplos de Implementação](#exemplos-de-implementação)

---

## Visão Geral

Este documento descreve a integração com as APIs externas de consulta de documentos brasileiros (CPF/CNPJ) fornecidas pela **SERPRO** e **SERASA**.

### Arquitetura de Comunicação

```
┌─────────────────┐
│ Aplicação       │
│ Cliente         │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ SerproService   │◄────┐
│ SerasaService   │     │ HttpClient
└────────┬────────┘     │ (Timeout configurável)
         │              │
         ▼              │
┌─────────────────────────┐
│ APIs Externas           │
│ ├─ SERPRO (Governo)     │
│ └─ SERASA (Privado)     │
└─────────────────────────┘
```

### Provedores Disponíveis

| Provedor | Tipo         | Documentos Suportados | Dados Adicionais          |
|----------|--------------|----------------------|---------------------------|
| SERPRO   | Governamental| CPF, CNPJ            | Oficial, Quadro Societário|
| SERASA   | Privado      | CPF, CNPJ            | Score de Crédito, Análise de Risco|

---

## API SERPRO

Base URL: `http://localhost:8000/ws`

### Autenticação

A API SERPRO utiliza CPF do operador como identificação nas requisições.

**Credenciais:**
- CPF Operador: `25008464825`
- Chave API: Configurável
- Token: Configurável
- Certificado: Opcional

---

### SERPRO: Consulta CPF

Consulta dados cadastrais de Pessoa Física (CPF) na base da Receita Federal.

#### Endpoint

```
POST /cpf/ConsultarCPFP1
```

#### Headers

```http
Content-Type: application/json
Accept: application/json
User-Agent: ConsultaDocumentos/1.0
```

#### Request Body

```json
{
  "listadecpf": "12345678909",
  "cpfUsuario": "25008464825"
}
```

**Campos:**
- `listadecpf` (string, obrigatório): CPF a ser consultado (11 dígitos, sem formatação)
- `cpfUsuario` (string, obrigatório): CPF do operador que realiza a consulta

#### Response - Sucesso (200 OK)

```json
{
  "nome": "JOÃO DA SILVA",
  "dataNascimento": "1985-03-15T00:00:00",
  "nomeMae": "MARIA DA SILVA",
  "sexo": "M",
  "tituloEleitor": "123456789012",
  "residenteExterior": "N",
  "anoObito": null,
  "situacaoCadastral": "ATIVA",
  "codigoControle": "ABC123456",
  "erro": null
}
```

**Campos da Resposta:**
- `nome` (string): Nome completo da pessoa física
- `dataNascimento` (datetime): Data de nascimento no formato ISO 8601
- `nomeMae` (string): Nome completo da mãe
- `sexo` (string): Sexo ("M" ou "F")
- `tituloEleitor` (string): Número do título de eleitor
- `residenteExterior` (string): Indica se é residente no exterior ("S" ou "N")
- `anoObito` (string, nullable): Ano de óbito, se aplicável
- `situacaoCadastral` (string): Status do CPF (ATIVA, SUSPENSA, INAPTA, BAIXADA, NULA, CANCELADA)
- `codigoControle` (string): Código de controle da consulta
- `erro` (string, nullable): Mensagem de erro, se houver

#### Response - Erro (200 OK com erro)

```json
{
  "nome": null,
  "dataNascimento": null,
  "nomeMae": null,
  "sexo": null,
  "tituloEleitor": null,
  "residenteExterior": null,
  "anoObito": null,
  "situacaoCadastral": null,
  "codigoControle": null,
  "erro": "CPF não encontrado na base de dados"
}
```

#### Timeout

**25 segundos**

#### Códigos de Status HTTP

- `200 OK`: Requisição processada (verificar campo `erro` para falhas de negócio)
- `400 Bad Request`: Parâmetros inválidos
- `401 Unauthorized`: CPF operador não autorizado
- `500 Internal Server Error`: Erro no servidor SERPRO
- `503 Service Unavailable`: Serviço temporariamente indisponível

---

### SERPRO: Consulta CNPJ Perfil 1

Consulta dados **básicos** de Pessoa Jurídica (CNPJ).

#### Endpoint

```
POST /cnpj/ConsultarCNPJP1
```

#### Headers

```http
Content-Type: application/json
Accept: application/json
User-Agent: ConsultaDocumentos/1.0
```

#### Request Body

```json
{
  "listadecnpj": "12345678000195",
  "cpfUsuario": "25008464825"
}
```

**Campos:**
- `listadecnpj` (string, obrigatório): CNPJ a ser consultado (14 dígitos, sem formatação)
- `cpfUsuario` (string, obrigatório): CPF do operador que realiza a consulta

#### Response - Sucesso (200 OK)

```json
{
  "cnpj": "12345678000195",
  "nomeEmpresarial": "EMPRESA EXEMPLO LTDA",
  "nomeFantasia": "EXEMPLO",
  "situacaoCadastral": "ATIVA",
  "estabelecimento": "MATRIZ",
  "codigoPais": "105",
  "nomePais": "BRASIL",
  "cidadeExterior": null,
  "erro": null
}
```

**Campos da Resposta:**
- `cnpj` (string): CNPJ consultado
- `nomeEmpresarial` (string): Razão social da empresa
- `nomeFantasia` (string): Nome fantasia
- `situacaoCadastral` (string): Status do CNPJ (ATIVA, SUSPENSA, INAPTA, BAIXADA, NULA, CANCELADA)
- `estabelecimento` (string): Tipo de estabelecimento (MATRIZ, FILIAL)
- `codigoPais` (string): Código do país (105 = Brasil)
- `nomePais` (string): Nome do país
- `cidadeExterior` (string, nullable): Cidade no exterior, se aplicável
- `erro` (string, nullable): Mensagem de erro, se houver

#### Timeout

**30 segundos**

---

### SERPRO: Consulta CNPJ Perfil 2

Consulta dados **completos** de Pessoa Jurídica (CNPJ), incluindo endereço, telefones e atividades.

#### Endpoint

```
POST /cnpj/ConsultarCNPJP2
```

#### Headers

```http
Content-Type: application/json
Accept: application/json
User-Agent: ConsultaDocumentos/1.0
```

#### Request Body

```json
{
  "listadecnpj": "12345678000195",
  "cpfUsuario": "25008464825"
}
```

#### Response - Sucesso (200 OK)

```json
{
  "cnpj": "12345678000195",
  "nomeEmpresarial": "EMPRESA EXEMPLO LTDA",
  "nomeFantasia": "EXEMPLO",
  "situacaoCadastral": "ATIVA",
  "dataAbertura": "15/03/2010",
  "dataSituacaoCadastral": "15/03/2010",
  "naturezaJuridica": "206-2",
  "cnAEPrincipal": "6201-5/00",
  "cnAESecundario": [
    "6202-3/00",
    "6203-1/00"
  ],
  "tipoLogradouro": "RUA",
  "logradouro": "DAS FLORES",
  "numeroLogradouro": "123",
  "complemento": "SALA 456",
  "bairro": "CENTRO",
  "cep": "01234567",
  "uf": "SP",
  "codigoMunicipio": "7107",
  "nomeMunicipio": "SAO PAULO",
  "ddd1": "11",
  "telefone1": "33334444",
  "ddd2": "11",
  "telefone2": "55556666",
  "email": "contato@exemplo.com.br",
  "codigoPais": "105",
  "nomePais": "BRASIL",
  "cidadeExterior": null,
  "estabelecimento": "MATRIZ",
  "erro": null
}
```

**Campos Adicionais (em relação ao Perfil 1):**
- `dataAbertura` (string): Data de abertura da empresa (formato: dd/MM/yyyy)
- `dataSituacaoCadastral` (string): Data da situação cadastral (formato: dd/MM/yyyy)
- `naturezaJuridica` (string): Código da natureza jurídica
- `cnAEPrincipal` (string): CNAE principal
- `cnAESecundario` (array): Lista de CNAEs secundários
- `tipoLogradouro` (string): Tipo do logradouro (RUA, AVENIDA, TRAVESSA, etc.)
- `logradouro` (string): Nome do logradouro
- `numeroLogradouro` (string): Número do endereço
- `complemento` (string): Complemento do endereço
- `bairro` (string): Bairro
- `cep` (string): CEP (8 dígitos, sem formatação)
- `uf` (string): Unidade Federativa (2 letras)
- `codigoMunicipio` (string): Código IBGE do município
- `nomeMunicipio` (string): Nome do município
- `ddd1` (string): DDD do telefone 1
- `telefone1` (string): Número do telefone 1
- `ddd2` (string): DDD do telefone 2
- `telefone2` (string): Número do telefone 2
- `email` (string): E-mail de contato

#### Timeout

**30 segundos**

---

### SERPRO: Consulta CNPJ Perfil 3

Consulta dados **completos com quadro societário** de Pessoa Jurídica (CNPJ).

#### Endpoint

```
POST /cnpj/ConsultarCNPJP3
```

#### Headers

```http
Content-Type: application/json
Accept: application/json
User-Agent: ConsultaDocumentos/1.0
```

#### Request Body

```json
{
  "listadecnpj": "12345678000195",
  "cpfUsuario": "25008464825"
}
```

#### Response - Sucesso (200 OK)

```json
{
  "cnpj": "12345678000195",
  "nomeEmpresarial": "EMPRESA EXEMPLO LTDA",
  "nomeFantasia": "EXEMPLO",
  "situacaoCadastral": "ATIVA",
  "dataAbertura": "15/03/2010",
  "dataSituacaoCadastral": "15/03/2010",
  "naturezaJuridica": "206-2",
  "cnAEPrincipal": "6201-5/00",
  "cnAESecundario": [
    "6202-3/00",
    "6203-1/00"
  ],
  "tipoLogradouro": "RUA",
  "logradouro": "DAS FLORES",
  "numeroLogradouro": "123",
  "complemento": "SALA 456",
  "bairro": "CENTRO",
  "cep": "01234567",
  "uf": "SP",
  "codigoMunicipio": "7107",
  "nomeMunicipio": "SAO PAULO",
  "ddd1": "11",
  "telefone1": "33334444",
  "ddd2": "11",
  "telefone2": "55556666",
  "email": "contato@exemplo.com.br",
  "codigoPais": "105",
  "nomePais": "BRASIL",
  "cidadeExterior": null,
  "estabelecimento": "MATRIZ",
  "socios": [
    {
      "cpfCnpj": "98765432100",
      "nome": "JOSE PROPRIETARIO",
      "qualificacao": "49",
      "dataEntrada": "15/03/2010",
      "percentualCapital": "50,00",
      "cpfRepresentanteLegal": null,
      "nomeRepresentanteLegal": null,
      "qualificacaoRepresentanteLegal": null
    },
    {
      "cpfCnpj": "12345678900",
      "nome": "MARIA SÓCIA",
      "qualificacao": "49",
      "dataEntrada": "15/03/2010",
      "percentualCapital": "50,00",
      "cpfRepresentanteLegal": null,
      "nomeRepresentanteLegal": null,
      "qualificacaoRepresentanteLegal": null
    }
  ],
  "erro": null
}
```

**Campo Adicional (em relação ao Perfil 2):**

**`socios` (array):** Lista de sócios da empresa

Cada objeto sócio contém:
- `cpfCnpj` (string): CPF ou CNPJ do sócio
- `nome` (string): Nome completo ou razão social do sócio
- `qualificacao` (string): Código de qualificação do sócio (49 = Sócio-Administrador)
- `dataEntrada` (string): Data de entrada na sociedade (formato: dd/MM/yyyy)
- `percentualCapital` (string): Percentual de participação no capital social
- `cpfRepresentanteLegal` (string, nullable): CPF do representante legal (se sócio for PJ)
- `nomeRepresentanteLegal` (string, nullable): Nome do representante legal
- `qualificacaoRepresentanteLegal` (string, nullable): Qualificação do representante legal

#### Timeout

**30 segundos**

---

### SERPRO: Health Check

Endpoint para verificação de disponibilidade do serviço SERPRO.

#### Endpoint

```
GET /health
```

#### Headers

```http
User-Agent: ConsultaDocumentos/1.0
```

#### Response - Sucesso (200 OK)

```json
{
  "status": "healthy",
  "timestamp": "2025-10-05T14:30:00Z"
}
```

#### Timeout

**5 segundos**

---

## API SERASA

Base URL: `http://localhost:8000/serasa`

### Autenticação

A API SERASA utiliza autenticação por usuário e senha em cada requisição.

**Credenciais:**
- Usuário: Configurável
- Senha: Configurável

---

### SERASA: Consulta CPF

Consulta dados cadastrais completos de Pessoa Física (CPF) com informações de crédito.

#### Endpoint

```
POST /cpf/consultar
```

#### Headers

```http
Content-Type: application/json
Accept: application/json
User-Agent: ConsultaDocumentos/1.0
```

#### Request Body

```json
{
  "cpf": "12345678909",
  "tipoConsulta": "COMPLETA",
  "usuario": "usuario_serasa",
  "senha": "senha_serasa"
}
```

**Campos:**
- `cpf` (string, obrigatório): CPF a ser consultado (11 dígitos, sem formatação)
- `tipoConsulta` (string, obrigatório): Tipo de consulta ("BASICA" ou "COMPLETA")
- `usuario` (string, obrigatório): Usuário de autenticação SERASA
- `senha` (string, obrigatório): Senha de autenticação SERASA

#### Response - Sucesso (200 OK)

```json
{
  "status": "SUCESSO",
  "codigo": "0000",
  "mensagem": "Consulta realizada com sucesso",
  "dataConsulta": "2025-10-05T14:30:00",
  "cpf": "12345678909",
  "dados": {
    "identificacao": {
      "nome": "JOÃO DA SILVA",
      "dataNascimento": "15/03/1985",
      "nomeMae": "MARIA DA SILVA",
      "sexo": "M",
      "situacaoCadastral": "REGULAR",
      "codigoSituacao": "0"
    },
    "documentos": {
      "cpf": "12345678909",
      "rg": "123456789",
      "orgaoExpedidor": "SSP",
      "ufRg": "SP",
      "dataExpedicaoRg": "01/01/2000",
      "tituloEleitor": "123456789012"
    },
    "endereco": {
      "logradouro": "RUA DAS FLORES",
      "numero": "123",
      "complemento": "APTO 456",
      "bairro": "CENTRO",
      "cep": "01234567",
      "cidade": "SAO PAULO",
      "uf": "SP"
    },
    "contato": {
      "ddd": "11",
      "telefone": "987654321",
      "email": "joao.silva@email.com"
    },
    "dadosComplementares": {
      "profissao": "ANALISTA DE SISTEMAS",
      "rendaMensal": 5000.00,
      "escolaridade": "SUPERIOR COMPLETO",
      "estadoCivil": "CASADO"
    },
    "analiseRisco": {
      "score": 750,
      "classificacao": "BAIXO RISCO",
      "dataUltimaAtualizacao": "2025-10-01T00:00:00"
    }
  }
}
```

**Estrutura da Resposta:**

**Nível raiz:**
- `status` (string): Status da consulta (SUCESSO, ERRO)
- `codigo` (string): Código de retorno
- `mensagem` (string): Mensagem descritiva
- `dataConsulta` (string): Data/hora da consulta
- `cpf` (string): CPF consultado

**`dados.identificacao`:**
- `nome` (string): Nome completo
- `dataNascimento` (string): Data de nascimento (dd/MM/yyyy)
- `nomeMae` (string): Nome da mãe
- `sexo` (string): Sexo (M/F)
- `situacaoCadastral` (string): Situação cadastral (REGULAR, SUSPENSO, CANCELADO, PENDENTE DE REGULARIZACAO, NULO)
- `codigoSituacao` (string): Código numérico da situação

**`dados.documentos`:**
- `cpf` (string): CPF
- `rg` (string): RG
- `orgaoExpedidor` (string): Órgão expedidor do RG
- `ufRg` (string): UF de expedição do RG
- `dataExpedicaoRg` (string): Data de expedição do RG
- `tituloEleitor` (string): Número do título de eleitor

**`dados.endereco`:**
- `logradouro` (string): Nome do logradouro
- `numero` (string): Número
- `complemento` (string): Complemento
- `bairro` (string): Bairro
- `cep` (string): CEP
- `cidade` (string): Nome da cidade
- `uf` (string): UF

**`dados.contato`:**
- `ddd` (string): DDD
- `telefone` (string): Número do telefone
- `email` (string): E-mail

**`dados.dadosComplementares`:**
- `profissao` (string): Profissão
- `rendaMensal` (decimal): Renda mensal estimada
- `escolaridade` (string): Nível de escolaridade
- `estadoCivil` (string): Estado civil

**`dados.analiseRisco`:**
- `score` (int): Score de crédito (0-1000)
- `classificacao` (string): Classificação de risco
- `dataUltimaAtualizacao` (string): Data da última atualização do score

#### Response - Erro (200 OK)

```json
{
  "status": "ERRO",
  "codigo": "E001",
  "mensagem": "CPF não encontrado",
  "dataConsulta": "2025-10-05T14:30:00",
  "cpf": "12345678909",
  "dados": null
}
```

#### Timeout

**30 segundos**

---

### SERASA: Consulta CNPJ

Consulta dados cadastrais completos de Pessoa Jurídica (CNPJ) com informações empresariais e quadro societário.

#### Endpoint

```
POST /cnpj/consultar
```

#### Headers

```http
Content-Type: application/json
Accept: application/json
User-Agent: ConsultaDocumentos/1.0
```

#### Request Body

```json
{
  "cnpj": "12345678000195",
  "tipoConsulta": "COMPLETA",
  "usuario": "usuario_serasa",
  "senha": "senha_serasa"
}
```

**Campos:**
- `cnpj` (string, obrigatório): CNPJ a ser consultado (14 dígitos, sem formatação)
- `tipoConsulta` (string, obrigatório): Tipo de consulta ("BASICA" ou "COMPLETA")
- `usuario` (string, obrigatório): Usuário de autenticação SERASA
- `senha` (string, obrigatório): Senha de autenticação SERASA

#### Response - Sucesso (200 OK)

```json
{
  "status": "SUCESSO",
  "codigo": "0000",
  "mensagem": "Consulta realizada com sucesso",
  "dataConsulta": "2025-10-05T14:30:00",
  "cnpj": "12345678000195",
  "dados": {
    "identificacao": {
      "razaoSocial": "EMPRESA EXEMPLO LTDA",
      "nomeFantasia": "EXEMPLO",
      "situacaoCadastral": "ATIVA",
      "codigoSituacao": "02",
      "dataAbertura": "15/03/2010",
      "dataSituacaoCadastral": "15/03/2010"
    },
    "atividade": {
      "naturezaJuridica": "206-2",
      "naturezaJuridicaDescricao": "SOCIEDADE EMPRESARIA LIMITADA",
      "cnAEPrincipal": "6201-5/00",
      "cnAEPrincipalDescricao": "DESENVOLVIMENTO DE PROGRAMAS DE COMPUTADOR SOB ENCOMENDA"
    },
    "endereco": {
      "logradouro": "RUA DAS FLORES",
      "numero": "123",
      "complemento": "SALA 456",
      "bairro": "CENTRO",
      "cep": "01234567",
      "cidade": "SAO PAULO",
      "uf": "SP"
    },
    "contato": {
      "ddd": "11",
      "telefone": "33334444",
      "email": "contato@exemplo.com.br"
    },
    "dadosComplementares": {
      "capitalSocial": 100000.00,
      "porte": "EMPRESA DE PEQUENO PORTE",
      "dataUltimaAtualizacao": "2025-09-15T00:00:00"
    },
    "quadroSocietario": [
      {
        "cpfCnpj": "98765432100",
        "nome": "JOSE PROPRIETARIO",
        "qualificacao": "49",
        "qualificacaoDescricao": "SOCIO-ADMINISTRADOR",
        "dataEntrada": "15/03/2010",
        "percentualCapital": "50.00"
      },
      {
        "cpfCnpj": "12345678900",
        "nome": "MARIA SÓCIA",
        "qualificacao": "49",
        "qualificacaoDescricao": "SOCIO-ADMINISTRADOR",
        "dataEntrada": "15/03/2010",
        "percentualCapital": "50.00"
      }
    ],
    "analiseRisco": {
      "score": 650,
      "classificacao": "RISCO MODERADO",
      "dataUltimaAtualizacao": "2025-10-01T00:00:00",
      "indicadores": {
        "protestos": 0,
        "acoesCiveis": 1,
        "falencias": 0,
        "concordatas": 0
      }
    }
  }
}
```

**Estrutura da Resposta:**

**Nível raiz:**
- `status` (string): Status da consulta
- `codigo` (string): Código de retorno
- `mensagem` (string): Mensagem descritiva
- `dataConsulta` (string): Data/hora da consulta
- `cnpj` (string): CNPJ consultado

**`dados.identificacao`:**
- `razaoSocial` (string): Razão social
- `nomeFantasia` (string): Nome fantasia
- `situacaoCadastral` (string): Situação cadastral (ATIVA, SUSPENSA, INAPTA, BAIXADA, NULA, CANCELADA)
- `codigoSituacao` (string): Código da situação
- `dataAbertura` (string): Data de abertura (dd/MM/yyyy)
- `dataSituacaoCadastral` (string): Data da situação cadastral

**`dados.atividade`:**
- `naturezaJuridica` (string): Código da natureza jurídica
- `naturezaJuridicaDescricao` (string): Descrição da natureza jurídica
- `cnAEPrincipal` (string): CNAE principal
- `cnAEPrincipalDescricao` (string): Descrição do CNAE principal

**`dados.endereco`:**
- `logradouro` (string): Logradouro
- `numero` (string): Número
- `complemento` (string): Complemento
- `bairro` (string): Bairro
- `cep` (string): CEP
- `cidade` (string): Cidade
- `uf` (string): UF

**`dados.contato`:**
- `ddd` (string): DDD
- `telefone` (string): Telefone
- `email` (string): E-mail

**`dados.dadosComplementares`:**
- `capitalSocial` (decimal): Capital social
- `porte` (string): Porte da empresa
- `dataUltimaAtualizacao` (string): Data da última atualização

**`dados.quadroSocietario` (array):**
- `cpfCnpj` (string): CPF/CNPJ do sócio
- `nome` (string): Nome do sócio
- `qualificacao` (string): Código de qualificação
- `qualificacaoDescricao` (string): Descrição da qualificação
- `dataEntrada` (string): Data de entrada na sociedade
- `percentualCapital` (string): Percentual de participação

**`dados.analiseRisco`:**
- `score` (int): Score empresarial
- `classificacao` (string): Classificação de risco
- `dataUltimaAtualizacao` (string): Data de atualização
- **`indicadores`:**
  - `protestos` (int): Quantidade de protestos
  - `acoesCiveis` (int): Quantidade de ações cíveis
  - `falencias` (int): Quantidade de falências
  - `concordatas` (int): Quantidade de concordatas

#### Timeout

**30 segundos**

---

### SERASA: Consulta Score

Consulta exclusiva para obtenção do score de crédito.

#### Endpoint

```
POST /score/consultar
```

#### Headers

```http
Content-Type: application/json
Accept: application/json
User-Agent: ConsultaDocumentos/1.0
```

#### Request Body

```json
{
  "documento": "12345678909",
  "tipoDocumento": "CPF",
  "usuario": "usuario_serasa",
  "senha": "senha_serasa"
}
```

**Campos:**
- `documento` (string, obrigatório): CPF ou CNPJ (sem formatação)
- `tipoDocumento` (string, obrigatório): "CPF" ou "CNPJ"
- `usuario` (string, obrigatório): Usuário de autenticação SERASA
- `senha` (string, obrigatório): Senha de autenticação SERASA

#### Response - Sucesso (200 OK)

```json
{
  "status": "SUCESSO",
  "codigo": "0000",
  "mensagem": "Score calculado com sucesso",
  "dataConsulta": "2025-10-05T14:30:00",
  "documento": "12345678909",
  "tipoDocumento": "CPF",
  "score": {
    "valor": 750,
    "classificacao": "BAIXO RISCO",
    "faixa": "700-800",
    "dataCalculo": "2025-10-05T14:30:00",
    "fatoresPositivos": [
      "Histórico de pagamentos em dia",
      "Baixa quantidade de consultas recentes",
      "Diversificação de crédito"
    ],
    "fatoresNegativos": [
      "Utilização elevada do limite de crédito"
    ]
  }
}
```

**Estrutura da Resposta:**

**Nível raiz:**
- `status` (string): Status da consulta
- `codigo` (string): Código de retorno
- `mensagem` (string): Mensagem descritiva
- `dataConsulta` (string): Data/hora da consulta
- `documento` (string): Documento consultado
- `tipoDocumento` (string): Tipo do documento

**`score`:**
- `valor` (int): Score numérico (0-1000)
- `classificacao` (string): Classificação textual (ALTO RISCO, RISCO MODERADO, BAIXO RISCO, MUITO BAIXO RISCO)
- `faixa` (string): Faixa de score
- `dataCalculo` (string): Data/hora do cálculo
- `fatoresPositivos` (array): Lista de fatores que contribuem positivamente para o score
- `fatoresNegativos` (array): Lista de fatores que contribuem negativamente para o score

#### Timeout

**25 segundos**

---

### SERASA: Health Check

Endpoint para verificação de disponibilidade do serviço SERASA.

#### Endpoint

```
GET /health
```

#### Headers

```http
User-Agent: ConsultaDocumentos/1.0
```

#### Response - Sucesso (200 OK)

```json
{
  "status": "healthy",
  "timestamp": "2025-10-05T14:30:00Z",
  "services": {
    "cpf": "available",
    "cnpj": "available",
    "score": "available"
  }
}
```

#### Timeout

**5 segundos**

---

## Detalhes Técnicos

### Configuração de Timeouts

| Provedor | Operação      | Timeout  |
|----------|--------------|----------|
| SERPRO   | Consulta CPF | 25s      |
| SERPRO   | Consulta CNPJ| 30s      |
| SERPRO   | Health Check | 5s       |
| SERASA   | Consulta CPF | 30s      |
| SERASA   | Consulta CNPJ| 30s      |
| SERASA   | Consulta Score| 25s     |
| SERASA   | Health Check | 5s       |

### Headers HTTP Padrão

Todos os endpoints devem incluir os seguintes headers:

```http
Content-Type: application/json
Accept: application/json
User-Agent: ConsultaDocumentos/1.0
```

### Códigos HTTP de Status

| Código | Significado                        | Ação Recomendada                      |
|--------|-----------------------------------|---------------------------------------|
| 200    | Sucesso (verificar campo `erro`)  | Processar resposta                    |
| 400    | Requisição inválida               | Validar parâmetros de entrada         |
| 401    | Não autorizado                    | Verificar credenciais                 |
| 408    | Timeout                           | Tentar novamente com timeout maior    |
| 429    | Muitas requisições                | Implementar rate limiting/retry       |
| 500    | Erro interno do servidor          | Tentar provedor alternativo           |
| 502    | Bad Gateway                       | Aguardar e tentar novamente           |
| 503    | Serviço indisponível              | Tentar provedor alternativo           |
| 504    | Gateway Timeout                   | Aumentar timeout ou usar alternativo  |

---

## Mapeamento de Dados

### Conversão de Datas

#### SERPRO
**Formato:** `dd/MM/yyyy` ou `ddMMyyyy`

**Exemplos:**
- `15/03/2010`
- `15032010`

**Conversão para DateTime:**
```csharp
public static DateTime? ParseDate(string? dateString)
{
    if (string.IsNullOrEmpty(dateString))
        return null;

    if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", null,
        DateTimeStyles.None, out var date1))
        return date1;

    if (DateTime.TryParseExact(dateString, "ddMMyyyy", null,
        DateTimeStyles.None, out var date2))
        return date2;

    return null;
}
```

#### SERASA
**Formatos:** `dd/MM/yyyy`, `yyyy-MM-dd`, `dd-MM-yyyy`

**Conversão para DateTime:**
```csharp
public static DateTime? ParseDate(string? dateStr)
{
    if (string.IsNullOrEmpty(dateStr))
        return null;

    var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "dd-MM-yyyy" };

    foreach (var format in formats)
    {
        if (DateTime.TryParseExact(dateStr, format, null,
            DateTimeStyles.None, out var result))
            return result;
    }

    return null;
}
```

### Conversão de Valores Booleanos

**SERPRO:**
- `"true"`, `"1"`, `"sim"`, `"s"`, `"yes"`, `"y"` → `true`
- `"false"`, `"0"`, `"não"`, `"nao"`, `"n"`, `"no"` → `false`

**SERASA:**
- `"S"`, `"SIM"`, `"TRUE"`, `"1"` → `true`
- Outros valores → `false`

### Conversão de Percentuais

**Formato:** String com vírgula ou ponto como separador decimal

**Exemplos:** `"50,00"`, `"50.00"`, `"33,33"`

**Conversão para Decimal:**
```csharp
public static decimal? ConvertToDecimal(string? value)
{
    if (string.IsNullOrEmpty(value))
        return null;

    // Remove caracteres não numéricos exceto vírgula e ponto
    var cleanStr = Regex.Replace(value, @"[^\d,.]", "");

    if (decimal.TryParse(cleanStr.Replace(',', '.'),
        NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        return result;

    return null;
}
```

---

## Códigos de Situação Cadastral

### Pessoa Física (CPF)

#### SERPRO
| Código | Descrição              | ID Interno |
|--------|------------------------|------------|
| ATIVA  | CPF Regular            | 1          |
| SUSPENSA| CPF Suspenso          | 2          |
| INAPTA | CPF Inapto             | 3          |
| BAIXADA| CPF Cancelado por Óbito| 4          |
| NULA   | CPF Nulo               | 5          |
| CANCELADA| CPF Cancelado        | 6          |

#### SERASA
| Código                       | Descrição              | ID Interno |
|------------------------------|------------------------|------------|
| REGULAR                      | CPF Regular            | 1          |
| SUSPENSO                     | CPF Suspenso           | 2          |
| CANCELADO                    | CPF Cancelado          | 3          |
| PENDENTE DE REGULARIZACAO    | Pendente Regularização | 4          |
| NULO                         | CPF Nulo               | 5          |

### Pessoa Jurídica (CNPJ)

#### SERPRO / SERASA
| Código    | Descrição              | ID Interno |
|-----------|------------------------|------------|
| ATIVA     | CNPJ Ativo             | 1          |
| SUSPENSA  | CNPJ Suspenso          | 2          |
| INAPTA    | CNPJ Inapto            | 3          |
| BAIXADA   | CNPJ Baixado           | 4          |
| NULA      | CNPJ Nulo              | 5          |
| CANCELADA | CNPJ Cancelado         | 6          |

---

## Tratamento de Erros

### Estratégia de Fallback

Quando uma API falha, o sistema deve tentar os provedores na seguinte ordem:

```
1. SERPRO (Fonte oficial)
   └─ Falha → Tentar SERASA
      └─ Falha → Retornar erro ao cliente
```

### Códigos de Erro Comuns

#### SERPRO

| Código | Descrição                          | Solução                              |
|--------|------------------------------------|--------------------------------------|
| E001   | CPF/CNPJ não encontrado            | Verificar número do documento        |
| E002   | CPF operador inválido              | Verificar credenciais                |
| E003   | Parâmetros inválidos               | Validar formato dos parâmetros       |
| E004   | Serviço temporariamente indisponível| Aguardar e tentar novamente         |
| E005   | Timeout na consulta                | Aumentar timeout ou usar alternativo |

#### SERASA

| Código | Descrição                          | Solução                              |
|--------|------------------------------------|--------------------------------------|
| E001   | Documento não encontrado           | Verificar número do documento        |
| E002   | Credenciais inválidas              | Verificar usuário/senha              |
| E003   | Tipo de consulta inválido          | Usar "BASICA" ou "COMPLETA"          |
| E004   | Limite de consultas excedido       | Aguardar reset do limite             |
| E005   | Serviço indisponível               | Tentar provedor alternativo          |

### Exemplo de Tratamento de Erro

```csharp
public async Task<DocumentoEntity?> ConsultarDocumentoAsync(
    string numeroDocumento,
    TipoDocumento tipoDocumento)
{
    // 1. Tentar SERPRO primeiro
    try
    {
        var serproService = new SerproService(_httpClient, _configuration, _logger);
        var documento = await serproService.ConsultarDocumentoAsync(
            numeroDocumento, tipoDocumento, cancellationToken);

        if (documento != null)
            return documento;
    }
    catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.ServiceUnavailable)
    {
        _logger.LogWarning("SERPRO indisponível, tentando SERASA...");
    }
    catch (TimeoutException)
    {
        _logger.LogWarning("Timeout no SERPRO, tentando SERASA...");
    }

    // 2. Fallback para SERASA
    try
    {
        var serasaService = new SerasaService(_httpClient, _configuration, _logger);
        var documento = await serasaService.ConsultarDocumentoAsync(
            numeroDocumento, tipoDocumento, cancellationToken);

        if (documento != null)
            return documento;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Erro ao consultar SERASA");
    }

    // 3. Todos os provedores falharam
    throw new ExternalProviderException(
        "Documento não encontrado em nenhum provedor disponível");
}
```

---

## Exemplos de Implementação

### Exemplo 1: Consulta CPF no SERPRO

```csharp
using System.Net.Http;
using System.Text;
using System.Text.Json;

public async Task<SerprocCPFResponse?> ConsultarCPFSerproAsync(string cpf)
{
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("http://localhost:8000/ws");
    httpClient.Timeout = TimeSpan.FromSeconds(25);
    httpClient.DefaultRequestHeaders.Add("User-Agent", "ConsultaDocumentos/1.0");

    var request = new SerprocCPFRequest
    {
        ListaDeCpf = cpf,
        CpfUsuario = "25008464825"
    };

    var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

    var response = await httpClient.PostAsync("/cpf/ConsultarCPFP1", content);

    if (!response.IsSuccessStatusCode)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Erro na consulta SERPRO: {response.StatusCode} - {errorContent}");
    }

    var responseContent = await response.Content.ReadAsStringAsync();
    var cpfData = JsonSerializer.Deserialize<SerprocCPFResponse>(responseContent,
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    return cpfData;
}
```

### Exemplo 2: Consulta CNPJ com Sócios no SERPRO

```csharp
public async Task<SerprocCNPJPerfil3Response?> ConsultarCNPJComSociosAsync(string cnpj)
{
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("http://localhost:8000/ws");
    httpClient.Timeout = TimeSpan.FromSeconds(30);
    httpClient.DefaultRequestHeaders.Add("User-Agent", "ConsultaDocumentos/1.0");

    var request = new SerprocCNPJRequest
    {
        ListaDeCnpj = cnpj,
        CpfUsuario = "25008464825"
    };

    var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

    // Perfil 3 inclui quadro societário
    var response = await httpClient.PostAsync("/cnpj/ConsultarCNPJP3", content);

    response.EnsureSuccessStatusCode();

    var responseContent = await response.Content.ReadAsStringAsync();
    var cnpjData = JsonSerializer.Deserialize<SerprocCNPJPerfil3Response>(
        responseContent,
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    return cnpjData;
}
```

### Exemplo 3: Consulta Score na SERASA

```csharp
public async Task<SerasaScoreResponse?> ConsultarScoreSerasaAsync(
    string documento,
    string tipoDocumento)
{
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("http://localhost:8000/serasa");
    httpClient.Timeout = TimeSpan.FromSeconds(25);
    httpClient.DefaultRequestHeaders.Add("User-Agent", "ConsultaDocumentos/1.0");

    var request = new SerasaScoreRequest
    {
        Documento = documento,
        TipoDocumento = tipoDocumento,
        Usuario = "usuario_serasa",
        Senha = "senha_serasa"
    };

    var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    });

    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

    var response = await httpClient.PostAsync("/score/consultar", content);

    if (!response.IsSuccessStatusCode)
    {
        throw new HttpRequestException(
            $"Erro na consulta Score SERASA: {response.StatusCode}");
    }

    var responseContent = await response.Content.ReadAsStringAsync();
    var scoreData = JsonSerializer.Deserialize<SerasaScoreResponse>(
        responseContent,
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

    if (scoreData?.Status != "SUCESSO")
    {
        throw new Exception($"Erro ao consultar score: {scoreData?.Mensagem}");
    }

    return scoreData;
}
```

### Exemplo 4: Verificação de Health Check

```csharp
public async Task<bool> VerificarDisponibilidadeProvedoresAsync()
{
    var httpClient = new HttpClient();
    httpClient.Timeout = TimeSpan.FromSeconds(5);

    var resultados = new Dictionary<string, bool>();

    // Verificar SERPRO
    try
    {
        var serproResponse = await httpClient.GetAsync(
            "http://localhost:8000/health");
        resultados["SERPRO"] = serproResponse.IsSuccessStatusCode;
    }
    catch
    {
        resultados["SERPRO"] = false;
    }

    // Verificar SERASA
    try
    {
        var serasaResponse = await httpClient.GetAsync(
            "http://localhost:8000/serasa/health");
        resultados["SERASA"] = serasaResponse.IsSuccessStatusCode;
    }
    catch
    {
        resultados["SERASA"] = false;
    }

    // Retornar true se ao menos um provedor estiver disponível
    return resultados.Values.Any(v => v);
}
```

### Exemplo 5: Retry com Backoff Exponencial

```csharp
public async Task<T?> ExecutarComRetryAsync<T>(
    Func<Task<T>> operacao,
    int maxTentativas = 3)
{
    var tentativa = 0;
    var codigosRetry = new[] { 408, 429, 500, 502, 503, 504 };

    while (tentativa < maxTentativas)
    {
        try
        {
            return await operacao();
        }
        catch (HttpRequestException ex) when (
            ex.StatusCode.HasValue &&
            codigosRetry.Contains((int)ex.StatusCode.Value))
        {
            tentativa++;

            if (tentativa >= maxTentativas)
                throw;

            // Backoff exponencial: 1s, 2s, 4s
            var delaySegundos = Math.Pow(2, tentativa - 1);
            await Task.Delay(TimeSpan.FromSeconds(delaySegundos));

            _logger.LogWarning(
                "Tentativa {Tentativa}/{MaxTentativas} após erro {StatusCode}",
                tentativa, maxTentativas, (int)ex.StatusCode.Value);
        }
    }

    return default;
}

// Uso:
var cpfData = await ExecutarComRetryAsync(
    () => ConsultarCPFSerproAsync("12345678909"),
    maxTentativas: 3
);
```

---

## Observações Finais

### Boas Práticas

1. **Sempre validar CPF/CNPJ** antes de enviar para as APIs
2. **Implementar retry** com backoff exponencial para erros temporários
3. **Configurar timeouts apropriados** para cada tipo de consulta
4. **Logar todas as requisições** para auditoria e troubleshooting
5. **Usar HttpClient compartilhado** via Dependency Injection
6. **Implementar circuit breaker** para provedores com falhas recorrentes
7. **Monitorar métricas** de taxa de sucesso e tempo de resposta
8. **Mascarar dados sensíveis** em logs (CPF/CNPJ parcial)

### Considerações de Performance

- **CPF:** ~25-30 segundos (timeout)
- **CNPJ Perfil 1:** ~30 segundos (timeout)
- **CNPJ Perfil 2/3:** ~30 segundos (timeout)
- **Score:** ~25 segundos (timeout)
- **Health Check:** ~5 segundos (timeout)

### Limites de Requisições

Consultar com cada provedor sobre limites de:
- Requisições por segundo (rate limiting)
- Requisições por dia/mês (quota)
- Tamanho máximo de payload

### Segurança

- **Nunca logar credenciais completas** (usuário/senha, tokens)
- **Usar HTTPS em produção** (endpoints de exemplo usam HTTP)
- **Rotacionar credenciais periodicamente**
- **Armazenar credenciais em cofre de senhas** (Azure Key Vault, AWS Secrets Manager)
- **Implementar autenticação mútua** com certificados quando disponível

---

**Versão:** 1.0
**Data:** 2025-10-05
**Autores:** Equipe de Desenvolvimento
