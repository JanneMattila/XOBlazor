# This Dockerfile contains Build and Release steps:
# 1. Build image
FROM mcr.microsoft.com/dotnet/core-nightly/sdk:3.0.100-preview4-alpine3.9 AS build
WORKDIR /source

# Cache nuget restore
COPY /src/XO/*.csproj XO/
COPY /src/XO.App/*.csproj XO.App/
RUN dotnet restore XO.App/XO.App.csproj

# Copy sources and compile
COPY /src .
WORKDIR /source/XO.App
RUN dotnet publish XO.App.csproj --output /app/ --configuration Release

RUN ls -lF /app/XO.App/dist

# 2. Release image
FROM nginx:1.15.12-alpine
WORKDIR /usr/share/nginx/html/

EXPOSE 80
EXPOSE 443

# Copy content from Build image
COPY --from=build /app/XO.App/dist .
