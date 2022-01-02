# DOCKER
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Copy Files to var/www/app
COPY Web/Api /var/www/app/Web/Api
COPY Application /var/www/app/Application
COPY Core /var/www/app/Core
COPY Infrastructure /var/www/app/Infrastructure

# Set enviroments if release version
RUN dotnet publish /var/www/app/Web/Api -c Release -o out

# Build projecct
WORKDIR /var/www/app/Web/Api
RUN dotnet build Api.csproj -c Release -o /var/www/app/build

# Generate publish version to production
FROM build AS publish
WORKDIR /var/www/app/Web/Api
RUN dotnet publish Api.csproj -c Release -o /var/www/app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /var/www/app/API
COPY --from=publish /var/www/app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
EXPOSE 9501