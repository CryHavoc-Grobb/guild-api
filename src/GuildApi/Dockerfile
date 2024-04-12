# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore any dependencies (via NuGet)
COPY src/GuildApi/GuildApi.csproj ./src/GuildApi/
RUN dotnet restore ./src/GuildApi/GuildApi.csproj

# Copy the project files and build the release
COPY src/GuildApi/ ./src/GuildApi/
RUN dotnet publish ./src/GuildApi/GuildApi.csproj -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "GuildApi.dll"]
