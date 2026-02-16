# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Cloud9.2.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish ./

# Render provides $PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

# Optional (Render doesn't rely on EXPOSE, but it's fine)
EXPOSE 8080

ENTRYPOINT ["dotnet", "Cloud9.2.dll"]
