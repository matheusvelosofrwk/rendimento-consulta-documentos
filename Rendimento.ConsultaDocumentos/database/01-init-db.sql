-- =============================================
-- Script de Inicialização do Banco de Dados
-- Consulta Documentos - Rendimento
-- =============================================

USE master;
GO

-- Criar database se não existir
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'RendimentoConsultaDocumentos')
BEGIN
    CREATE DATABASE RendimentoConsultaDocumentos;
    PRINT 'Database RendimentoConsultaDocumentos criado com sucesso!';
END
ELSE
BEGIN
    PRINT 'Database RendimentoConsultaDocumentos já existe.';
END
GO

USE RendimentoConsultaDocumentos;
GO

-- =============================================
-- ASP.NET Identity Tables
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetRoles')
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
    CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
    PRINT 'Tabela AspNetRoles criada.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset(7) NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
    CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
    PRINT 'Tabela AspNetUsers criada.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserClaims')
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
    PRINT 'Tabela AspNetUserClaims criada.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserLogins')
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
    PRINT 'Tabela AspNetUserLogins criada.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserRoles')
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
    PRINT 'Tabela AspNetUserRoles criada.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUserTokens')
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    PRINT 'Tabela AspNetUserTokens criada.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetRoleClaims')
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
    PRINT 'Tabela AspNetRoleClaims criada.';
END
GO

-- =============================================
-- Domain Tables
-- =============================================

-- Cliente (Legacy)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cliente')
BEGIN
    CREATE TABLE [Cliente] (
        [Id] uniqueidentifier NOT NULL,
        [Nome] nvarchar(max) NOT NULL,
        [CPF] nvarchar(11) NOT NULL,
        [Endereco] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Cliente] PRIMARY KEY ([Id])
    );
    PRINT 'Tabela Cliente criada.';
END
GO

-- Nacionalidade
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Nacionalidade')
BEGIN
    CREATE TABLE [Nacionalidade] (
        [Id] uniqueidentifier NOT NULL,
        [Descricao] nvarchar(100) NOT NULL,
        [Codigo] nvarchar(10) NULL,
        [Ativo] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Nacionalidade] PRIMARY KEY ([Id])
    );
    PRINT 'Tabela Nacionalidade criada.';
END
GO

-- SituacaoCadastral
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SituacaoCadastral')
BEGIN
    CREATE TABLE [SituacaoCadastral] (
        [Id] uniqueidentifier NOT NULL,
        [Descricao] nvarchar(100) NOT NULL,
        [Ativo] bit NOT NULL DEFAULT 1,
        [DataCriacao] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_SituacaoCadastral] PRIMARY KEY ([Id])
    );
    PRINT 'Tabela SituacaoCadastral criada.';
END
GO

-- Aplicacao
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Aplicacao')
BEGIN
    CREATE TABLE [Aplicacao] (
        [Id] uniqueidentifier NOT NULL,
        [Nome] nvarchar(100) NOT NULL,
        [Descricao] nvarchar(500) NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [Serpro] bit NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Aplicacao] PRIMARY KEY ([Id])
    );
    PRINT 'Tabela Aplicacao criada.';
END
GO

-- Provedor
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Provedor')
BEGIN
    CREATE TABLE [Provedor] (
        [Id] uniqueidentifier NOT NULL,
        [Nome] nvarchar(100) NOT NULL,
        [Descricao] nvarchar(500) NOT NULL,
        [EndpointUrl] nvarchar(500) NOT NULL,
        [Credencial] nvarchar(500) NOT NULL,
        [Prioridade] int NOT NULL,
        [Status] nvarchar(50) NOT NULL,
        [Timeout] int NOT NULL,
        [EndCertificado] nvarchar(500) NULL,
        [Usuario] nvarchar(200) NULL,
        [Senha] nvarchar(500) NULL,
        [Dominio] nvarchar(200) NULL,
        [QtdAcessoMinimo] int NULL,
        [QtdAcessoMaximo] int NULL,
        [QtdDiasValidadePF] int NOT NULL DEFAULT 30,
        [QtdDiasValidadePJ] int NOT NULL DEFAULT 30,
        [QtdDiasValidadeEND] int NOT NULL DEFAULT 30,
        [QtdMinEmailLog] int NULL,
        [DiaCorte] int NULL,
        [Porta] int NULL,
        [TipoWebService] int NOT NULL DEFAULT 3,
        CONSTRAINT [PK_Provedor] PRIMARY KEY ([Id])
    );
    PRINT 'Tabela Provedor criada.';
