FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY PortfolioManagement.sln ./
COPY src/PortfolioManagement.Domain/PortfolioManagement.Domain.csproj src/PortfolioManagement.Domain/
COPY src/PortfolioManagement.Application/PortfolioManagement.Application.csproj src/PortfolioManagement.Application/
COPY src/PortfolioManagement.Infrastructure/PortfolioManagement.Infrastructure.csproj src/PortfolioManagement.Infrastructure/
COPY src/PortfolioManagement.Api/PortfolioManagement.Api.csproj src/PortfolioManagement.Api/
RUN dotnet restore src/PortfolioManagement.Api/PortfolioManagement.Api.csproj
COPY . .
RUN dotnet publish src/PortfolioManagement.Api/PortfolioManagement.Api.csproj -c Release -o /app/publish --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PortfolioManagement.Api.dll"]
