# DOCKER
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# Copy Files to var/www/app
COPY WebApi /var/www/app/API
COPY Application /var/www/app/Application
COPY Core /var/www/app/Core
COPY Infrastructure /var/www/app/Infrastructure

# Set enviroments if release version
RUN dotnet publish /var/www/app/API -c Release -o out

# Build projecct
WORKDIR /var/www/app/API
RUN dotnet build WebApi.csproj -c Release -o /var/www/app/build

# Generate publish version to production
FROM build AS publish
WORKDIR /var/www/app/API
RUN dotnet publish WebApi.csproj -c Release -o /var/www/app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /var/www/app/API
COPY --from=publish /var/www/app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
EXPOSE 9501