END
GO

-- Documento
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Documento')
BEGIN
    CREATE TABLE [Documento] (
        [Id] uniqueidentifier NOT NULL,
        [TipoPessoa] char(1) NOT NULL,
        [Numero] nvarchar(20) NOT NULL,
        [Nome] nvarchar(200) NOT NULL,
        [DataConsulta] datetime2 NOT NULL,
        [DataConsultaValidade] datetime2 NOT NULL,
        [RowVersion] rowversion NULL,

        -- Campos PJ
        [DataAbertura] datetime2 NULL,
        [Inscricao] nvarchar(50) NULL,
        [NaturezaJuridica] int NULL,
        [DescricaoNaturezaJuridica] nvarchar(200) NULL,
        [Segmento] nvarchar(100) NULL,
        [RamoAtividade] int NULL,
        [DescricaoRamoAtividade] nvarchar(200) NULL,
        [NomeFantasia] nvarchar(200) NULL,
        [MatrizFilialQtde] int NULL,
        [Matriz] bit NULL,
        [Porte] nvarchar(50) NULL,

        -- Campos PF
        [DataNascimento] datetime2 NULL,
        [NomeMae] nvarchar(200) NULL,
        [Sexo] nvarchar(10) NULL,
        [TituloEleitor] nvarchar(20) NULL,
        [ResidenteExterior] bit NULL,
        [AnoObito] int NULL,
        [NomeSocial] nvarchar(100) NULL,

        -- Campos de Situação
        [DataSituacao] datetime2 NULL,
        [IdSituacao] uniqueidentifier NULL,
        [CodControle] nvarchar(50) NULL,
        [DataFundacao] datetime2 NULL,
        [OrigemBureau] nvarchar(100) NULL,
        [IdNacionalidade] uniqueidentifier NULL,

        CONSTRAINT [PK_Documento] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Documento_Nacionalidade] FOREIGN KEY ([IdNacionalidade]) REFERENCES [Nacionalidade]([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Documento_SituacaoCadastral] FOREIGN KEY ([IdSituacao]) REFERENCES [SituacaoCadastral]([Id]) ON DELETE SET NULL
    );

    CREATE UNIQUE INDEX [IX_Documento_Numero] ON [Documento] ([Numero]);
    CREATE INDEX [IX_Documento_DataConsultaValidade] ON [Documento] ([DataConsultaValidade]);
    CREATE INDEX [IX_Documento_IdNacionalidade] ON [Documento] ([IdNacionalidade]);
    CREATE INDEX [IX_Documento_IdSituacao] ON [Documento] ([IdSituacao]);

    PRINT 'Tabela Documento criada.';
END
GO

-- Endereco
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Endereco')
BEGIN
    CREATE TABLE [Endereco] (
        [Id] uniqueidentifier NOT NULL,
        [IdDocumento] uniqueidentifier NOT NULL,
        [Logradouro] nvarchar(300) NULL,
        [Numero] nvarchar(20) NULL,
        [Complemento] nvarchar(100) NULL,
        [Bairro] nvarchar(100) NULL,
        [CEP] nvarchar(10) NULL,
        [Cidade] nvarchar(100) NULL,
        [UF] nvarchar(2) NULL,
        [Tipo] int NOT NULL,
        [DataAtualizacao] datetime2 NULL,
        [RowVersion] rowversion NULL,
        [TipoLogradouro] nvarchar(50) NULL,
        CONSTRAINT [PK_Endereco] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Endereco_Documento] FOREIGN KEY ([IdDocumento]) REFERENCES [Documento]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_Endereco_IdDocumento] ON [Endereco] ([IdDocumento]);

    PRINT 'Tabela Endereco criada.';
END
GO

