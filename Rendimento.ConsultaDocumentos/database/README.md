# 📦 Database Initialization Scripts

Pasta contendo scripts SQL para inicialização automática do banco de dados SQL Server com dados de teste.

## 📋 Conteúdo

- **`01-init-db.sql`** - Script de criação do schema (tabelas, índices, constraints)
- **`02-seed-data.sql`** - Script de inserção de dados de teste
- **`entrypoint.sh`** - Script de inicialização automática do SQL Server
- **`README.md`** - Esta documentação

## 🚀 Como Funciona

Quando você executa `docker-compose up`, o seguinte acontece:

1. **SQL Server é iniciado** usando a imagem oficial `mcr.microsoft.com/mssql/server:2022-latest`
2. **Entrypoint customizado** (`entrypoint.sh`) é executado
3. **Aguarda SQL Server ficar pronto** (timeout de 90 segundos)
4. **Verifica se o banco já existe**:
   - Se **NÃO existe**: Executa `01-init-db.sql` e `02-seed-data.sql`
   - Se **já existe**: Pula a inicialização (preserva dados existentes)
5. **Banco pronto para uso** com todos os dados de teste

### ⏱️ Tempo de Inicialização

- **Primeira vez**: ~60-90 segundos (criação do schema + inserção de dados)
- **Subsequentes**: ~20-30 segundos (SQL Server já configurado)

## 🔐 Credenciais

### SQL Server

```
Server: localhost,1433
Database: RendimentoConsultaDocumentos
User: sa
Password: YourStrong@Password123
```

**Connection String**:
```
Server=localhost,1433;Database=RendimentoConsultaDocumentos;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;
```

### Usuários da Aplicação

| Email | Senha | Role | Descrição |
|-------|-------|------|-----------|
| `admin@test.com` | `Admin@123` | Admin | Administrador com acesso total |
| `user@test.com` | `User@123` | User | Usuário padrão |
| `viewer@test.com` | `Viewer@123` | User | Usuário apenas visualização |

## 📊 Dados de Teste Incluídos

### 📁 Tabelas de Domínio

- **10 Nacionalidades**: Brasileira, Americana, Portuguesa, Argentina, Chilena, Espanhola, Italiana, Francesa, Alemã, Japonesa
- **8 Situações Cadastrais**: Regular, Suspensa, Inapta, Baixada, Cancelada, Nula, Ativa, Inativa

### 🏢 Configuração do Sistema

- **3 Aplicações**:
  - Sistema Web (Serpro habilitado)
  - App Mobile
  - API Externa (Serpro habilitado)

- **2 Provedores**:
  - SERPRO Mock (Prioridade 1)
  - Serasa Mock (Prioridade 2)

- **5 Relacionamentos AplicacaoProvedor** com ordem de fallback configurada

### 👤 Pessoas Físicas (CPF)

| CPF | Nome | Nascimento |
|-----|------|------------|
| 111.222.333-44 | João da Silva Santos | 15/03/1985 |
| 222.333.444-55 | Maria Oliveira Costa | 22/07/1990 |
| 333.444.555-66 | Pedro Santos Almeida | 10/11/1978 |
| 444.555.666-77 | Ana Paula Rodrigues | 30/05/1995 |
| 555.666.777-88 | Carlos Eduardo Ferreira | 18/09/1982 |

**Cada pessoa física possui**:
- ✅ Endereço completo
- ✅ Telefone(s)
- ✅ Email
- ✅ Situação cadastral (Regular)
- ✅ Nacionalidade (Brasileira)

### 🏢 Pessoas Jurídicas (CNPJ)

| CNPJ | Razão Social | Nome Fantasia | Abertura |
|------|--------------|---------------|----------|
| 11.222.333/0001-44 | Tech Solutions Desenvolvimento de Software Ltda | Tech Solutions | 10/05/2015 |
| 22.333.444/0001-55 | Comercial ABC Importação e Exportação SA | ABC Importação | 20/03/2010 |
| 33.444.555/0001-66 | Serviços XYZ Consultoria Empresarial Ltda | XYZ Consultoria | 15/11/2018 |

**Cada pessoa jurídica possui**:
- ✅ Endereço comercial
- ✅ Telefone comercial
- ✅ Email corporativo
- ✅ Quadro societário completo
- ✅ Situação cadastral (Regular)

### 👥 Quadro Societário

**Tech Solutions**:
- João da Silva Santos (60%) - Sócio Administrador
- Maria Oliveira Costa (40%) - Sócia

**Comercial ABC**:
- Pedro Santos Almeida (50%) - Diretor Presidente
- Ana Paula Rodrigues (30%) - Diretora Financeira
- Carlos Eduardo Ferreira (20%) - Diretor Comercial

**Serviços XYZ**:
- João da Silva Santos (100%) - Sócio

### 📈 Logs de Exemplo

- **5 Logs de Auditoria**: Consultas de CPF/CNPJ com métricas de performance
- **3 Logs de Erro**: Exemplos de timeout, credenciais inválidas, documentos não encontrados

