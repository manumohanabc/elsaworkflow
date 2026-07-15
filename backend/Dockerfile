# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file
COPY ElsaWorkflow.Api/ElsaWorkflow.Api.csproj ElsaWorkflow.Api/

# Restore dependencies
RUN dotnet restore ElsaWorkflow.Api/ElsaWorkflow.Api.csproj

# Copy source code
COPY ElsaWorkflow.Api/ ElsaWorkflow.Api/

# Build and publish
RUN dotnet publish ElsaWorkflow.Api/ElsaWorkflow.Api.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish .

# Expose port
EXPOSE 10000

# Set environment
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD dotnet-health-check || exit 1

# Run
ENTRYPOINT ["dotnet", "ElsaWorkflow.Api.dll"]
