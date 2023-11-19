# Use the official Microsoft .NET 7 SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Auth.MicroService.sln .
COPY src/Auth.MicroService.WebApi/*.csproj ./src/Auth.MicroService.WebApi/
RUN dotnet restore

# Copy everything else and build
COPY src/Auth.MicroService.WebApi/. ./src/Auth.MicroService.WebApi/
WORKDIR /app/src/Auth.MicroService.WebApi
RUN dotnet build -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/src/Auth.MicroService.WebApi/out .
ENTRYPOINT ["dotnet", "Auth.MicroService.WebApi.dll"]