-- Telefone
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Telefone')
BEGIN
    CREATE TABLE [Telefone] (
        [Id] uniqueidentifier NOT NULL,
        [IdDocumento] uniqueidentifier NOT NULL,
        [DDD] nvarchar(3) NULL,
        [Numero] nvarchar(15) NULL,
        [Tipo] int NOT NULL,
        [DataCriacao] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [DataAtualizacao] datetime2 NULL,
        CONSTRAINT [PK_Telefone] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Telefone_Documento] FOREIGN KEY ([IdDocumento]) REFERENCES [Documento]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_Telefone_IdDocumento] ON [Telefone] ([IdDocumento]);

    PRINT 'Tabela Telefone criada.';
END
GO

-- Email
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Email')
BEGIN
    CREATE TABLE [Email] (
        [Id] uniqueidentifier NOT NULL,
        [IdDocumento] uniqueidentifier NOT NULL,
        [EnderecoEmail] nvarchar(200) NULL,
        [DataCriacao] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_Email] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Email_Documento] FOREIGN KEY ([IdDocumento]) REFERENCES [Documento]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_Email_IdDocumento] ON [Email] ([IdDocumento]);

    PRINT 'Tabela Email criada.';
END
GO

-- QuadroSocietario
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'QuadroSocietario')
BEGIN
    CREATE TABLE [QuadroSocietario] (
        [Id] uniqueidentifier NOT NULL,
        [IdDocumento] uniqueidentifier NOT NULL,

        -- Campos legados
        [CPFSocio] nvarchar(20) NULL,
        [NomeSocio] nvarchar(200) NULL,
        [QualificacaoSocio] nvarchar(200) NULL,

        -- Campos novos
        [CpfCnpj] nvarchar(20) NULL,
        [Nome] nvarchar(200) NULL,
        [Qualificacao] nvarchar(200) NULL,

        -- Representante legal
        [CpfRepresentanteLegal] nvarchar(20) NULL,
        [NomeRepresentanteLegal] nvarchar(200) NULL,
        [QualificacaoRepresentanteLegal] nvarchar(200) NULL,

        -- Outras informações
        [DataEntrada] datetime2 NULL,
        [DataSaida] datetime2 NULL,
        [PercentualCapital] decimal(5,2) NULL,
        [DataCriacao] datetime2 NOT NULL DEFAULT GETUTCDATE(),

        -- Novos campos
        [Tipo] nvarchar(50) NULL,
        [IdNacionalidade] uniqueidentifier NULL,
        [CodPaisOrigem] nvarchar(10) NULL,
        [NomePaisOrigem] nvarchar(100) NULL,

        CONSTRAINT [PK_QuadroSocietario] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_QuadroSocietario_Documento] FOREIGN KEY ([IdDocumento]) REFERENCES [Documento]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_QuadroSocietario_Nacionalidade] FOREIGN KEY ([IdNacionalidade]) REFERENCES [Nacionalidade]([Id]) ON DELETE SET NULL
    );

    CREATE INDEX [IX_QuadroSocietario_IdDocumento] ON [QuadroSocietario] ([IdDocumento]);
    CREATE INDEX [IX_QuadroSocietario_IdNacionalidade] ON [QuadroSocietario] ([IdNacionalidade]);

    PRINT 'Tabela QuadroSocietario criada.';
END
GO

-- AplicacaoProvedor
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AplicacaoProvedor')
BEGIN
    CREATE TABLE [AplicacaoProvedor] (
        [Id] uniqueidentifier NOT NULL,
        [AplicacaoId] uniqueidentifier NOT NULL,
        [ProvedorId] uniqueidentifier NOT NULL,
        [Ordem] int NOT NULL,
        [Status] char(1) NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [DataAtualizacao] datetime2 NULL,
        [CriadoPor] nvarchar(450) NULL,
        [AtualizadoPor] nvarchar(450) NULL,

        -- Campos de LOG DE USO (billing)
        [IdDocumento] uniqueidentifier NULL,
        [DataConsulta] datetime2 NULL,
        [EnderecoIP] nvarchar(50) NULL,
        [RemoteHost] nvarchar(100) NULL,

        CONSTRAINT [PK_AplicacaoProvedor] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AplicacaoProvedor_Aplicacao] FOREIGN KEY ([AplicacaoId]) REFERENCES [Aplicacao]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AplicacaoProvedor_Provedor] FOREIGN KEY ([ProvedorId]) REFERENCES [Provedor]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AplicacaoProvedor_Documento] FOREIGN KEY ([IdDocumento]) REFERENCES [Documento]([Id]) ON DELETE SET NULL
    );

    CREATE INDEX [IX_AplicacaoProvedor_AplicacaoId] ON [AplicacaoProvedor] ([AplicacaoId]);
    CREATE INDEX [IX_AplicacaoProvedor_ProvedorId] ON [AplicacaoProvedor] ([ProvedorId]);
    CREATE INDEX [IX_AplicacaoProvedor_Ordem] ON [AplicacaoProvedor] ([Ordem]);
    CREATE INDEX [IX_AplicacaoProvedor_DataConsulta] ON [AplicacaoProvedor] ([DataConsulta]);
    CREATE INDEX [IX_AplicacaoProvedor_IdDocumento] ON [AplicacaoProvedor] ([IdDocumento]);
    CREATE UNIQUE INDEX [IX_AplicacaoProvedor_Aplicacao_Provedor] ON [AplicacaoProvedor] ([AplicacaoId], [ProvedorId]);
    CREATE INDEX [IX_AplicacaoProvedor_Billing] ON [AplicacaoProvedor] ([AplicacaoId], [ProvedorId], [DataConsulta]);

    PRINT 'Tabela AplicacaoProvedor criada.';
