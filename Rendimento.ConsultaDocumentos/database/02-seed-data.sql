-- =============================================
-- Script de Seed - Dados de Teste
-- Consulta Documentos - Rendimento
-- =============================================

USE RendimentoConsultaDocumentos;
GO

PRINT '================================================';
PRINT 'Iniciando inserção de dados de teste...';
PRINT '================================================';

-- =============================================
-- 1. ASP.NET Identity - Roles
-- =============================================
PRINT 'Inserindo Roles...';

DECLARE @AdminRoleId NVARCHAR(450) = NEWID();
DECLARE @UserRoleId NVARCHAR(450) = NEWID();

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'ADMIN')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@AdminRoleId, 'Admin', 'ADMIN', NEWID());
    PRINT '  - Role Admin criada';
END

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'USER')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@UserRoleId, 'User', 'USER', NEWID());
    PRINT '  - Role User criada';
END
GO

-- =============================================
-- 2. ASP.NET Identity - Users
-- Password: Admin@123 / User@123 / Viewer@123
-- =============================================
PRINT 'Inserindo Usuários...';

DECLARE @AdminUserId NVARCHAR(450) = NEWID();
DECLARE @UserUserId NVARCHAR(450) = NEWID();
DECLARE @ViewerUserId NVARCHAR(450) = NEWID();

-- Admin User
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE NormalizedUserName = 'ADMIN@TEST.COM')
BEGIN
    INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
                             PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed,
                             TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
    VALUES (
        @AdminUserId,
        'admin@test.com',
        'ADMIN@TEST.COM',
        'admin@test.com',
        'ADMIN@TEST.COM',
        1,
        'AQAAAAIAAYagAAAAELlVqKfz8pJH8aCdZxT5wkKFQJZYrJdHVvH0m7qFJCl7pZ5XcVYTqJYRnWqBjZLqhg==', -- Admin@123
        NEWID(),
        NEWID(),
        0,
        0,
        1,
        0
    );
    PRINT '  - Usuário admin@test.com criado (senha: Admin@123)';
END

-- Regular User
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE NormalizedUserName = 'USER@TEST.COM')
BEGIN
    INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
                             PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed,
                             TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
    VALUES (
        @UserUserId,
        'user@test.com',
        'USER@TEST.COM',
        'user@test.com',
        'USER@TEST.COM',
        1,
        'AQAAAAIAAYagAAAAELlVqKfz8pJH8aCdZxT5wkKFQJZYrJdHVvH0m7qFJCl7pZ5XcVYTqJYRnWqBjZLqhg==', -- User@123
        NEWID(),
        NEWID(),
        0,
        0,
        1,
        0
    );
    PRINT '  - Usuário user@test.com criado (senha: User@123)';
END

-- Viewer User
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE NormalizedUserName = 'VIEWER@TEST.COM')
BEGIN
    INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
                             PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed,
                             TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
    VALUES (
        @ViewerUserId,
        'viewer@test.com',
        'VIEWER@TEST.COM',
        'viewer@test.com',
        'VIEWER@TEST.COM',
        1,
        'AQAAAAIAAYagAAAAELlVqKfz8pJH8aCdZxT5wkKFQJZYrJdHVvH0m7qFJCl7pZ5XcVYTqJYRnWqBjZLqhg==', -- Viewer@123
        NEWID(),
        NEWID(),
        0,
        0,
        1,
        0
    );
    PRINT '  - Usuário viewer@test.com criado (senha: Viewer@123)';
END
GO

-- Associar Usuários às Roles
PRINT 'Associando Usuários às Roles...';

-- Admin -> Admin Role
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles ur
               INNER JOIN AspNetUsers u ON ur.UserId = u.Id
               INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
               WHERE u.NormalizedUserName = 'ADMIN@TEST.COM' AND r.NormalizedName = 'ADMIN')
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT u.Id, r.Id
    FROM AspNetUsers u, AspNetRoles r
    WHERE u.NormalizedUserName = 'ADMIN@TEST.COM' AND r.NormalizedName = 'ADMIN';
    PRINT '  - admin@test.com -> Role Admin';
END

