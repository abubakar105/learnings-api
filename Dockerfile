FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["Learnings.Api/Learnings.Api.csproj", "Learnings.Api/"]
COPY ["Learnings.Application/Learnings.Application.csproj", "Learnings.Application/"]
COPY ["Learnings.Domain/Learnings.Domain.csproj", "Learnings.Domain/"]
COPY ["Learnings.Infrastructure/Learnings.Infrastructure.csproj", "Learnings.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Learnings.Api/Learnings.Api.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/Learnings.Api"
RUN dotnet build "Learnings.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "Learnings.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Learnings.Api.dll"]