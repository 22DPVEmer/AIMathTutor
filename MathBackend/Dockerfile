# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["MathTutor.API/MathTutor.API.csproj", "MathTutor.API/"]
COPY ["MathTutor.Application/MathTutor.Application.csproj", "MathTutor.Application/"]
COPY ["MathTutor.Core/MathTutor.Core.csproj", "MathTutor.Core/"]
COPY ["MathTutor.Infrastructure/MathTutor.Infrastructure.csproj", "MathTutor.Infrastructure/"]
COPY ["MathTutor.Tests/MathTutor.Tests.csproj", "MathTutor.Tests/"]

# Restore dependencies
RUN dotnet restore "MathTutor.API/MathTutor.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/MathTutor.API"
RUN dotnet build "MathTutor.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MathTutor.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8.0 SDK image for the final stage (includes runtime + SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Install tools for migrations and debugging
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Install EF Core tools globally
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy the published application
COPY --from=publish /app/publish .

# Install PostgreSQL client for health checks and debugging
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Expose the port the app runs on
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "MathTutor.API.dll"]