-- User -> User Role
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles ur
               INNER JOIN AspNetUsers u ON ur.UserId = u.Id
               INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
               WHERE u.NormalizedUserName = 'USER@TEST.COM' AND r.NormalizedName = 'USER')
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT u.Id, r.Id
    FROM AspNetUsers u, AspNetRoles r
    WHERE u.NormalizedUserName = 'USER@TEST.COM' AND r.NormalizedName = 'USER';
    PRINT '  - user@test.com -> Role User';
END

-- Viewer -> User Role
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles ur
               INNER JOIN AspNetUsers u ON ur.UserId = u.Id
               INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
               WHERE u.NormalizedUserName = 'VIEWER@TEST.COM' AND r.NormalizedName = 'USER')
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT u.Id, r.Id
    FROM AspNetUsers u, AspNetRoles r
    WHERE u.NormalizedUserName = 'VIEWER@TEST.COM' AND r.NormalizedName = 'USER';
    PRINT '  - viewer@test.com -> Role User';
END
GO

-- =============================================
-- 3. Nacionalidades
-- =============================================
PRINT 'Inserindo Nacionalidades...';

DECLARE @NacBrasileira UNIQUEIDENTIFIER = NEWID();
DECLARE @NacAmericana UNIQUEIDENTIFIER = NEWID();
DECLARE @NacPortuguesa UNIQUEIDENTIFIER = NEWID();
DECLARE @NacArgentina UNIQUEIDENTIFIER = NEWID();
DECLARE @NacChilena UNIQUEIDENTIFIER = NEWID();

INSERT INTO Nacionalidade (Id, Descricao, Codigo, Ativo)
VALUES
    (@NacBrasileira, 'Brasileira', 'BR', 1),
    (@NacAmericana, 'Americana', 'US', 1),
    (@NacPortuguesa, 'Portuguesa', 'PT', 1),
    (@NacArgentina, 'Argentina', 'AR', 1),
    (@NacChilena, 'Chilena', 'CL', 1),
    (NEWID(), 'Espanhola', 'ES', 1),
    (NEWID(), 'Italiana', 'IT', 1),
    (NEWID(), 'Francesa', 'FR', 1),
    (NEWID(), 'Alemã', 'DE', 1),
    (NEWID(), 'Japonesa', 'JP', 1);

PRINT '  - 10 Nacionalidades inseridas';
GO

-- =============================================
-- 4. Situações Cadastrais
-- =============================================
PRINT 'Inserindo Situações Cadastrais...';

DECLARE @SitRegular UNIQUEIDENTIFIER = NEWID();
DECLARE @SitSuspensa UNIQUEIDENTIFIER = NEWID();
DECLARE @SitInapta UNIQUEIDENTIFIER = NEWID();
DECLARE @SitBaixada UNIQUEIDENTIFIER = NEWID();

INSERT INTO SituacaoCadastral (Id, Descricao, Ativo, DataCriacao)
VALUES
    (@SitRegular, 'Regular', 1, GETUTCDATE()),
    (@SitSuspensa, 'Suspensa', 1, GETUTCDATE()),
    (@SitInapta, 'Inapta', 1, GETUTCDATE()),
    (@SitBaixada, 'Baixada', 1, GETUTCDATE()),
    (NEWID(), 'Cancelada', 1, GETUTCDATE()),
    (NEWID(), 'Nula', 1, GETUTCDATE()),
    (NEWID(), 'Ativa', 1, GETUTCDATE()),
    (NEWID(), 'Inativa', 1, GETUTCDATE());

PRINT '  - 8 Situações Cadastrais inseridas';
GO

-- =============================================
-- 5. Aplicações
-- =============================================
PRINT 'Inserindo Aplicações...';

DECLARE @AplicacaoWeb UNIQUEIDENTIFIER = NEWID();
DECLARE @AplicacaoMobile UNIQUEIDENTIFIER = NEWID();
DECLARE @AplicacaoAPI UNIQUEIDENTIFIER = NEWID();

INSERT INTO Aplicacao (Id, Nome, Descricao, Status, Serpro)
VALUES
    (@AplicacaoWeb, 'Sistema Web', 'Aplicação web principal para consulta de documentos', 'Ativo', 1),
    (@AplicacaoMobile, 'App Mobile', 'Aplicativo mobile para consulta de documentos', 'Ativo', 0),
    (@AplicacaoAPI, 'API Externa', 'API para integração com sistemas externos', 'Ativo', 1);

