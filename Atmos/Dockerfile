# Stage 1: Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-windowsservercore-ltsc2025 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 2: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0-windowsservercore-ltsc2025 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["/API/API.csproj", "API/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Domain/Domain.csproj", "Domain/"]
RUN dotnet restore "API/API.csproj"

# Copy the rest of the source
COPY . .

# Build
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -r win-x64 --self-contained false -c Release -o /app/build

# Stage 3: Publish
FROM build AS publish
RUN dotnet publish "API.csproj" -r win-x64 --self-contained false -c Release -o /app/publish /p:UseAppHost=false 

# Stage 4: Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "atmos.exe"]
