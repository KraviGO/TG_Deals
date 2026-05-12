FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем общую конфигурацию NuGet-пакетов.
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]

# Сначала копируем csproj-файлы, чтобы Docker кешировал restore отдельным слоем.
COPY ["src/Services/ApiGateway/ApiGateway.Host/ApiGateway.Host.csproj", "src/Services/ApiGateway/ApiGateway.Host/"]
COPY ["src/BuildingBlocks/Marketplace.Observability/Marketplace.Observability.csproj", "src/BuildingBlocks/Marketplace.Observability/"]

RUN dotnet restore "src/Services/ApiGateway/ApiGateway.Host/ApiGateway.Host.csproj"

# Затем копируем остальные исходники и публикуем приложение.
COPY ["src/Services/ApiGateway/ApiGateway.Host/", "src/Services/ApiGateway/ApiGateway.Host/"]
COPY ["src/BuildingBlocks/Marketplace.Observability/", "src/BuildingBlocks/Marketplace.Observability/"]

WORKDIR "/src/src/Services/ApiGateway/ApiGateway.Host"
RUN dotnet build "ApiGateway.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.Host.dll"]