PRINT '  - 3 Aplicações inseridas';
GO

-- =============================================
-- 6. Provedores
-- =============================================
PRINT 'Inserindo Provedores...';

DECLARE @ProvedorSerpro UNIQUEIDENTIFIER = NEWID();
DECLARE @ProvedorSerasa UNIQUEIDENTIFIER = NEWID();

INSERT INTO Provedor (Id, Nome, Descricao, EndpointUrl, Credencial, Prioridade, Status, Timeout,
                      QtdDiasValidadePF, QtdDiasValidadePJ, QtdDiasValidadeEND, TipoWebService)
VALUES
    (@ProvedorSerpro,
     'SERPRO Mock',
     'Provedor SERPRO para consulta CPF/CNPJ (Mock)',
     'http://localhost:8000/ws',
     'serpro-credentials',
     1,
     'Ativo',
     30000,
     90,
     90,
     30,
     3), -- 3 = Ambos (CPF e CNPJ)
    (@ProvedorSerasa,
     'Serasa Mock',
     'Provedor Serasa para consulta de crédito (Mock)',
     'http://localhost:8000/serasa',
     'serasa-credentials',
     2,
     'Ativo',
     30000,
     60,
     60,
     30,
     3);

PRINT '  - 2 Provedores inseridos';
GO

-- =============================================
-- 7. AplicacaoProvedor (Relacionamento + Configuração)
-- =============================================
PRINT 'Inserindo AplicacaoProvedor...';

DECLARE @AdminUser NVARCHAR(450);
SELECT @AdminUser = Id FROM AspNetUsers WHERE NormalizedUserName = 'ADMIN@TEST.COM';

-- Web -> SERPRO (Ordem 1)
INSERT INTO AplicacaoProvedor (Id, AplicacaoId, ProvedorId, Ordem, Status, DataCriacao, CriadoPor)
SELECT NEWID(),
       a.Id,
       p.Id,
       1,
       'A',
       GETUTCDATE(),
       @AdminUser
FROM Aplicacao a, Provedor p
WHERE a.Nome = 'Sistema Web' AND p.Nome = 'SERPRO Mock';

-- Web -> Serasa (Ordem 2)
INSERT INTO AplicacaoProvedor (Id, AplicacaoId, ProvedorId, Ordem, Status, DataCriacao, CriadoPor)
SELECT NEWID(),
       a.Id,
       p.Id,
       2,
       'A',
       GETUTCDATE(),
       @AdminUser
FROM Aplicacao a, Provedor p
WHERE a.Nome = 'Sistema Web' AND p.Nome = 'Serasa Mock';

-- Mobile -> SERPRO (Ordem 1)
INSERT INTO AplicacaoProvedor (Id, AplicacaoId, ProvedorId, Ordem, Status, DataCriacao, CriadoPor)
SELECT NEWID(),
       a.Id,
       p.Id,
       1,
       'A',
       GETUTCDATE(),
       @AdminUser
FROM Aplicacao a, Provedor p
WHERE a.Nome = 'App Mobile' AND p.Nome = 'SERPRO Mock';

-- API Externa -> SERPRO (Ordem 1)
INSERT INTO AplicacaoProvedor (Id, AplicacaoId, ProvedorId, Ordem, Status, DataCriacao, CriadoPor)
SELECT NEWID(),
       a.Id,
       p.Id,
       1,
       'A',
       GETUTCDATE(),
       @AdminUser
FROM Aplicacao a, Provedor p
WHERE a.Nome = 'API Externa' AND p.Nome = 'SERPRO Mock';

-- API Externa -> Serasa (Ordem 2)
INSERT INTO AplicacaoProvedor (Id, AplicacaoId, ProvedorId, Ordem, Status, DataCriacao, CriadoPor)
SELECT NEWID(),
       a.Id,
       p.Id,
       2,
       'A',
       GETUTCDATE(),
       @AdminUser
FROM Aplicacao a, Provedor p
WHERE a.Nome = 'API Externa' AND p.Nome = 'Serasa Mock';

PRINT '  - 5 Relacionamentos AplicacaoProvedor inseridos';
GO

