# This Dockerfile contains Build and Release steps:
# 1. Build image
FROM mcr.microsoft.com/dotnet/core-nightly/sdk:3.0.100-preview4-alpine3.9 AS build
WORKDIR /source

# Cache nuget restore
COPY /src/XO/*.csproj XO/
COPY /src/XO.Web/*.csproj XO.Web/
RUN dotnet restore XO.Web/XO.Web.csproj

# Copy sources and compile
COPY /src .
WORKDIR /source/XO.Web
RUN dotnet publish XO.Web.csproj --output /app/ --configuration Release

# 2. Release image
FROM mcr.microsoft.com/dotnet/core-nightly/aspnet:3.0.0-preview4-alpine3.9
WORKDIR /app
EXPOSE 80

# Copy content from Build image
COPY --from=build /app .

ENTRYPOINT ["dotnet", "XO.Web.dll"]
