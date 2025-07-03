# Estágio 1: Build da aplicação
# Usamos a imagem do SDK do .NET 9.0 para compilar a aplicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia o arquivo .csproj e restaura as dependências primeiro
# Isso aproveita o cache do Docker. Se o .csproj não mudar, o restore não é re-executado.
COPY *.csproj ./
RUN dotnet restore

# Copia o resto do código fonte da aplicação
COPY . ./

# Publica a aplicação em modo Release para uma pasta específica
RUN dotnet publish -c Release -o /app/publish

# ---

# Estágio 2: Imagem final de execução
# Usamos a imagem do ASP.NET Runtime, que é muito menor que a do SDK
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia os arquivos publicados do estágio de build para a imagem final
COPY --from=build /app/publish .

# Expõe a porta 8080 (porta padrão que o Render espera)
EXPOSE 8080

# Define o ponto de entrada para executar a aplicação quando o contêiner iniciar
ENTRYPOINT ["dotnet", "RegistroPonto.Api.dll"]