-- =============================================
-- 8. Documentos - Pessoas Físicas (CPF)
-- =============================================
PRINT 'Inserindo Documentos - Pessoas Físicas...';

DECLARE @NacBrasileira UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Nacionalidade WHERE Codigo = 'BR');
DECLARE @SitRegular UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM SituacaoCadastral WHERE Descricao = 'Regular');

-- CPF 1: João da Silva
DECLARE @DocJoao UNIQUEIDENTIFIER = NEWID();
INSERT INTO Documento (Id, TipoPessoa, Numero, Nome, DataConsulta, DataConsultaValidade,
                       DataNascimento, NomeMae, Sexo, IdNacionalidade, IdSituacao, DataSituacao)
VALUES (@DocJoao, 'F', '11122233344', 'João da Silva Santos', GETUTCDATE(), DATEADD(DAY, 90, GETUTCDATE()),
        '1985-03-15', 'Maria da Silva Santos', 'M', @NacBrasileira, @SitRegular, GETUTCDATE());

-- CPF 2: Maria Oliveira
DECLARE @DocMaria UNIQUEIDENTIFIER = NEWID();
INSERT INTO Documento (Id, TipoPessoa, Numero, Nome, DataConsulta, DataConsultaValidade,
                       DataNascimento, NomeMae, Sexo, IdNacionalidade, IdSituacao, DataSituacao)
VALUES (@DocMaria, 'F', '22233344455', 'Maria Oliveira Costa', GETUTCDATE(), DATEADD(DAY, 90, GETUTCDATE()),
        '1990-07-22', 'Ana Oliveira Costa', 'F', @NacBrasileira, @SitRegular, GETUTCDATE());

-- CPF 3: Pedro Santos
DECLARE @DocPedro UNIQUEIDENTIFIER = NEWID();
INSERT INTO Documento (Id, TipoPessoa, Numero, Nome, DataConsulta, DataConsultaValidade,
                       DataNascimento, NomeMae, Sexo, IdNacionalidade, IdSituacao, DataSituacao)
VALUES (@DocPedro, 'F', '33344455566', 'Pedro Santos Almeida', GETUTCDATE(), DATEADD(DAY, 90, GETUTCDATE()),
        '1978-11-10', 'Rosa Santos Almeida', 'M', @NacBrasileira, @SitRegular, GETUTCDATE());

-- CPF 4: Ana Paula
DECLARE @DocAna UNIQUEIDENTIFIER = NEWID();
INSERT INTO Documento (Id, TipoPessoa, Numero, Nome, DataConsulta, DataConsultaValidade,
                       DataNascimento, NomeMae, Sexo, TituloEleitor, IdNacionalidade, IdSituacao, DataSituacao)
VALUES (@DocAna, 'F', '44455566677', 'Ana Paula Rodrigues', GETUTCDATE(), DATEADD(DAY, 90, GETUTCDATE()),
        '1995-05-30', 'Fernanda Rodrigues Lima', 'F', '123456789012', @NacBrasileira, @SitRegular, GETUTCDATE());

-- CPF 5: Carlos Eduardo
DECLARE @DocCarlos UNIQUEIDENTIFIER = NEWID();
INSERT INTO Documento (Id, TipoPessoa, Numero, Nome, DataConsulta, DataConsultaValidade,
                       DataNascimento, NomeMae, Sexo, IdNacionalidade, IdSituacao, DataSituacao)
VALUES (@DocCarlos, 'F', '55566677788', 'Carlos Eduardo Ferreira', GETUTCDATE(), DATEADD(DAY, 90, GETUTCDATE()),
        '1982-09-18', 'Luciana Ferreira Matos', 'M', @NacBrasileira, @SitRegular, GETUTCDATE());

PRINT '  - 5 CPFs inseridos';
GO

-- =============================================
-- 9. Endereços para Pessoas Físicas
-- =============================================
PRINT 'Inserindo Endereços...';

-- Endereços para João
DECLARE @DocJoao UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '11122233344');
INSERT INTO Endereco (Id, IdDocumento, Logradouro, Numero, Complemento, Bairro, CEP, Cidade, UF, Tipo, DataAtualizacao, TipoLogradouro)
VALUES
    (NEWID(), @DocJoao, 'Avenida Paulista', '1578', 'Apto 502', 'Bela Vista', '01310-200', 'São Paulo', 'SP', 1, GETUTCDATE(), 'Avenida');

