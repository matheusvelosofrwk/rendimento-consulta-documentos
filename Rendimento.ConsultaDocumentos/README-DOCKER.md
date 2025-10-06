# Docker e Health Check - Rendimento.ConsultaDocumentos

Este documento descreve como executar a aplicação usando Docker e como funcionam os health checks implementados.

## 📋 Índice

- [Health Checks](#health-checks)
- [Docker](#docker)
- [Docker Compose](#docker-compose)
- [Comandos Úteis](#comandos-úteis)
- [Troubleshooting](#troubleshooting)

## 🏥 Health Checks

A aplicação implementa três endpoints de health check:

### 1. `/health` - Liveness Probe
- **Objetivo**: Verificar se a aplicação está rodando
- **Uso**: Liveness probe do Kubernetes/Docker
- **Resposta**: Status 200 se a aplicação estiver viva
- **Verificações**: Nenhuma (apenas verifica se o processo está respondendo)

```bash
curl http://localhost:5000/health
```

### 2. `/health/ready` - Readiness Probe
- **Objetivo**: Verificar se a aplicação está pronta para receber tráfego
- **Uso**: Readiness probe do Kubernetes/Docker
- **Resposta**: Status 200 + JSON detalhado com status de cada componente
- **Verificações**:
  - ✅ Banco de dados SQL Server (conexão e query)
  - ✅ Redis (se habilitado)
  - ✅ Provedores externos (configuração e conectividade)

```bash
curl http://localhost:5000/health/ready
```

**Exemplo de resposta:**
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.1234567",
  "entries": {
    "database": {
      "status": "Healthy",
      "duration": "00:00:00.0567890"
    },
    "sqlserver": {
      "status": "Healthy",
      "duration": "00:00:00.0234567"
    },
    "redis": {
      "status": "Healthy",
      "duration": "00:00:00.0123456"
    },
    "external_providers": {
      "status": "Healthy",
      "duration": "00:00:00.0345678",
      "data": {
        "UseMockProviders": true,
        "SerproConfigured": true,
        "SerasaConfigured": true
      }
    }
  }
}
```

### 3. `/health/detailed` - Health Check Detalhado
- **Objetivo**: Monitoramento e debugging
- **Uso**: Monitoramento manual ou ferramentas de observabilidade
- **Resposta**: Status 200 + JSON completo com todos os checks
- **Verificações**: Todos os health checks disponíveis

```bash
curl http://localhost:5000/health/detailed
```

## 🐳 Docker

### Construir a imagem

```bash
# Navegar até a pasta da solução
cd Rendimento.ConsultaDocumentos

# Construir a imagem
docker build -t consultadocumentos-api:latest .
```

### Executar o container

```bash
docker run -d \
  --name consultadocumentos-api \
  -p 5000:8080 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=RendimentoConsultaDocumentos;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;" \
  -e ASPNETCORE_ENVIRONMENT=Production \
  consultadocumentos-api:latest
```

### Verificar health do container

```bash
# Ver status do health check
docker ps

# Inspecionar detalhes do health check
docker inspect --format='{{json .State.Health}}' consultadocumentos-api | jq
```

## 🐙 Docker Compose

O `docker-compose.yml` orquestra toda a infraestrutura:
- API (ConsultaDocumentos)
- SQL Server 2022
- Redis (cache)

### Iniciar todos os serviços

```bash
# Navegar até a pasta da solução
cd Rendimento.ConsultaDocumentos

# Iniciar todos os serviços
docker-compose up -d

# Verificar logs
docker-compose logs -f api

# Verificar status dos serviços
docker-compose ps
```

### Aplicar migrations

```bash
# Executar migrations no banco de dados
docker-compose exec api dotnet ef database update --project /src/ConsultaDocumentos.Infra.Data

# OU, se precisar executar do host:
dotnet ef database update \
  --project ConsultaDocumentos.Infra.Data \
  --startup-project ConsultaDocumentos.API \
  --connection "Server=localhost;Database=RendimentoConsultaDocumentos;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True;"
```

### Parar os serviços

```bash
# Parar todos os serviços
docker-compose down

# Parar e remover volumes (CUIDADO: apaga dados do banco!)
docker-compose down -v
```

## 🔧 Comandos Úteis

### Health Checks

```bash
# Verificar health básico
curl http://localhost:5000/health

# Verificar readiness (com formato JSON bonito)
curl -s http://localhost:5000/health/ready | jq

# Verificar health detalhado
curl -s http://localhost:5000/health/detailed | jq

# Monitorar health check em loop (útil para debugging)
watch -n 5 'curl -s http://localhost:5000/health/ready | jq'
```

### Docker

```bash
# Ver logs do container
docker logs -f consultadocumentos-api

# Ver logs apenas dos últimos 100 registros
docker logs --tail 100 consultadocumentos-api

# Executar comando dentro do container
docker exec -it consultadocumentos-api /bin/bash

# Ver estatísticas de recursos
docker stats consultadocumentos-api

# Reiniciar o container
docker restart consultadocumentos-api
```

### Docker Compose

```bash
# Iniciar apenas um serviço específico
docker-compose up -d api

# Reconstruir imagens antes de iniciar
docker-compose up -d --build

# Ver logs de um serviço específico
docker-compose logs -f api

# Escalar o serviço da API (múltiplas instâncias)
docker-compose up -d --scale api=3

# Ver variáveis de ambiente de um serviço
docker-compose exec api env
```

## 🔍 Troubleshooting

### Container não passa no health check

1. **Verificar logs do container:**
   ```bash
   docker logs consultadocumentos-api
   ```

2. **Verificar endpoint de health manualmente:**
   ```bash
   docker exec consultadocumentos-api curl -f http://localhost:8080/health
   ```

3. **Verificar se o SQL Server está acessível:**
   ```bash
   docker-compose exec api ping sqlserver
   ```

### Banco de dados não conecta

1. **Verificar se o SQL Server está rodando:**
   ```bash
   docker-compose ps sqlserver
   ```

2. **Verificar logs do SQL Server:**
   ```bash
   docker-compose logs sqlserver
   ```

3. **Testar conexão direta com SQL Server:**
   ```bash
   docker-compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P YourStrong@Password123 -C -Q "SELECT 1"
   ```

### Redis não conecta

1. **Verificar se o Redis está rodando:**
   ```bash
   docker-compose ps redis
   ```

2. **Testar conexão com Redis:**
   ```bash
   docker-compose exec redis redis-cli ping
   ```

3. **Desabilitar Redis temporariamente:**
   Edite o `docker-compose.yml` e defina:
   ```yaml
   - Redis__Enabled=false
   ```

### Aplicação está lenta

1. **Verificar recursos do container:**
   ```bash
   docker stats consultadocumentos-api
   ```

2. **Verificar health check detalhado para identificar gargalos:**
   ```bash
   curl -s http://localhost:5000/health/detailed | jq '.entries | to_entries | map({key: .key, duration: .value.duration})'
   ```

## 🚀 Produção

### Recomendações para ambiente de produção:

1. **Variáveis de ambiente sensíveis**: Use secrets management (Azure Key Vault, AWS Secrets Manager, etc.)

2. **Configurar limites de recursos no docker-compose:**
   ```yaml
   services:
     api:
       deploy:
         resources:
           limits:
             cpus: '2'
             memory: 2G
           reservations:
             cpus: '1'
             memory: 1G
   ```

3. **Configurar política de restart:**
   ```yaml
   restart: unless-stopped
   ```

4. **Usar volumes para persistência:**
   - Banco de dados (já configurado)
   - Logs da aplicação
   - Certificados SSL

5. **Configurar reverse proxy (Nginx/Traefik)** para SSL/TLS

6. **Implementar log aggregation** (ELK Stack, Grafana Loki, etc.)

7. **Configurar monitoramento** (Prometheus, Grafana, Application Insights)

## 📊 Integração com Kubernetes

Os health checks estão prontos para uso com Kubernetes:

```yaml
apiVersion: v1
kind: Pod
metadata:
  name: consultadocumentos-api
spec:
  containers:
  - name: api
    image: consultadocumentos-api:latest
    livenessProbe:
      httpGet:
        path: /health
        port: 8080
      initialDelaySeconds: 30
      periodSeconds: 30
      timeoutSeconds: 10
      failureThreshold: 3
    readinessProbe:
      httpGet:
        path: /health/ready
        port: 8080
      initialDelaySeconds: 10
      periodSeconds: 10
      timeoutSeconds: 5
      failureThreshold: 3
```

## 📝 Notas

- O endpoint `/health` é usado pelo Docker HEALTHCHECK no Dockerfile
- O endpoint `/health/ready` deve ser usado para readiness checks em orquestradores
- O endpoint `/health/detailed` é útil para debugging e monitoramento manual
- Todos os health checks são não-bloqueantes e têm timeouts configurados
- O health check de provedores externos verifica apenas configuração (não faz chamadas reais em produção)
