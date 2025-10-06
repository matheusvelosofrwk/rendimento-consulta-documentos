# 🚀 Quick Start - Database Initialization

## ⚡ Início Rápido

### 1. Iniciar o ambiente

```bash
cd Rendimento.ConsultaDocumentos
docker-compose up
```

### 2. Aguardar inicialização (60-90 segundos na primeira vez)

Você verá no console:
```
✓ Schema criado com sucesso!
✓ Dados de teste inseridos com sucesso!
Inicialização concluída com sucesso!
```

### 3. Pronto! 🎉

O banco está configurado com dados de teste e pronto para uso.

## 🔐 Credenciais Rápidas

### SQL Server
```
Server: localhost,1433
Database: RendimentoConsultaDocumentos
User: sa
Password: YourStrong@Password123
```

### Aplicação
| Email | Senha |
|-------|-------|
| admin@test.com | Admin@123 |
| user@test.com | User@123 |
| viewer@test.com | Viewer@123 |

## 📊 Dados de Teste

- ✅ 5 CPFs completos
- ✅ 3 CNPJs com quadro societário
- ✅ Endereços, telefones e emails
- ✅ 2 provedores configurados (SERPRO, Serasa)
- ✅ 3 aplicações
- ✅ Logs de auditoria e erro

## 🔄 Reiniciar do Zero

```bash
docker-compose down -v && docker-compose up
```

## 📚 Documentação Completa

Veja [README.md](README.md) para documentação detalhada.

## 🧪 Testar Conexão

### Via Docker Exec
```bash
docker exec -it consultadocumentos-sqlserver /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P YourStrong@Password123 -C \
  -Q "USE RendimentoConsultaDocumentos; SELECT COUNT(*) AS TotalDocumentos FROM Documento;"
```

### Via API
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@test.com","password":"Admin@123"}'
```

---

💡 **Dica**: Para ver os logs da inicialização: `docker-compose logs sqlserver`