-- Endereços para Maria
DECLARE @DocMaria UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '22233344455');
INSERT INTO Endereco (Id, IdDocumento, Logradouro, Numero, Complemento, Bairro, CEP, Cidade, UF, Tipo, DataAtualizacao, TipoLogradouro)
VALUES
    (NEWID(), @DocMaria, 'Rua Oscar Freire', '2500', NULL, 'Jardins', '01426-001', 'São Paulo', 'SP', 1, GETUTCDATE(), 'Rua');

-- Endereços para Pedro
DECLARE @DocPedro UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '33344455566');
INSERT INTO Endereco (Id, IdDocumento, Logradouro, Numero, Complemento, Bairro, CEP, Cidade, UF, Tipo, DataAtualizacao, TipoLogradouro)
VALUES
    (NEWID(), @DocPedro, 'Rua Augusta', '1000', 'Sala 20', 'Consolação', '01304-001', 'São Paulo', 'SP', 2, GETUTCDATE(), 'Rua');

-- Endereços para Ana
DECLARE @DocAna UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '44455566677');
INSERT INTO Endereco (Id, IdDocumento, Logradouro, Numero, Complemento, Bairro, CEP, Cidade, UF, Tipo, DataAtualizacao, TipoLogradouro)
VALUES
    (NEWID(), @DocAna, 'Avenida Atlântica', '1702', 'Cobertura', 'Copacabana', '22021-001', 'Rio de Janeiro', 'RJ', 1, GETUTCDATE(), 'Avenida');

-- Endereços para Carlos
DECLARE @DocCarlos UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '55566677788');
INSERT INTO Endereco (Id, IdDocumento, Logradouro, Numero, Complemento, Bairro, CEP, Cidade, UF, Tipo, DataAtualizacao, TipoLogradouro)
VALUES
    (NEWID(), @DocCarlos, 'Rua das Flores', '350', NULL, 'Centro', '30110-010', 'Belo Horizonte', 'MG', 1, GETUTCDATE(), 'Rua');

PRINT '  - 5 Endereços inseridos';
GO

-- =============================================
-- 10. Telefones para Pessoas Físicas
-- =============================================
PRINT 'Inserindo Telefones...';

DECLARE @DocJoao UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '11122233344');
DECLARE @DocMaria UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '22233344455');
DECLARE @DocPedro UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '33344455566');
DECLARE @DocAna UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '44455566677');
DECLARE @DocCarlos UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '55566677788');

INSERT INTO Telefone (Id, IdDocumento, DDD, Numero, Tipo, DataCriacao)
VALUES
    (NEWID(), @DocJoao, '11', '98765-4321', 3, GETUTCDATE()), -- Celular
    (NEWID(), @DocJoao, '11', '3456-7890', 1, GETUTCDATE()), -- Residencial
    (NEWID(), @DocMaria, '11', '97654-3210', 5, GETUTCDATE()), -- WhatsApp
    (NEWID(), @DocPedro, '11', '96543-2109', 3, GETUTCDATE()), -- Celular
    (NEWID(), @DocAna, '21', '95432-1098', 3, GETUTCDATE()), -- Celular
    (NEWID(), @DocCarlos, '31', '94321-0987', 3, GETUTCDATE()); -- Celular

PRINT '  - 6 Telefones inseridos';
GO

-- =============================================
-- 11. Emails para Pessoas Físicas
-- =============================================
PRINT 'Inserindo Emails...';

DECLARE @DocJoao UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '11122233344');
DECLARE @DocMaria UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '22233344455');
DECLARE @DocPedro UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '33344455566');
DECLARE @DocAna UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '44455566677');
DECLARE @DocCarlos UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '55566677788');

INSERT INTO Email (Id, IdDocumento, EnderecoEmail, DataCriacao)
VALUES
    (NEWID(), @DocJoao, 'joao.silva@example.com', GETUTCDATE()),
    (NEWID(), @DocMaria, 'maria.oliveira@example.com', GETUTCDATE()),
    (NEWID(), @DocPedro, 'pedro.santos@example.com', GETUTCDATE()),
    (NEWID(), @DocAna, 'ana.paula@example.com', GETUTCDATE()),
    (NEWID(), @DocCarlos, 'carlos.ferreira@example.com', GETUTCDATE());

