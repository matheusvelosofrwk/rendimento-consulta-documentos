#!/bin/bash
# =============================================
# Entrypoint Script para SQL Server
# Executa scripts de inicialização automaticamente
# =============================================

set -e

# Variáveis de ambiente
SA_PASSWORD="${SA_PASSWORD:-YourStrong@Password123}"
ACCEPT_EULA="${ACCEPT_EULA:-Y}"

echo "================================================"
echo "Iniciando SQL Server..."
echo "================================================"

# Iniciar SQL Server em background
/opt/mssql/bin/sqlservr &

# Aguardar SQL Server ficar pronto
echo "Aguardando SQL Server inicializar..."
sleep 30s

# Função para verificar se SQL Server está pronto
check_sql_server() {
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" > /dev/null 2>&1
    return $?
}

# Aguardar até SQL Server estar pronto (timeout de 90 segundos)
COUNTER=0
MAX_TRIES=18

until check_sql_server || [ $COUNTER -eq $MAX_TRIES ]; do
    echo "SQL Server ainda não está pronto... tentativa $((COUNTER+1))/$MAX_TRIES"
    sleep 5s
    COUNTER=$((COUNTER+1))
done

if [ $COUNTER -eq $MAX_TRIES ]; then
    echo "ERROR: SQL Server não ficou pronto após 90 segundos!"
    exit 1
fi

echo "================================================"
echo "SQL Server pronto! Executando scripts de inicialização..."
echo "================================================"

# Verificar se o banco já existe
DB_EXISTS=$(/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -h -1 -Q "SET NOCOUNT ON; SELECT name FROM sys.databases WHERE name='RendimentoConsultaDocumentos'" | tr -d '[:space:]')

if [ -z "$DB_EXISTS" ]; then
    echo "Database não encontrado. Criando estrutura..."

    # Executar script de inicialização (schema)
    if [ -f /docker-entrypoint-initdb.d/01-init-db.sql ]; then
        echo "Executando 01-init-db.sql..."
        /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -i /docker-entrypoint-initdb.d/01-init-db.sql

        if [ $? -eq 0 ]; then
            echo "✓ Schema criado com sucesso!"
        else
            echo "✗ ERRO ao criar schema!"
            exit 1
        fi
    else
        echo "AVISO: Arquivo 01-init-db.sql não encontrado!"
    fi

    # Executar script de seed (dados de teste)
    if [ -f /docker-entrypoint-initdb.d/02-seed-data.sql ]; then
        echo "Executando 02-seed-data.sql..."
        /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -i /docker-entrypoint-initdb.d/02-seed-data.sql

        if [ $? -eq 0 ]; then
            echo "✓ Dados de teste inseridos com sucesso!"
        else
            echo "✗ ERRO ao inserir dados de teste!"
            exit 1
        fi
    else
        echo "AVISO: Arquivo 02-seed-data.sql não encontrado!"
    fi

    echo "================================================"
    echo "Inicialização concluída com sucesso!"
    echo "================================================"
    echo ""
    echo "Credenciais de acesso:"
    echo "  Database: RendimentoConsultaDocumentos"
    echo "  Server: localhost,1433"
    echo "  User: sa"
    echo "  Password: $SA_PASSWORD"
    echo ""
    echo "Usuários da aplicação:"
    echo "  - admin@test.com (senha: Admin@123)"
    echo "  - user@test.com (senha: User@123)"
    echo "  - viewer@test.com (senha: Viewer@123)"
    echo "================================================"
else
    echo "Database 'RendimentoConsultaDocumentos' já existe. Pulando inicialização."
    echo "Para reinicializar, remova o volume do Docker:"
    echo "  docker-compose down -v"
    echo "  docker-compose up"
fi

# Manter o processo rodando (aguardar processo do SQL Server)
wait
