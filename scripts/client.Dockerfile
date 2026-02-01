FROM mcr.microsoft.com/dotnet/sdk:8.0 AS generator

ARG SCHEMA_URL
ARG NAMESPACE="NotificationService.ShiftserviceClient"
ARG OUTPUT_FILE="ShiftserviceClient.cs"

WORKDIR /app

# Install NSwag CLI globally
RUN dotnet tool install --global NSwag.ConsoleCore --version 14.4.0
ENV PATH="$PATH:/root/.dotnet/tools"

# Download OpenAPI schema
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
RUN curl -L -o openapi.json "$SCHEMA_URL"

# Generate C# client
RUN nswag openapi2csclient \
    /input:openapi.json \
    /output:$OUTPUT_FILE \
    /namespace:$NAMESPACE

# Optional: list generated file
CMD ["ls", "-la", "/app"]