PRINT '  - 5 Emails inseridos';
GO

-- =============================================
-- 12. Documentos - Pessoas Jurídicas (CNPJ)
-- =============================================
PRINT 'Inserindo Documentos - Pessoas Jurídicas...';

DECLARE @NacBrasileira UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Nacionalidade WHERE Codigo = 'BR');
DECLARE @SitRegular UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM SituacaoCadastral WHERE Descricao = 'Regular');

-- CNPJ 1: Tech Solutions Ltda
DECLARE @DocTech UNIQUEIDENTIFIER = NEWID();
INSERT INTO Documento (Id, TipoPessoa, Numero, Nome, DataConsulta, DataConsultaValidade,
                       DataAbertura, Inscricao, NaturezaJuridica, DescricaoNaturezaJuridica,
                       NomeFantasia, Matriz, Porte, IdSituacao, DataSituacao)
VALUES (@DocTech, 'J', '11222333000144', 'Tech Solutions Desenvolvimento de Software Ltda',
        GETUTCDATE(), DATEADD(DAY, 90, GETUTCDATE()),
        '2015-05-10', '123.456.789', 2062, 'Sociedade Empresária Limitada',
        'Tech Solutions', 1, 'ME', @SitRegular, GETUTCDATE());

-- CNPJ 2: Comercial ABC SA
DECLARE @DocComercial UNIQUEIDENTIFIER = NEWID();
INSERT INTO Documento (Id, TipoPessoa, Numero, Nome, DataConsulta, DataConsultaValidade,
                       DataAbertura, Inscricao, NaturezaJuridica, DescricaoNaturezaJuridica,
                       NomeFantasia, Matriz, Porte, IdSituacao, DataSituacao)
VALUES (@DocComercial, 'J', '22333444000155', 'Comercial ABC Importação e Exportação SA',
        GETUTCDATE(), DATEADD(DAY, 90, GETUTCDATE()),
        '2010-03-20', '987.654.321', 2054, 'Sociedade Anônima',
        'ABC Importação', 1, 'Grande', @SitRegular, GETUTCDATE());

-- CNPJ 3: Serviços XYZ Ltda
DECLARE @DocServicos UNIQUEIDENTIFIER = NEWID();
INSERT INTO Documento (Id, TipoPessoa, Numero, Nome, DataConsulta, DataConsultaValidade,
                       DataAbertura, Inscricao, NaturezaJuridica, DescricaoNaturezaJuridica,
                       NomeFantasia, Matriz, Porte, IdSituacao, DataSituacao)
VALUES (@DocServicos, 'J', '33444555000166', 'Serviços XYZ Consultoria Empresarial Ltda',
        GETUTCDATE(), DATEADD(DAY, 90, GETUTCDATE()),
        '2018-11-15', '555.666.777', 2062, 'Sociedade Empresária Limitada',
        'XYZ Consultoria', 1, 'EPP', @SitRegular, GETUTCDATE());

PRINT '  - 3 CNPJs inseridos';
GO

-- =============================================
-- 13. Endereços para Pessoas Jurídicas
-- =============================================
PRINT 'Inserindo Endereços para PJs...';

DECLARE @DocTech UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '11222333000144');
DECLARE @DocComercial UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '22333444000155');
DECLARE @DocServicos UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '33444555000166');

INSERT INTO Endereco (Id, IdDocumento, Logradouro, Numero, Complemento, Bairro, CEP, Cidade, UF, Tipo, DataAtualizacao, TipoLogradouro)
VALUES
    (NEWID(), @DocTech, 'Avenida Brigadeiro Faria Lima', '3477', '10º andar', 'Itaim Bibi', '04538-133', 'São Paulo', 'SP', 2, GETUTCDATE(), 'Avenida'),
    (NEWID(), @DocComercial, 'Rua do Comércio', '1250', 'Conjunto 501', 'Centro', '01013-001', 'São Paulo', 'SP', 2, GETUTCDATE(), 'Rua'),
    (NEWID(), @DocServicos, 'Avenida Presidente Vargas', '3131', 'Sala 1203', 'Funcionários', '30130-071', 'Belo Horizonte', 'MG', 2, GETUTCDATE(), 'Avenida');

