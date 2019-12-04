# This Dockerfile contains Build and Release steps:
# 1. Build image (https://hub.docker.com/_/microsoft-dotnet-core-sdk/)
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.100-alpine3.10 AS build
WORKDIR /source

# Cache nuget restore
COPY /src/XO/*.csproj XO/
COPY /src/XO.Web/*.csproj XO.Web/
RUN dotnet restore XO.Web/XO.Web.csproj

# Copy sources and compile
COPY /src .
WORKDIR /source/XO.Web
RUN dotnet publish XO.Web.csproj --output /app/ --configuration Release

# 2. Release image (https://hub.docker.com/_/nginx)
FROM nginx:1.15.12-alpine
WORKDIR /usr/share/nginx/html/

EXPOSE 80
EXPOSE 443

# Copy content from Build image
COPY --from=build /app/XO.Web/dist .