END
GO

-- LogAuditoria
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LogAuditoria')
BEGIN
    CREATE TABLE [LogAuditoria] (
        [Id] uniqueidentifier NOT NULL,
        [AplicacaoId] uniqueidentifier NOT NULL,
        [NomeAplicacao] nvarchar(200) NOT NULL,
        [DocumentoNumero] nvarchar(20) NOT NULL,
        [TipoDocumento] int NOT NULL,

        -- Parâmetros e resultado
        [ParametrosEntrada] nvarchar(max) NULL,
        [ProvedoresUtilizados] nvarchar(500) NULL,
        [ProvedorPrincipal] nvarchar(100) NULL,
        [ConsultaSucesso] bit NOT NULL,
        [RespostaProvedor] nvarchar(max) NULL,
        [MensagemRetorno] nvarchar(max) NULL,

        -- Métricas
        [TempoProcessamentoMs] bigint NOT NULL,
        [DataHoraConsulta] datetime2 NOT NULL,

        -- Informações de requisição
        [EnderecoIp] nvarchar(50) NULL,
        [UserAgent] nvarchar(500) NULL,
        [TokenAutenticacao] nvarchar(500) NULL,

        -- Controle
        [OrigemCache] bit NOT NULL DEFAULT 0,
        [InformacoesAdicionais] nvarchar(max) NULL,

        CONSTRAINT [PK_LogAuditoria] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LogAuditoria_Aplicacao] FOREIGN KEY ([AplicacaoId]) REFERENCES [Aplicacao]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_LogAuditoria_AplicacaoId] ON [LogAuditoria] ([AplicacaoId]);
    CREATE INDEX [IX_LogAuditoria_DataHoraConsulta] ON [LogAuditoria] ([DataHoraConsulta]);
    CREATE INDEX [IX_LogAuditoria_DocumentoNumero] ON [LogAuditoria] ([DocumentoNumero]);

    PRINT 'Tabela LogAuditoria criada.';
END
GO

-- LogErro
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LogErro')
BEGIN
    CREATE TABLE [LogErro] (
        [Id] uniqueidentifier NOT NULL,
        [DataHora] datetime2 NOT NULL,
        [Aplicacao] nvarchar(200) NULL,
        [Metodo] nvarchar(200) NULL,
        [Erro] nvarchar(max) NULL,
        [StackTrace] nvarchar(max) NULL,
        [Usuario] nvarchar(200) NULL,
        [IdSistema] uniqueidentifier NULL,
        CONSTRAINT [PK_LogErro] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LogErro_Aplicacao] FOREIGN KEY ([IdSistema]) REFERENCES [Aplicacao]([Id]) ON DELETE SET NULL
    );

    CREATE INDEX [IX_LogErro_IdSistema] ON [LogErro] ([IdSistema]);
    CREATE INDEX [IX_LogErro_DataHora] ON [LogErro] ([DataHora]);

    PRINT 'Tabela LogErro criada.';
END
GO

PRINT '================================================';
PRINT 'Schema do banco de dados criado com sucesso!';
PRINT '================================================';
GO