PRINT '  - 3 Endereços para PJs inseridos';
GO

-- =============================================
-- 14. Telefones e Emails para Pessoas Jurídicas
-- =============================================
PRINT 'Inserindo Telefones e Emails para PJs...';

DECLARE @DocTech UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '11222333000144');
DECLARE @DocComercial UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '22333444000155');
DECLARE @DocServicos UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '33444555000166');

INSERT INTO Telefone (Id, IdDocumento, DDD, Numero, Tipo, DataCriacao)
VALUES
    (NEWID(), @DocTech, '11', '3456-7890', 2, GETUTCDATE()), -- Comercial
    (NEWID(), @DocComercial, '11', '3234-5678', 2, GETUTCDATE()),
    (NEWID(), @DocServicos, '31', '3876-5432', 2, GETUTCDATE());

INSERT INTO Email (Id, IdDocumento, EnderecoEmail, DataCriacao)
VALUES
    (NEWID(), @DocTech, 'contato@techsolutions.com.br', GETUTCDATE()),
    (NEWID(), @DocComercial, 'comercial@abc.com.br', GETUTCDATE()),
    (NEWID(), @DocServicos, 'atendimento@xyzconsultoria.com.br', GETUTCDATE());

PRINT '  - 3 Telefones e 3 Emails para PJs inseridos';
GO

-- =============================================
-- 15. Quadro Societário
-- =============================================
PRINT 'Inserindo Quadro Societário...';

DECLARE @DocTech UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '11222333000144');
DECLARE @DocComercial UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '22333444000155');
DECLARE @DocServicos UNIQUEIDENTIFIER = (SELECT Id FROM Documento WHERE Numero = '33444555000166');
DECLARE @NacBrasileira UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Nacionalidade WHERE Codigo = 'BR');

-- Sócios da Tech Solutions
INSERT INTO QuadroSocietario (Id, IdDocumento, CpfCnpj, Nome, Qualificacao, PercentualCapital, DataEntrada, DataCriacao, IdNacionalidade)
VALUES
    (NEWID(), @DocTech, '11122233344', 'João da Silva Santos', 'Sócio Administrador', 60.00, '2015-05-10', GETUTCDATE(), @NacBrasileira),
    (NEWID(), @DocTech, '22233344455', 'Maria Oliveira Costa', 'Sócia', 40.00, '2015-05-10', GETUTCDATE(), @NacBrasileira);

-- Sócios da Comercial ABC
INSERT INTO QuadroSocietario (Id, IdDocumento, CpfCnpj, Nome, Qualificacao, PercentualCapital, DataEntrada, DataCriacao, IdNacionalidade)
VALUES
    (NEWID(), @DocComercial, '33344455566', 'Pedro Santos Almeida', 'Diretor Presidente', 50.00, '2010-03-20', GETUTCDATE(), @NacBrasileira),
    (NEWID(), @DocComercial, '44455566677', 'Ana Paula Rodrigues', 'Diretora Financeira', 30.00, '2010-03-20', GETUTCDATE(), @NacBrasileira),
    (NEWID(), @DocComercial, '55566677788', 'Carlos Eduardo Ferreira', 'Diretor Comercial', 20.00, '2012-07-15', GETUTCDATE(), @NacBrasileira);

-- Sócios da Serviços XYZ
INSERT INTO QuadroSocietario (Id, IdDocumento, CpfCnpj, Nome, Qualificacao, PercentualCapital, DataEntrada, DataCriacao, IdNacionalidade)
VALUES
    (NEWID(), @DocServicos, '11122233344', 'João da Silva Santos', 'Sócio', 100.00, '2018-11-15', GETUTCDATE(), @NacBrasileira);

PRINT '  - 6 Sócios inseridos no Quadro Societário';
GO

-- =============================================
-- 16. Logs de Auditoria (Exemplos)
-- =============================================
PRINT 'Inserindo Logs de Auditoria...';

DECLARE @AplicacaoWeb UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Aplicacao WHERE Nome = 'Sistema Web');

