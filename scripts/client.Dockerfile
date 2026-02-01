FROM mcr.microsoft.com/dotnet/sdk:8.0 AS generator

# Install NSwag CLI globally
RUN dotnet tool install --global NSwag.ConsoleCore --version 14.4.0
ENV PATH="$PATH:/root/.dotnet/tools"

# Install curl
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy generator script
COPY script-client.sh .
  
# Run script
CMD ["sh", "script-client.sh"]