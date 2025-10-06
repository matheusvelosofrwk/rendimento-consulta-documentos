# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["ConsultaDocumentos.API/ConsultaDocumentos.API.csproj", "ConsultaDocumentos.API/"]
COPY ["ConsultaDocumentos.Application/ConsultaDocumentos.Application.csproj", "ConsultaDocumentos.Application/"]
COPY ["ConsultaDocumentos.Domain/ConsultaDocumentos.Domain.csproj", "ConsultaDocumentos.Domain/"]
COPY ["ConsultaDocumentos.Infra.Data/ConsultaDocumentos.Infra.Data.csproj", "ConsultaDocumentos.Infra.Data/"]
COPY ["ConsultaDocumentos.Infra.Ioc/ConsultaDocumentos.Infra.Ioc.csproj", "ConsultaDocumentos.Infra.Ioc/"]

RUN dotnet restore "ConsultaDocumentos.API/ConsultaDocumentos.API.csproj"

# Copiar todo o código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/ConsultaDocumentos.API"
RUN dotnet build "ConsultaDocumentos.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ConsultaDocumentos.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalar curl para healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copiar arquivos publicados
COPY --from=publish /app/publish .

# Criar usuário não-root para segurança
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

# Expor portas
EXPOSE 8080
EXPOSE 8081

# Variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Configurar healthcheck
# O Docker executará este comando a cada 30 segundos
# Se falhar 3 vezes consecutivas (90s), o container será marcado como unhealthy
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "ConsultaDocumentos.API.dll"]