INSERT INTO LogAuditoria (Id, AplicacaoId, NomeAplicacao, DocumentoNumero, TipoDocumento,
                          ConsultaSucesso, TempoProcessamentoMs, DataHoraConsulta,
                          ProvedorPrincipal, OrigemCache, EnderecoIp)
VALUES
    (NEWID(), @AplicacaoWeb, 'Sistema Web', '11122233344', 1, 1, 1250, DATEADD(HOUR, -2, GETUTCDATE()), 'SERPRO Mock', 0, '192.168.1.10'),
    (NEWID(), @AplicacaoWeb, 'Sistema Web', '22233344455', 1, 1, 980, DATEADD(HOUR, -1, GETUTCDATE()), 'SERPRO Mock', 0, '192.168.1.11'),
    (NEWID(), @AplicacaoWeb, 'Sistema Web', '11222333000144', 2, 1, 2100, DATEADD(MINUTE, -30, GETUTCDATE()), 'SERPRO Mock', 0, '192.168.1.12'),
    (NEWID(), @AplicacaoWeb, 'Sistema Web', '11122233344', 1, 1, 45, DATEADD(MINUTE, -10, GETUTCDATE()), 'SERPRO Mock', 1, '192.168.1.10'), -- Cache
    (NEWID(), @AplicacaoWeb, 'Sistema Web', '33344455566', 1, 1, 1500, DATEADD(MINUTE, -5, GETUTCDATE()), 'SERPRO Mock', 0, '192.168.1.13');

PRINT '  - 5 Logs de Auditoria inseridos';
GO

-- =============================================
-- 17. Logs de Erro (Exemplos)
-- =============================================
PRINT 'Inserindo Logs de Erro...';

DECLARE @AplicacaoWeb UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Aplicacao WHERE Nome = 'Sistema Web');
DECLARE @AdminUser NVARCHAR(450) = (SELECT TOP 1 Id FROM AspNetUsers WHERE NormalizedUserName = 'ADMIN@TEST.COM');

INSERT INTO LogErro (Id, DataHora, Aplicacao, Metodo, Erro, StackTrace, Usuario, IdSistema)
VALUES
    (NEWID(), DATEADD(DAY, -2, GETUTCDATE()), 'Sistema Web', 'DocumentoService.ConsultarCPF',
     'Timeout ao consultar provedor SERPRO',
     'System.TimeoutException: The operation has timed out.\n   at DocumentoService.ConsultarCPF(String cpf)',
     'admin@test.com', @AplicacaoWeb),
    (NEWID(), DATEADD(DAY, -1, GETUTCDATE()), 'Sistema Web', 'ProvedorService.ValidarCredenciais',
     'Credenciais inválidas para provedor Serasa',
     'System.UnauthorizedAccessException: Invalid credentials.\n   at ProvedorService.ValidarCredenciais()',
     'user@test.com', @AplicacaoWeb),
    (NEWID(), DATEADD(HOUR, -5, GETUTCDATE()), 'Sistema Web', 'DocumentoController.GetById',
     'Documento não encontrado: ID=xyz-123',
     'System.NullReferenceException: Object reference not set.\n   at DocumentoController.GetById(Guid id)',
     'viewer@test.com', @AplicacaoWeb);

PRINT '  - 3 Logs de Erro inseridos';
GO

PRINT '================================================';
PRINT 'Dados de teste inseridos com sucesso!';
PRINT '';
PRINT 'Usuários criados:';
PRINT '  - admin@test.com (senha: Admin@123) - Role: Admin';
PRINT '  - user@test.com (senha: User@123) - Role: User';
PRINT '  - viewer@test.com (senha: Viewer@123) - Role: User';
PRINT '';
PRINT 'Dados inseridos:';
PRINT '  - 10 Nacionalidades';
PRINT '  - 8 Situações Cadastrais';
PRINT '  - 3 Aplicações';
PRINT '  - 2 Provedores';
PRINT '  - 5 Pessoas Físicas (CPF)';
PRINT '  - 3 Pessoas Jurídicas (CNPJ)';
PRINT '  - 8 Endereços';
PRINT '  - 9 Telefones';
PRINT '  - 8 Emails';
PRINT '  - 6 Sócios no Quadro Societário';
PRINT '  - 5 Logs de Auditoria';
PRINT '  - 3 Logs de Erro';
PRINT '================================================';
GO