## 🔄 Reinicializar o Banco

Para recriar o banco do zero (apagar todos os dados):

```bash
# Parar e remover containers e volumes
docker-compose down -v

# Subir novamente (vai recriar tudo)
docker-compose up
```

> ⚠️ **ATENÇÃO**: O comando `-v` apaga TODOS os volumes, incluindo dados do Redis. Use com cuidado!

### Reinicializar apenas o SQL Server

```bash
# Remover apenas o volume do SQL Server
docker volume rm rendimento-consultadocumentos_sqlserver-data

# Recriar o container
docker-compose up sqlserver
```

## 🧪 Testando a Inicialização

### 1. Verificar logs da inicialização

```bash
docker-compose logs sqlserver
```

Você deve ver:
```
✓ Schema criado com sucesso!
✓ Dados de teste inseridos com sucesso!
Inicialização concluída com sucesso!
```

### 2. Conectar ao banco via SQL Client

```bash
# Via docker exec
docker exec -it consultadocumentos-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Password123 -C

# Executar query de teste
1> USE RendimentoConsultaDocumentos;
2> SELECT COUNT(*) FROM Documento;
3> GO
```

Deve retornar: **8** documentos (5 CPFs + 3 CNPJs)

### 3. Testar autenticação na API

```bash
# Login com usuário admin
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"Admin@123"}'
```

## 📝 Estrutura dos Scripts SQL

### 01-init-db.sql

```sql
1. Criar database RendimentoConsultaDocumentos
2. Criar tabelas do ASP.NET Identity (AspNetUsers, AspNetRoles, etc.)
3. Criar tabelas de domínio:
   - Cliente (Legacy)
   - Nacionalidade
   - SituacaoCadastral
   - Aplicacao
   - Provedor
   - Documento (PF/PJ)
   - Endereco
   - Telefone
   - Email
   - QuadroSocietario
   - AplicacaoProvedor
   - LogAuditoria
   - LogErro
4. Criar índices e constraints
5. Criar foreign keys
```

### 02-seed-data.sql

```sql
1. Inserir Roles (Admin, User)
2. Inserir Usuários (admin, user, viewer)
3. Associar Usuários às Roles
4. Inserir Nacionalidades
5. Inserir Situações Cadastrais
6. Inserir Aplicações
7. Inserir Provedores
8. Inserir AplicacaoProvedor (relacionamentos)
9. Inserir Documentos PF (5 CPFs)
10. Inserir Endereços, Telefones, Emails para PFs
11. Inserir Documentos PJ (3 CNPJs)
12. Inserir Endereços, Telefones, Emails para PJs
13. Inserir Quadro Societário
14. Inserir Logs de Auditoria (exemplos)
15. Inserir Logs de Erro (exemplos)
```

## 🔧 Customização

### Adicionar Novos Dados de Teste

1. Edite o arquivo `02-seed-data.sql`
2. Adicione seus INSERTs no final do arquivo
3. Recrie o banco: `docker-compose down -v && docker-compose up`

### Modificar Credenciais

Para alterar a senha do SQL Server:

1. Edite `docker-compose.yml`:
```yaml
environment:
  - SA_PASSWORD=SuaNovaSenha@123
```

2. Edite `database/entrypoint.sh`:
```bash
SA_PASSWORD="${SA_PASSWORD:-SuaNovaSenha@123}"
```

3. Recrie: `docker-compose down -v && docker-compose up`

### Desabilitar Inicialização Automática

Se quiser desabilitar temporariamente a inicialização automática:

```yaml
# docker-compose.yml
sqlserver:
  # Comentar estas linhas:
  # entrypoint: /bin/bash /usr/local/bin/entrypoint.sh
  # volumes:
  #   - ./database:/docker-entrypoint-initdb.d
  #   - ./database/entrypoint.sh:/usr/local/bin/entrypoint.sh
```

## 📚 Referências

- [SQL Server Docker Documentation](https://hub.docker.com/_/microsoft-mssql-server)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [Entity Framework Core Migrations](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)

## 🐛 Troubleshooting

### Problema: Scripts SQL não são executados

**Solução**: Verifique permissões do arquivo `entrypoint.sh`:
```bash
chmod +x database/entrypoint.sh
```

### Problema: SQL Server não fica pronto

**Solução**: Aumente o timeout no `entrypoint.sh`:
```bash
MAX_TRIES=30  # De 18 para 30 (150 segundos)
```

### Problema: Erro "database already exists"

**Solução**: O banco já foi criado anteriormente. Para recriar:
```bash
docker-compose down -v
docker-compose up
```

### Problema: Permissão negada ao executar scripts

**Solução**: Verifique que os arquivos SQL estão no encoding UTF-8 sem BOM.

## 📞 Suporte

Para problemas ou dúvidas:
1. Verifique os logs: `docker-compose logs sqlserver`
2. Consulte este README
3. Verifique a documentação oficial do SQL Server Docker
